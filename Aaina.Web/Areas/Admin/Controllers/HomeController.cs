using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Aaina.Web.Areas.Admin.Controllers
{

    public class HomeController : BaseController
    {
        
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IPushNotificationService pushNotificationService;
        private readonly IGameService gameService;
        private readonly IUserLoginService userLoginService;
        private readonly ITeamService teamService;
        private readonly INotificationService _notificationService;
        private readonly IWeightageService weightageService;
        private readonly IHostingEnvironment env;
        public HomeController(IPushNotificationService pushNotificationService,
            IGameService gameService, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager,
            IUserLoginService userLoginService, ITeamService teamService, INotificationService notificationService, IHostingEnvironment env, IWeightageService weightageService)
        {
            this.pushNotificationService = pushNotificationService;
            this.gameService = gameService;
            this.userLoginService = userLoginService;
            this.teamService = teamService;            
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;
            _notificationService = notificationService;
            this.env = env;
            this.weightageService = weightageService;
        }
        public async Task<IActionResult> Index()
        {
            // List<GoogleDriveFile> allFiles = GoogleDriveAPIHelper.GetDriveFiles(this.env);
            //GoogleDriveAPIHelper.CreateFolder(this.env,CurrentUser.CompanyId.ToString());
            //GoogleDriveAPIHelper.CreateFolderInFolder(this.env, CurrentUser.CompanyId.ToString(),CurrentUser.UserId.ToString());

            LeftMenuDto leftMenu = new LeftMenuDto();
            var gamemenu = await gameService.GetMenu(CurrentUser.CompanyId);
            leftMenu.LeftMenu = gamemenu;            
            leftMenu.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            PermissionHelper.SetPermission(JsonConvert.SerializeObject(leftMenu), CurrentUser.UserId);

            var defaultGame = gameService.GetFirstGame(CurrentUser.CompanyId);
            if (defaultGame != null)
            {
                return RedirectToAction("gamefeebback", "game", new { Area = "admin", routegameid = defaultGame.Id, id = defaultGame.Id });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult PushNotification()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PushNotification(IFormCollection fc)
        {
            var title = fc["title"];
            var Desciption = fc["Desciption"];
            var allUser = pushNotificationService.GetUserToken(CurrentUser.CompanyId);
            List<PushNotificationItemDto> GetNotificationToken = allUser.Select(x =>
            new PushNotificationItemDto()
            {
                Token = x.TokenId,
                Description = Desciption,
                Text = title,
                click_action = "https://staging.peoplepro.online/Company/ManageStaff"
            }).ToList();

            await pushNotificationService.PushNotifications(GetNotificationToken);
            return View();
        }

        [Route("/home/teams")]
        public IActionResult Teams()
        {
            List<SelectedItemDto> teams = new List<SelectedItemDto>();
            teams = teamService.GetTeamList(null, CurrentUser.CompanyId);
            return Json(new { items = teams });
        }

        [Route("/home/players")]
        public IActionResult Players()
        {
            List<SelectedItemDto> users = new List<SelectedItemDto>();
            users = userLoginService.GetTeamPlayers(CurrentUser.CompanyId);
            return Json(new { items = users });
        }

        public IActionResult SendToUser()
        {
            NotificationModel vModel = new NotificationModel();
            return View(vModel);
        }



        [NonAction]
        private async Task<bool> SaveNotification(NotificationModel vModel)
        {
            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            if (vModel.ConnectedPlayerIds != null && vModel.ConnectedPlayerIds.Count() > 0)
            {
                foreach (var connItem in vModel.ConnectedPlayerIds)
                {
                    NotificationReminderDto NCModel = new NotificationReminderDto();
                    NCModel.SendTo = Convert.ToInt32(connItem);
                    NCModel.SendBy = CurrentUser.UserId;
                    NCModel.SendDate = DateTime.Now;
                    NCModel.Reason = vModel.Region;
                    NCModel.Message = vModel.Message;
                    NCModel.NotificationType = (int)NotificationType.Notification;
                    NCModel.IsRead = true;
                    NCModel.ReadDate = DateTime.Now;
                    notifyModel.Add(NCModel);
                }
            }

            if (vModel.NotConnectedPlayerIds != null && vModel.NotConnectedPlayerIds.Count() > 0)
            {
                foreach (var connItem in vModel.NotConnectedPlayerIds)
                {
                    NotificationReminderDto NNCModel = new NotificationReminderDto();
                    NNCModel.SendTo = Convert.ToInt32(connItem);
                    NNCModel.SendBy = CurrentUser.UserId;
                    NNCModel.SendDate = DateTime.Now;
                    NNCModel.Reason = vModel.Region;
                    NNCModel.Message = vModel.Message;
                    NNCModel.NotificationType = (int)NotificationType.Notification;
                    NNCModel.IsRead = false;
                    notifyModel.Add(NNCModel);
                }
            }
            await _notificationService.AddReminderNotification(notifyModel);
            return true;
        }


        [HttpPost]
        public async Task<IActionResult> SendToUser(NotificationModel model)
        {
            string customMsg = string.Empty;
            bool msgSend = false;
            if (model.TeamIds!=null && model.TeamIds.Count() > 0 || model.PlayerIds!=null && model.PlayerIds.Count() > 0)
            {
                var msgHeader = "<h3>Message has been successfully sent to .</h3><br/>";
                string succsMsg = string.Empty;
                string errorMsg = string.Empty;

                string[] allPlayerIds = new string[0];
                var teamPlayerIds = teamService.TeamPlayerIds(Array.ConvertAll(model.TeamIds, s => int.Parse(s)));
                if (teamPlayerIds != null && teamPlayerIds.Count() > 0 && model.PlayerIds != null && model.PlayerIds.Count() > 0)
                {
                    allPlayerIds = model.PlayerIds.Union(teamPlayerIds).ToArray();
                }
                else if ((model.PlayerIds == null || model.PlayerIds.Count() == 0) && teamPlayerIds != null && teamPlayerIds.Count() > 0)
                {
                    allPlayerIds = teamPlayerIds;
                }
                else if ((teamPlayerIds == null || teamPlayerIds.Count() == 0) && (model.PlayerIds != null && model.PlayerIds.Count() > 0))
                {
                    allPlayerIds = model.PlayerIds;
                }

                var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
                if (connections != null && connections.Count > 0)
                {
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", model.Region, model.Message);
                    var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
                    var connectedUserIds = userIds.Item1.ToArray();
                    model.ConnectedPlayerIds = connectedUserIds;
                    if (connectedUserIds != null && connectedUserIds.Count() > 0)
                    {
                        var users = userLoginService.GetConnectedPlayers(Array.ConvertAll(connectedUserIds, s => int.Parse(s)));
                        succsMsg = "<span>";
                        succsMsg = "<h3>Online users.</h3>";
                        succsMsg += "<br/>";
                        foreach (var item in users)
                        {
                            succsMsg += item.Name + "<br/> ";
                        }
                        succsMsg += "</span>" + "<br/>";
                    }

                    var notConnectedUserIds = userIds.Item2.ToArray();
                    model.NotConnectedPlayerIds = notConnectedUserIds;
                    if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                    {
                        var users = userLoginService.GetConnectedPlayers(Array.ConvertAll(notConnectedUserIds, s => int.Parse(s)));
                        errorMsg = "<span>";
                        errorMsg = "<h3>Offline Users</h3>";
                        errorMsg += "<br/>";
                        foreach (var item in users)
                        {
                            errorMsg += item.Name + "<br/> ";
                        }
                        errorMsg += "</span>";
                    }

                    customMsg = msgHeader + succsMsg + errorMsg;

                }
                else
                {
                    var notConnectedUserIds = _userConnectionManager.GetHubUsers(allPlayerIds).Item2.ToArray();
                    model.NotConnectedPlayerIds = notConnectedUserIds;
                    if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                    {
                        var users = userLoginService.GetConnectedPlayers(Array.ConvertAll(notConnectedUserIds, s => int.Parse(s)));
                        errorMsg = "<span>";
                        errorMsg = "<h3>Offline Users</h3>";
                        errorMsg += "<br/>";
                        foreach (var item in users)
                        {
                            errorMsg += item.Name + "<br/> ";
                        }
                        errorMsg += "</span>";
                        customMsg = msgHeader + succsMsg + errorMsg;
                    }
                    else
                    {
                        customMsg = "Record not found !!";
                    }
                }
                msgSend = true;
                await SaveNotification(model); // save Notification into DB..
            }
            else
            {
                msgSend = false;
                customMsg = "Please select at least one team / player";
            }
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                IsSuccess = msgSend,
                Message = customMsg
            });
        }

       
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aaina.Web.Models;
using Aaina.Service;
using Aaina.Dto;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Aaina.Common;
using Dropbox.Api;

namespace Aaina.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IPushNotificationService pushNotificationService;
        private readonly IGameService gameService;
        private readonly INotificationService notificationService;
        private readonly ISessionService sessionService;
        private readonly IGoogleDriveService googleDriveService;
        private readonly IDropBoxService dropBoxService;
        public HomeController(IPushNotificationService pushNotificationService,
            IGameService gameService, INotificationService notificationService,
            ISessionService sessionService, IGoogleDriveService googleDriveService, IDropBoxService dropBoxService)
        {
            this.pushNotificationService = pushNotificationService;
            this.gameService = gameService;
            this.notificationService = notificationService;
            this.sessionService = sessionService;
            this.googleDriveService = googleDriveService;
            this.dropBoxService = dropBoxService;
        }

        // GET: /Home/Connect
        public ActionResult Connect()
        {

            var connectState = Guid.NewGuid().ToString("N");

            var redirect = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, SiteKeys.Dropbox_APIKey, SiteKeys.Dropbox_RedirectUri, connectState);
            return Redirect(redirect.ToString());
        }

        [Route("authorize")]
        public ActionResult Authorized(string id)
        {


            return View();
        }

        // GET: /Home/Auth
        public async Task<ActionResult> AuthAsync(string code, string state)
        {

            OAuth2Response response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, SiteKeys.Dropbox_APIKey, SiteKeys.Dropbox_ApiSecret, SiteKeys.Dropbox_RedirectUri);

            return Json(response.AccessToken);
        }

        public IActionResult Index(string ty = "s", string path = "")
        {

            var defaultGame = gameService.GetFirstGame(CurrentUser.CompanyId);
            if (defaultGame != null && ty == "s")
            {
                if (CurrentUser.RoleId == (int)UserType.Admin)
                {
                    return Redirect($"/{defaultGame.Id}/game/gamefeebback");
                }
                else
                {
                    return Redirect($"/{defaultGame.Id}/project/gamefeebback");
                }
                //return Json(new RequestOutcome<string> { RedirectUrl = $"/{defaultGame.Id}/project/gamefeebback", IsSuccess = true });
            }
            else
            {
                ViewBag.path = path;
                List<DropboxDiles> list = dropBoxService.GetFileFolders(path);
                return View(list);
            }
        }



        public IActionResult UploadFile()
        {
            string path = Convert.ToString(Request.Form["path"]);
            if (Request.Form != null && Request.Form.Files != null)
            {
                dropBoxService.Upload(Request.Form.Files[0], path);
            }

            return Redirect($"/home/index?ty=l&path={path}");

        }

        public IActionResult folderCreate()
        {
            string path = Convert.ToString(Request.Form["path"]);
            if (Request.Form != null && !string.IsNullOrEmpty(Convert.ToString(Request.Form["folderName"])))
            {
                dropBoxService.CreateFolder($"{path}/{Convert.ToString(Request.Form["folderName"])}");
            }

            return Redirect($"/home/index?ty=l&path={path}");

        }

        public async Task<IActionResult> DeleteFile(string path, string retunPath)
        {
            if (!string.IsNullOrEmpty(path))
            {
                dropBoxService.Delete(path);
            }

            return Redirect($"/home/index?ty=l&path={retunPath}");

        }

        public IActionResult ShareLink(string path)
        {
            return Json(dropBoxService.PrivateShare(path));

        }



        [HttpGet]
        public IActionResult MessageCount()
        {
            var msgCount = notificationService.UnreadMessageCount(CurrentUser.UserId);
            return Json(msgCount);
        }

        [HttpGet]
        public IActionResult ShowNotificationMessage(int page)
        {
            List<PendingNotificationDto> notificationMsg = notificationService.GetPlayerPendingMessage(CurrentUser.UserId, page);
            return Json(notificationMsg);
        }

        public IActionResult PushNotification()
        {
            return View();
        }
        public IActionResult PushNotificationSave(string token)
        {
            this.pushNotificationService.Add(CurrentUser.UserId, token);
            return Json(new { issucess = true });
        }

        public IActionResult PushNotificationDelete(string token)
        {
            this.pushNotificationService.Delete(CurrentUser.UserId, token);
            return Json(new { issucess = true });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Error = exception });
            }
        }


    }
}

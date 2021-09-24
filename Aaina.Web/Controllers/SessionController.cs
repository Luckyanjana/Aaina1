using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Dto;

using Aaina.Service;
using Aaina.Web.Models;
using Aaina.Web.Models.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Wkhtmltopdf.NetCore;

namespace Aaina.Web.Controllers
{
    public class SessionController : BaseController
    {
        private readonly IGeneratePdf generatePdf;
        private readonly ISessionService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly INotificationService notificationService;
        private readonly IPreSessionGroupService preSessionGroupService;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IDropBoxService dropBoxService;
        private readonly IUnitOfWork unitOfWork;

        public SessionController(ISessionService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService, INotificationService notificationService, IUserConnectionManager userConnectionManager,
            IHubContext<NotificationUserHub> notificationUserHubContext, IPreSessionGroupService preSessionGroupService, IGeneratePdf generatePdf, IDropBoxService dropBoxService,
            IUnitOfWork unitOfWork)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
            this.notificationService = notificationService;
            this._userConnectionManager = userConnectionManager;
            this._notificationUserHubContext = notificationUserHubContext;
            this.preSessionGroupService = preSessionGroupService;
            this.generatePdf = generatePdf;
            this.dropBoxService = dropBoxService;
            this.unitOfWork = unitOfWork;


        }
        public IActionResult Index(int tenant)
        {
            ViewBag.TypeList = Enum.GetValues(typeof(SessionType)).Cast<SessionType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = ((int)c) }).ToList();
            ViewBag.ModeList = Enum.GetValues(typeof(SessionMode)).Cast<SessionMode>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = ((int)c) }).ToList();
            return View();
        }

        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SessionDto model, int tenant)
        {
            return await AddEdit(model, tenant);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int tenant, int? copyId)
        {
            return await AddEdit(id, tenant, copyId);
        }

        private async Task<IActionResult> AddEdit(SessionDto model, int tenant)
        {

            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.SessionScheduler == null || !model.SessionScheduler.StartDate.HasValue)
            {
                ModelState.AddModelError("", $"Session Scheduler Required!");
                return CreateModelStateErrors();
            }

            model.SessionReminder = model.SessionReminder.Where(x => x.Every.HasValue && x.TypeId.HasValue && x.Unit.HasValue).ToList();

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.SessionScheduler.Type == (int)ScheduleType.Recurring && !model.SessionScheduler.RecurseEvery.HasValue)
            {
                ModelState.AddModelError("", $"Recurse Every is required");
                return CreateModelStateErrors();
            }

            if (model.SessionScheduler.DailyFrequency == (int)ScheduleDailyFrequency.Every)
            {
                bool isValid = true;

                if (!model.SessionScheduler.OccursEveryValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Value is required");
                }

                if (!model.SessionScheduler.OccursEveryTimeUnit.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Time Unit is required");
                }

                if (!isValid)
                {
                    return CreateModelStateErrors();
                }
            }


            if (model.SessionScheduler != null && model.SessionScheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.SessionScheduler.DaysOfWeek = string.Join(",", model.SessionScheduler.DaysOfWeekList);
            }

            try
            {
                //unitOfWork.CreateTransaction();
                if (model.Id > 0)
                {
                    await service.AddUpdateAsync(model);
                    ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
                }
                else
                {
                    model.CompanyId = CurrentUser.CompanyId;
                    model.CreatedBy = CurrentUser.UserId;
                    model.CompanyId = CurrentUser.CompanyId;
                    var sessionId = await service.AddUpdateAsync(model);
                    model.SessionId = sessionId;
                    await UserNotification(model); //Save & Send Notification
                    ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
                }

              ///  unitOfWork.Commit();
            }
            catch (Exception ex)
            {
               // unitOfWork.Rollback();

            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/session/index", IsSuccess = true });
        }
        private async Task<IActionResult> AddEdit(int? id, int tenant, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }

            SessionDto model = new SessionDto()
            {
                SessionScheduler = new SessionSchedulerDto()
                {
                    Type = (int)ScheduleType.OneTime
                }
            };
            string gameName = string.Empty;
            if (tenant == 0)
            {
                var game = gameService.GetFirstGame(CurrentUser.CompanyId);
                tenant = game.Id.Value;
                gameName = game.Name;
            }
            else
            {
                var game = await gameService.GetDetailsId(tenant);
                gameName = game.Name;
            }

            model.GameId = tenant;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            model.Game = gameName;
            model.TypeList = Enum.GetValues(typeof(SessionType)).Cast<SessionType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.ModeList = Enum.GetValues(typeof(SessionMode)).Cast<SessionMode>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ParticipantTypeList = Enum.GetValues(typeof(ParticipantType)).Cast<ParticipantType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.SessionScheduler != null && !string.IsNullOrEmpty(model.SessionScheduler.DaysOfWeek))
            {
                model.SessionScheduler.DaysOfWeekList = model.SessionScheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await gameService.GetGamlePlayer(CurrentUser.CompanyId)).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.UserId.ToString(), Additional = s.UserTypeId.ToString() }).ToList();
            model.GameList =
                gameService.GetAllDropSecondParent(CurrentUser.CompanyId).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.Id.ToString() }).ToList();
            gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.Id }).ToList();
            model.AllRecord = model.GameId.HasValue ? service.GetAllByGameId(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, model.GameId.Value) : new List<SessionDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }
        private async Task<bool> UserNotification(SessionDto model)
        {
            string[] userIds = new string[0];
            if (model.SessionParticipant != null && model.SessionParticipant.Count > 0)
            {
                string notificationMessage = $"New Session Request Notification {model.Name} {model.SessionId}";
                //userIds = model.SessionParticipant.Where(x => x.ParticipantTyprId == (int)ParticipantType.DecisionMaker).Select(y => y.UserId.Value.ToString()).ToArray();
                if (userIds == null || userIds.Count() == 0)
                {
                    userIds = model.SessionParticipant.Where(x => x.ParticipantTyprId != (int)ParticipantType.DecisionMaker).Select(y => y.UserId.Value.ToString()).ToArray();
                    //.ConvertAll<string>(delegate (int i) { return i.ToString(); });
                }
                if (userIds != null && userIds.Count() > 0)
                {
                    var connections = _userConnectionManager.GetUserAllConnections(userIds);
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sessionNotification", "Confirmation", notificationMessage);
                    List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
                    foreach (var userId in userIds)
                    {
                        NotificationReminderDto NNCModel = new NotificationReminderDto();
                        NNCModel.SendTo = Convert.ToInt32(userId);
                        NNCModel.SendBy = CurrentUser.UserId;
                        NNCModel.SendDate = DateTime.Now;
                        NNCModel.Reason = Convert.ToString(model.SessionId);
                        NNCModel.Message = notificationMessage;
                        NNCModel.NotificationType = (int)NotificationType.SessionNotifiction;
                        NNCModel.IsRead = false;
                        notifyModel.Add(NNCModel);
                    }

                    await notificationService.AddReminderNotification(notifyModel);
                }
            }

            return true;
        }


        [HttpGet]
        public IActionResult Delete(int id, int tenant)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Session?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Session" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int tenant, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteById(id);
                ShowSuccessMessage("Success!", $"Look has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/session/index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }


        [HttpGet]
        public IActionResult GetSessionPendingMessage(int id, int sessionId)
        {
            PendingNotificationDto vModel = new PendingNotificationDto();
            vModel.UserId = id;
            vModel.SessionId = sessionId;
            return PartialView("_ReplyPendingMessage", vModel);
        }

        [HttpGet]
        public IActionResult SessionMessageCount()
        {
            var msgCount = notificationService.UnreadMessageTotalCount(CurrentUser.UserId, (int)NotificationType.SessionNotifiction);
            return Json(msgCount);
        }

        [HttpGet]
        public async Task<IActionResult> PendingMessage()
        {
            List<SessionDto> sessionDtos = new List<SessionDto>();
            List<PendingNotificationDto> pendingNotifications = notificationService.GetPendingMessage(CurrentUser.UserId, (int)NotificationType.SessionNotifiction);
            foreach (var item in pendingNotifications.OrderByDescending(o => o.SendDate).ToList())
            {
                SessionDto sessionDto = new SessionDto();
                sessionDto.NotificationId = item.Id;
                var session = await service.GetSessionByUserId(Convert.ToInt32(item.Reason), CurrentUser.UserId);
                if (session != null)
                {
                    sessionDto.Id = session.Id;
                    sessionDto.Name = session.Name; // Session Name
                    sessionDto.MessageStatus = session.MessageStatus; // Message Status
                }
                sessionDto.PendingMessage = item.Message; // Message
                sessionDtos.Add(sessionDto);
            }
            return View(sessionDtos);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSessionNotification(int userId, int sessionId, string acceptType, string remarks)
        {
            var notification = notificationService.GetNotificationById(userId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                notificationService.UpdateNotification(notification);
            }

            var sessionParticipant = service.GetSessionParticipantByUserId(sessionId, notification.SendTo);
            if (sessionParticipant != null)
            {
                sessionParticipant.Status = acceptType == "A" ? (int)ConfirmStatus.Confirmed : (int)ConfirmStatus.Rejected;
                sessionParticipant.Remarks = remarks;
                service.UpdateSessionParticipant(sessionParticipant);
            }
            if (acceptType == "A" && service.IsDecisionMaker(notification.SendTo, sessionId))
            {
                string notificationMessage = $"New Session Request Notification {sessionId}";
                var partiOtherUser = service.GetSessionParticipantNonMake(sessionId);
                var list = partiOtherUser.Select(x => new NotificationReminderDto()
                {
                    SendTo = x.UserId.Value,
                    SendDate = DateTime.Now,
                    Reason = x.SessionId.ToString(),
                    Message = notificationMessage,
                    NotificationType = (int)NotificationType.SessionNotifiction,
                    IsRead = false
                }).ToList();
                await ApproveNotification(list, notificationMessage);
            }

            var displayMsg = "Notification has been" + (acceptType == "A" ? " accepted " : " rejected ") + " successfully.";
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                Message = displayMsg
            });
        }

        public async Task<IActionResult> SendNotiToDecision(int sessionId, string start, string end)
        {
            var sessionParticipant = service.GetSessionDecisionMaker(sessionId);

            string notificationMessage = $" Coordinator approve agenda and send you {sessionId}";

            var list = sessionParticipant.Select(x => new NotificationReminderDto()
            {
                SendTo = x.UserId.Value,
                SendDate = DateTime.Now,
                Reason = x.SessionId.ToString(),
                Message = notificationMessage,
                NotificationType = (int)NotificationType.Reminder,
                IsRead = false
            }).ToList();

            string[] userIds = new string[0];
            if (list != null && list.Count > 0)
            {

                userIds = list.Select(x => x.SendTo.ToString()).ToArray();

                if (userIds != null && userIds.Count() > 0)
                {
                    var connections = _userConnectionManager.GetUserAllConnections(userIds);
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", notificationMessage, notificationMessage);
                    await notificationService.AddReminderNotification(list);
                }
            }


            var displayMsg = "Notification has been sent successfully.";
            return NewtonSoftJsonResult(new
            {
                message = displayMsg
            });
        }

        public async Task<IActionResult> StartChat(int sessionId, DateTime start, DateTime end)
        {
            var sessionParticipant = service.GetAllDecisionParticipant(sessionId);

            await preSessionGroupService.AddPreSessionGroupAsync(new PreSessionGroupDto() { EndDate = end, SessionId = sessionId, StartDate = start });

            foreach (var item in sessionParticipant)
            {
                var preSessionGroup = preSessionGroupService.GetPresessionProupId(item.UserId.Value);
                if (item.UserId > 0 && preSessionGroup.Any())
                {
                    var connections = _userConnectionManager.GetUserConnections(item.UserId.ToString());
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToJoinGroup", string.Join(",", preSessionGroup.Select(x => x.Id)),
                        string.Join(",", preSessionGroup.Select(x => x.Name)), item.ParticipantTyprId == (int)ParticipantType.Coordinator);
                }
            }
            var displayMsg = "Group Chat will be start in few seconds.";
            return NewtonSoftJsonResult(new
            {
                message = displayMsg
            });
        }

        [HttpPost]
        public async Task<IActionResult> EndChat(int sessionGroupId, string htmlContent)
        {
            var session = preSessionGroupService.GetByIdWithGame(sessionGroupId);
            var result = generatePdf.GetPDF(htmlContent);
            dropBoxService.CreateFolder($"/{CurrentUser.CompanyId}/Game_{session.GameId}/{session.SessionId}");
            dropBoxService.Upload($"/{CurrentUser.CompanyId}/Game_{session.GameId}/{session.SessionId}", $"{session.GroupName.Replace("/", "~")}.pdf", result);
            return Json(true);
        }

        public IActionResult GetChatHistory(int sessionGroupId)
        {
            var chatList = preSessionGroupService.GetChatList(sessionGroupId);
            ViewBag.chatHistory = chatList;
            return PartialView("~/Views/Shared/_GroupChat.cshtml");

        }

        public IActionResult GetGroupChat(int id)
        {
            var allMessage = preSessionGroupService.GetChatList(id);
            return Json(allMessage);
        }



        [NonAction]
        private async Task<bool> ApproveNotification(List<NotificationReminderDto> notifyModel, string notificationMessage)
        {
            string[] userIds = new string[0];
            if (notifyModel != null && notifyModel.Count > 0)
            {


                userIds = notifyModel.Select(x => x.SendTo.ToString()).ToArray();

                if (userIds != null && userIds.Count() > 0)
                {
                    var connections = _userConnectionManager.GetUserAllConnections(userIds);
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sessionNotification", "Confirmation", notificationMessage);
                    await notificationService.AddReminderNotification(notifyModel);
                }
            }

            return true;
        }
    }
}
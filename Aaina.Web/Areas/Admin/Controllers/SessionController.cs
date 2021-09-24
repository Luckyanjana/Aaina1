using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;

using Aaina.Service;
using Aaina.Web.Code;
using Aaina.Web.Models;
using Aaina.Web.Models.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class SessionController : BaseController
    {

        private readonly ISessionService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly INotificationService _notificationService;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IHostingEnvironment env;
        public SessionController(ISessionService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService, INotificationService notificationService,
            IUserConnectionManager userConnectionManager,
            IHubContext<NotificationUserHub> notificationUserHubContext, IHostingEnvironment env)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
            _notificationService = notificationService;
            _userConnectionManager = userConnectionManager;
            _notificationUserHubContext = notificationUserHubContext;
            this.env = env;
        }
        public IActionResult Index()
        {
            var AllRecord = service.GetAllByCompanyIdAsync(CurrentUser.CompanyId);
            return View(AllRecord);
        }

        public async Task<IActionResult> AddEdit(int? id, int? parentId, int? copyId)
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
            if (!parentId.HasValue)
            {
                var game = gameService.GetFirstGame(CurrentUser.CompanyId);
                parentId = game.Id;
                gameName = game.Name;
            }
            else
            {
                var game = await gameService.GetDetailsId(parentId.Value);
                gameName = game.Name;
            }

            model.GameId = parentId;
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
            if (model.SessionScheduler != null && !string.IsNullOrEmpty(model.SessionScheduler.MeetingUrl))
            {
                model.SessionScheduler.MeetingUrl = model.SessionScheduler.MeetingUrl;
            }
            model.PlayerList = (await gameService.GetGamlePlayer(CurrentUser.CompanyId)).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.UserId.ToString(), Additional = s.UserTypeId.ToString() }).ToList();
            model.GameList =
                gameService.GetAllDropSecondParent(CurrentUser.CompanyId).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.Id.ToString() }).ToList();
            model.AllRecord = model.GameId.HasValue ? service.GetAllByGameId(CurrentUser.CompanyId, model.GameId.Value) : new List<SessionDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }

        [NonAction]
        private async Task<bool> UserNotification(SessionDto model)
        {
            string[] userIds = new string[0];
            if (model.SessionParticipant != null && model.SessionParticipant.Count > 0)
            {
                string notificationMessage = $"New Session Request Notification {model.Name} {model.SessionId}";
               // userIds = model.SessionParticipant.Where(x => x.ParticipantTyprId == (int)ParticipantType.DecisionMaker).Select(y => y.UserId.Value.ToString()).ToArray();
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

                    await _notificationService.AddReminderNotification(notifyModel);
                }
            }

            return true;
        }



        [HttpPost]
        public async Task<IActionResult> AddEdit(SessionDto model)
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

            if (!model.SessionParticipant.Any(a => a.ParticipantTyprId == (int)ParticipantType.Coordinator))
            {
                ModelState.AddModelError("", $"Please select at least one coordinator");
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

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                model.CreatedBy = CurrentUser.UserId;
                var sessionId = await service.AddUpdateAsync(model);
                model.SessionId = sessionId;
                await UserNotification(model); //Save & Send Notification
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("AddEdit", new { id = "", parentId = model.GameId }), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
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
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteById(id);
                ShowSuccessMessage("Success!", $"Look has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShareSessionExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var sessionModel = service.GetAllByCompanyIdAsync(id);
                string FileName = string.Empty;
                var newFile = ShareSessionExportExcel(sessionModel, out FileName);
                // attachments.Add(MimeEntity.Load(newFile.FullName));
                byte[] myFileAsByteArray = Common.Common.FileToByteArray(newFile.FullName);
                attachments.Add(newFile.FullName, myFileAsByteArray);

                Common.Common.SendMailWithAttachment(allusersEmails, "Share", html, attachments);
                ShowSuccessMessage("Success!", $"Share successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = "", Message = "Share successfully.", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }

        private FileInfo ShareSessionExportExcel(List<SessionDto> sessionModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Session List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Session List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Session List";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;
                worksheet.Cells[rowNo, column].Value = "Session Name";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 2;
                worksheet.Cells[rowNo, column].Value = "Game Name";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Session Type";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 4;
                worksheet.Cells[rowNo, column].Value = "Mode";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 5;
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 6;
                worksheet.Cells[rowNo, column].Value = "Start";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 7;
                worksheet.Cells[rowNo, column].Value = "End";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 8;
                worksheet.Cells[rowNo, column].Value = "Created By";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 9;
                worksheet.Cells[rowNo, column].Value = "Created Date";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                rowNo++;

                foreach (var row in sessionModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.Game;

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = Convert.ToString((SessionType)row.TypeId);

                    column = 4;
                    worksheet.Cells[rowNo, column].Value = Convert.ToString((SessionType)row.ModeId);

                    column = 5;
                    worksheet.Cells[rowNo, column].Value = row.IsActive ? "Active" : "InActive";

                    column = 6;
                    worksheet.Cells[rowNo, column].Value = row.SessionScheduler.StartDate.HasValue ? row.SessionScheduler.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty;

                    column = 7;
                    worksheet.Cells[rowNo, column].Value = row.SessionScheduler.EndDate.HasValue ? row.SessionScheduler.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty;

                    column = 8;
                    worksheet.Cells[rowNo, column].Value = row.CreatedByStr;

                    column = 9;
                    worksheet.Cells[rowNo, column].Value = row.CreatedDate.Value.ToString("dd/MM/yyyy");

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
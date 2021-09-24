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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class StatusController : BaseController
    {
        private readonly IStatusService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IHostingEnvironment env;

        public StatusController(IStatusService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
           IAttributeService attributeService, ITeamService teamService, IHostingEnvironment env)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.teamService = teamService;
            this.env = env;
        }
        public IActionResult Index()
        {
            var AllRecord = service.GetAllByCompanyId(CurrentUser.CompanyId, null,null);
            return View(AllRecord);
        }

        public async Task<IActionResult> AddEdit(int? id, int? parentId, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }
            StatusDto model = new StatusDto()
            {
                StatusScheduler = new StatusSchedulerDto()
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

            model.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId);

            model.TeamList = teamService.GetAllDrop(null, CurrentUser.CompanyId);

            model.UserList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            {
                Name = $"{c.Fname} {c.Lname}",
                Id = c.Id.ToString()
            }).ToList();

            model.ModeList = Enum.GetValues(typeof(StatusModeType)).Cast<StatusModeType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.StatusScheduler != null && !string.IsNullOrEmpty(model.StatusScheduler.DaysOfWeek))
            {
                model.StatusScheduler.DaysOfWeekList = model.StatusScheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await gameService.GetGamlePlayer(CurrentUser.CompanyId)).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.UserId.ToString(), Additional = s.UserTypeId.ToString() }).ToList();
            model.GameList =
                gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(s => new SelectedItemDto() { Name = s.Name, Id = s.Id }).ToList();
            model.AllRecord = model.GameId.HasValue ? service.GetAllByGameId(CurrentUser.CompanyId, model.GameId.Value, null) : new List<StatusDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(StatusDto model)
        {
            model.CompanyId = CurrentUser.CompanyId;
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.StatusScheduler == null || !model.StatusScheduler.StartDate.HasValue)
            {
                ModelState.AddModelError("", $"Status Scheduler Required!");
                return CreateModelStateErrors();
            }

            model.StatusReminder = model.StatusReminder.Where(x => x.Every.HasValue && x.TypeId.HasValue && x.Unit.HasValue).ToList();

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.StatusScheduler.Type == (int)ScheduleType.Recurring && !model.StatusScheduler.RecurseEvery.HasValue)
            {
                ModelState.AddModelError("", $"Recurse Every is required");
                return CreateModelStateErrors();
            }

            if (model.StatusScheduler.DailyFrequency == (int)ScheduleDailyFrequency.Every)
            {
                bool isValid = true;

                if (!model.StatusScheduler.OccursEveryValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Value is required");
                }

                if (!model.StatusScheduler.OccursEveryTimeUnit.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Time Unit is required");
                }

                if (!isValid)
                {
                    return CreateModelStateErrors();
                }
            }



            if (model.StatusScheduler != null && model.StatusScheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.StatusScheduler.DaysOfWeek = string.Join(",", model.StatusScheduler.DaysOfWeekList);
            }

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;

                await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index", new { id = "", parentId = model.GameId }), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Status?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Status" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteById(id);
                ShowSuccessMessage("Success!", $"Status has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShareStatusExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var statusModel = service.GetAllByCompanyId(id, null,null);
                string FileName = string.Empty;
                var newFile = ShareStatusExportExcel(statusModel, out FileName);
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

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var details = await service.GetFeedbackDetailsById(id);
            ViewBag.By = details.By;
            ViewBag.For = details.For;
            ViewBag.name = details.Name;
            List<StatusFeedbackDisplayDto> status = await service.ViewResult(id, null);
            ViewBag.ParticipantList = await service.GetParticipentByStatusId(id);
            return PartialView("_result", status);
        }

        [HttpGet]
        public async Task<IActionResult> ViewResult(int id, int userId)
        {
            List<StatusFeedbackDisplayDto> status = await service.ViewResult(id, null);
            return Json(status);
        }

        private FileInfo ShareStatusExportExcel(List<StatusDto> statusModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Status List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Status List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Status List";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;
                worksheet.Cells[rowNo, column].Value = "Name";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 2;
                worksheet.Cells[rowNo, column].Value = "Status Mode";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                rowNo++;

                foreach (var row in statusModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = Convert.ToString((StatusModeType)row.StatusModeId);

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.IsActive ? "Active" : "InActive";

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
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
    public class ReportController : BaseController
    {
        private readonly IReportService service;
        private readonly IGameService _gameService;
        private readonly IFormBuilderService formBuilderService;
        private readonly IHostingEnvironment env;
        public ReportController(IReportService reportService, IGameService gameService, IFormBuilderService formBuilderService, IHostingEnvironment env)
        {
            service = reportService;
            _gameService = gameService;
            this.formBuilderService = formBuilderService;
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
            ReportDto model = new ReportDto()
            {
                Scheduler = new ReportSchedulerDto()
                {
                    Type = (int)ScheduleType.OneTime
                }
            };
            string gameName = string.Empty;
            if (!parentId.HasValue)
            {
                var game = _gameService.GetFirstGame(CurrentUser.CompanyId);
                parentId = game.Id;
                gameName = game.Name;
            }
            else
            {
                var game = await _gameService.GetDetailsId(parentId.Value);
                gameName = game.Name;
            }

            model.GameId = parentId;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            //model.gamen = gameName;
            model.TypeList = Enum.GetValues(typeof(SessionType)).Cast<SessionType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            //model.ModeList = Enum.GetValues(typeof(SessionMode)).Cast<SessionMode>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.AccountAbilityList = Enum.GetValues(typeof(AccountAbilityType)).Cast<AccountAbilityType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.Scheduler != null && !string.IsNullOrEmpty(model.Scheduler.DaysOfWeek))
            {
                model.Scheduler.DaysOfWeekList = model.Scheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await _gameService.GetGamlePlayer(CurrentUser.CompanyId));
            model.GameList =
                _gameService.GetAllByParentId(parentId.HasValue ? parentId.Value : (int?)null, CurrentUser.CompanyId);
            model.FormBuilderList = formBuilderService.GetAll(CurrentUser.CompanyId).Select(x => new SelectedItemDto()
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();
            model.AllRecord = model.GameId.HasValue ? service.GetAllByGameId(CurrentUser.CompanyId, model.GameId.Value) : new List<ReportDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddEdit(ReportDto model)
        {
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.Scheduler == null || !model.Scheduler.StartDate.HasValue)
            {
                ModelState.AddModelError("", $"Report Scheduler Required!");
                return CreateModelStateErrors();
            }

            model.Reminder = model.Reminder.Where(x => x.Every.HasValue && x.TypeId.HasValue && x.Unit.HasValue).ToList();

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.Scheduler.Type == (int)ScheduleType.Recurring && !model.Scheduler.RecurseEvery.HasValue)
            {
                ModelState.AddModelError("", $"Recurse Every is required");
                return CreateModelStateErrors();
            }

            if (model.Scheduler.DailyFrequency == (int)ScheduleDailyFrequency.Every)
            {
                bool isValid = true;

                if (!model.Scheduler.OccursEveryValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Value is required");
                }

                if (!model.Scheduler.OccursEveryTimeUnit.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Time Unit is required");
                }

                if (!isValid)
                {
                    return CreateModelStateErrors();
                }
            }


            if (model.Scheduler != null && model.Scheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.Scheduler.DaysOfWeek = string.Join(",", model.Scheduler.DaysOfWeekList);
            }

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                //model.CreatedBy = CurrentUser.UserId;
                var sessionId = await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }

        [HttpGet]
        public async Task<IActionResult> ResultAsync(int id)
        {
            var list = await this.service.GetGiveListByReportId(id);
            return View(list);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Report?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Report" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Report has been delete  successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShareReportExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var reportModel = service.GetAllByCompanyId(id, null,null);
                string FileName = string.Empty;
                var newFile = ShareReportExportExcel(reportModel, out FileName);
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

        private FileInfo ShareReportExportExcel(List<ReportDto> reportModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Report List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Report List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Report List";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;
                worksheet.Cells[rowNo, column].Value = "Report Name";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 2;
                worksheet.Cells[rowNo, column].Value = "Report Type";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                rowNo++;

                foreach (var row in reportModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = Convert.ToString((SessionType)row.TypeId);

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}

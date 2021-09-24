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
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportService service;
        private readonly IGameService _gameService;
        private readonly IFormBuilderService formBuilderService;
        private readonly IHostingEnvironment env;
        private readonly IUserLoginService userLogin;
        private readonly ITeamService teamService;
        private readonly IWeightageService weightageService;
        public ReportController(IReportService reportService, IGameService gameService, IFormBuilderService formBuilderService, IHostingEnvironment env, IUserLoginService userLogin,
            ITeamService teamService, IWeightageService weightageService)
        {
            service = reportService;
            _gameService = gameService;
            this.formBuilderService = formBuilderService;
            this.env = env;
            this.userLogin = userLogin;
            this.teamService = teamService;
            this.weightageService = weightageService;
            FileOutputUtil.OutputDir = new DirectoryInfo(env.WebRootPath + "/TempExcel");
        }
        public IActionResult Index(int tenant)
        {
            ViewBag.TypeList = Enum.GetValues(typeof(ReportType)).Cast<ReportType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            return View();
        }
        public async Task<IActionResult> Give(int tenant, int id)
        {
            var giveRecord = await service.GetGiveByReportId(id);
            giveRecord.UserList = await service.GetEntityByReportId(id);
            return View(giveRecord);
        }

        public async Task<IActionResult> GiveUpdate(int tenant, int reportId)
        {
            var giveRecord = await service.GetGiveUpdateByReportId(reportId);
            return View(giveRecord);
        }

        [HttpPost]
        public async Task<IActionResult> Give(int tenant, ReportGiveSaveDto model)
        {
            model.UserId = CurrentUser.UserId;
            var giveRecord = await service.SaveReortGive(model);
            ShowSuccessMessage("Success!", $"Report Give has been added successfully.", false);
            return Redirect($"/{tenant}/report/index");
        }

        [HttpPost]
        public async Task<IActionResult> GiveUpdate(int tenant, ReportGiveSaveDto model)
        {
            model.UserId = CurrentUser.UserId;
            await service.UpdateReortGive(model);
            ShowSuccessMessage("Success!", $"Report Give has been updated successfully.", false);
            return Redirect($"/{tenant}/report/index");
        }
        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ReportDto model, int tenant)
        {
            return await AddEdit(model, tenant);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int tenant, int? copyId)
        {
            return await AddEdit(id, tenant, copyId);
        }

        private async Task<IActionResult> AddEdit(int? id, int tenant, int? copyId)
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

            var game = await _gameService.GetDetailsId(tenant);
            gameName = game.Name;


            model.GameId = tenant;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);


                if (model.EntityType == (int)EmotionsFor.Game)
                {
                    model.EntityIdList = _gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = c.Name,
                        Value = int.Parse(c.Id)
                    }).ToList();

                }
                else if (model.EntityType == (int)EmotionsFor.Team)
                {
                    model.EntityIdList = teamService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = c.Name,
                        Value = int.Parse(c.Id)
                    }).ToList();
                }
                else
                {
                    model.EntityIdList = userLogin.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = $"{c.Fname} {c.Lname}",
                        Value = c.Id
                    }).ToList();
                }
            }

            model.TypeList = Enum.GetValues(typeof(ReportType)).Cast<ReportType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.AccountAbilityList = Enum.GetValues(typeof(AccountAbilityType)).Cast<AccountAbilityType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.EntityTypeList = Enum.GetValues(typeof(EmotionsFor)).Cast<EmotionsFor>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();

            if (model.Scheduler != null && !string.IsNullOrEmpty(model.Scheduler.DaysOfWeek))
            {
                model.Scheduler.DaysOfWeekList = model.Scheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await _gameService.GetGamlePlayer(CurrentUser.CompanyId));
            model.GameList =
                _gameService.GetAllByParentId(tenant, CurrentUser.CompanyId);
            model.FormBuilderList = formBuilderService.GetAll(CurrentUser.CompanyId).Select(x => new SelectedItemDto()
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();
            model.AllRecord = service.GetAllByCompanyId(CurrentUser.CompanyId, CurrentUser.UserId, model.GameId.Value);
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }

        private async Task<IActionResult> AddEdit(ReportDto model, int tenant)
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
                model.CreatedBy = CurrentUser.UserId;
                var sessionId = await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Report/Index", IsSuccess = true });

        }

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var giveRecord = await service.GetGiveUpdateByReportId(id);
            return View(giveRecord);
        }

        [HttpGet]
        public async Task<IActionResult> ResultRange(int id, DateTime? start, DateTime? end)
        {
            ViewBag.id = id;
            ViewBag.start = start;
            ViewBag.end = end;
            var giveRecord = start.HasValue && end.HasValue ? await service.GetGiveUpdateByReportRange(id, start.Value, end.Value) : new List<ReportGiveSaveDto>();
            return View(giveRecord);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var giveRecord = await service.GetGiveUpdateByReportId(id);
            return View(giveRecord);
        }


        public async Task<IActionResult> Export(int id, DateTime start, DateTime end)
        {
            var model = await service.GetGiveUpdateByReportId(id);
            var emojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Report Give";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;

                if (model.Details.Any(a => a.EntityId.HasValue))
                {
                    worksheet.Cells[rowNo, column].Value = " User   Name";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                foreach (var item in model.FormBuilderAttribute)
                {
                    worksheet.Cells[rowNo, column].Value = item.AttributeName;
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                worksheet.Cells[rowNo, column].Value = "Remark";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column++;

                rowNo++;

                string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
                column = 1;
                foreach (var user in model.Details)
                {
                    column = 1;
                    if (user.EntityId.HasValue)
                    {
                        worksheet.Cells[rowNo, column].Value = user.Name;
                        column++;
                    }
                    foreach (var item in model.FormBuilderAttribute)
                    {
                        FormBuilderAttributeValueDto attr = user.Attribute.Any(x => x.FormBuilderAttributeId == item.Id) ? user.Attribute.FirstOrDefault(x => x.FormBuilderAttributeId == item.Id) : new FormBuilderAttributeValueDto();



                        if (item.DataType == (int)Aaina.Common.OptionType.Dropdown || item.DataType == (int)Aaina.Common.OptionType.RadioButton)
                        {

                            worksheet.Cells[rowNo, column].Value = attr.LookUp;
                            column++;
                        }
                        else
                        if (item.DataType == (int)Aaina.Common.OptionType.Emotion)
                        {
                            var path = @$"{folder}{this.GetEmojiNameMini(emojiList, double.Parse(attr.AttributeValue))}";
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(path);
                            var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.From.Row = rowNo - 1;
                            pic.From.Column = column - 1;
                            //pic.To.Row = rowNo;
                            //pic.To.Column = column;
                            pic.SetSize(100);

                            //worksheet.Cells[rowNo, column].Value = attr.LookUp;
                            column++;
                        }
                        else
                        {
                            worksheet.Cells[rowNo, column].Value = attr.AttributeValue;
                            column++;
                        }
                    }

                    worksheet.Cells[rowNo, column].Value = user.Remark;
                    column++;
                    rowNo++;
                }

                rowNo++;
                column = 1;
                worksheet.Cells[rowNo, column, rowNo, (column + model.FormBuilderAttribute.Count)].Value = model.Remark;

                pack.Save();
            }

            string FileName = $"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }
        public async Task<IActionResult> ExportWithoutRemark(int id, DateTime start, DateTime end)
        {
            var model = await service.GetGiveUpdateByReportId(id);
            var emojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Report Give";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;

                if (model.Details.Any(a => a.EntityId.HasValue))
                {
                    worksheet.Cells[rowNo, column].Value = " User   Name";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                foreach (var item in model.FormBuilderAttribute)
                {
                    worksheet.Cells[rowNo, column].Value = item.AttributeName;
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }



                rowNo++;

                string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
                column = 1;
                foreach (var user in model.Details)
                {
                    column = 1;

                    if (user.EntityId.HasValue)
                    {
                        worksheet.Cells[rowNo, column].Value = user.Name;
                        column++;
                    }
                    foreach (var item in model.FormBuilderAttribute)
                    {
                        FormBuilderAttributeValueDto attr = user.Attribute.Any(x => x.FormBuilderAttributeId == item.Id) ? user.Attribute.FirstOrDefault(x => x.FormBuilderAttributeId == item.Id) : new FormBuilderAttributeValueDto();



                        if (item.DataType == (int)Aaina.Common.OptionType.Dropdown || item.DataType == (int)Aaina.Common.OptionType.RadioButton)
                        {

                            worksheet.Cells[rowNo, column].Value = attr.LookUp;
                            column++;
                        }
                        else
                        if (item.DataType == (int)Aaina.Common.OptionType.Emotion)
                        {
                            var path = @$"{folder}{this.GetEmojiNameMini(emojiList, double.Parse(attr.AttributeValue))}";
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(path);
                            var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.From.Row = rowNo - 1;
                            pic.From.Column = column - 1;
                            //pic.To.Row = rowNo;
                            //pic.To.Column = column;
                            pic.SetSize(100);

                            //worksheet.Cells[rowNo, column].Value = attr.LookUp;
                            column++;
                        }
                        else
                        {
                            worksheet.Cells[rowNo, column].Value = attr.AttributeValue;
                            column++;
                        }
                    }
                    rowNo++;
                }

                pack.Save();
            }

            string FileName = $"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }

        public async Task<IActionResult> ExportRange(int id, DateTime start, DateTime end)
        {
            var models = await service.GetGiveUpdateByReportRange(id, start, end);
            var emojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Report Give";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                ReportGiveSaveDto firstRecord = models.FirstOrDefault();


                int column = 1;


                worksheet.Cells[rowNo, column].Value = " Give Date";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column++;


                if (firstRecord.Details.Any(a => a.EntityId.HasValue))
                {
                    worksheet.Cells[rowNo, column].Value = " User   Name";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                foreach (var item in firstRecord.FormBuilderAttribute)
                {
                    worksheet.Cells[rowNo, column].Value = item.AttributeName;
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                worksheet.Cells[rowNo, column].Value = "Remark";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column++;

                rowNo++;

                string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
                foreach (var model in models)
                {
                    column = 1;
                    foreach (var user in model.Details)
                    {
                        column = 1;

                        worksheet.Cells[rowNo, column].Value = model.GiveDate.ToString("dd/MM/yyyy");
                        column++;

                        if (user.EntityId.HasValue)
                        {
                            worksheet.Cells[rowNo, column].Value = user.Name;
                            column++;
                        }

                        foreach (var item in model.FormBuilderAttribute)
                        {
                            FormBuilderAttributeValueDto attr = user.Attribute.Any(x => x.FormBuilderAttributeId == item.Id) ? user.Attribute.FirstOrDefault(x => x.FormBuilderAttributeId == item.Id) : new FormBuilderAttributeValueDto();



                            if (item.DataType == (int)Aaina.Common.OptionType.Dropdown || item.DataType == (int)Aaina.Common.OptionType.RadioButton)
                            {

                                worksheet.Cells[rowNo, column].Value = attr.LookUp;
                                column++;
                            }
                            else
                            if (item.DataType == (int)Aaina.Common.OptionType.Emotion)
                            {
                                var path = @$"{folder}{this.GetEmojiNameMini(emojiList, double.Parse(attr.AttributeValue))}";
                                System.Drawing.Image myImage = System.Drawing.Image.FromFile(path);
                                var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                                pic.From.Row = rowNo - 1;
                                pic.From.Column = column - 1;
                                //pic.To.Row = rowNo;
                                //pic.To.Column = column;
                                pic.SetSize(100);

                                //worksheet.Cells[rowNo, column].Value = attr.LookUp;
                                column++;
                            }
                            else
                            {
                                worksheet.Cells[rowNo, column].Value = attr.AttributeValue;
                                column++;
                            }
                        }

                        worksheet.Cells[rowNo, column].Value = user.Remark;
                        column++;
                        rowNo++;
                    }


                }

                rowNo++;
                column = 1;
                worksheet.Cells[rowNo, column, rowNo, (column + firstRecord.FormBuilderAttribute.Count)].Value = firstRecord.Remark;



                pack.Save();
            }

            string FileName = $"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }
        public async Task<IActionResult> ExportRangeWithoutRemark(int id, DateTime start, DateTime end)
        {
            var models = await service.GetGiveUpdateByReportRange(id, start, end);
            var emojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Report Give";
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;

                ReportGiveSaveDto firstRecord = models.FirstOrDefault();


                int column = 1;

                worksheet.Cells[rowNo, column].Value = "Give Date";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column++;

                if (firstRecord.Details.Any(a => a.EntityId.HasValue))
                {
                    worksheet.Cells[rowNo, column].Value = " User   Name";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                foreach (var item in firstRecord.FormBuilderAttribute)
                {
                    worksheet.Cells[rowNo, column].Value = item.AttributeName;
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column++;
                }

                rowNo++;

                string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";

                foreach (var model in models)
                {
                    column = 1;
                    foreach (var user in model.Details)
                    {
                        column = 1;

                        worksheet.Cells[rowNo, column].Value = model.GiveDate.ToString("dd/MM/yyyy");
                        column++;

                        if (user.EntityId.HasValue)
                        {
                            worksheet.Cells[rowNo, column].Value = user.Name;
                            column++;
                        }
                        foreach (var item in model.FormBuilderAttribute)
                        {
                            FormBuilderAttributeValueDto attr = user.Attribute.Any(x => x.FormBuilderAttributeId == item.Id) ? user.Attribute.FirstOrDefault(x => x.FormBuilderAttributeId == item.Id) : new FormBuilderAttributeValueDto();



                            if (item.DataType == (int)Aaina.Common.OptionType.Dropdown || item.DataType == (int)Aaina.Common.OptionType.RadioButton)
                            {

                                worksheet.Cells[rowNo, column].Value = attr.LookUp;
                                column++;
                            }
                            else
                            if (item.DataType == (int)Aaina.Common.OptionType.Emotion)
                            {
                                var path = @$"{folder}{this.GetEmojiNameMini(emojiList, double.Parse(attr.AttributeValue))}";
                                System.Drawing.Image myImage = System.Drawing.Image.FromFile(path);
                                var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                                pic.From.Row = rowNo - 1;
                                pic.From.Column = column - 1;
                                //pic.To.Row = rowNo;
                                //pic.To.Column = column;
                                pic.SetSize(100);

                                //worksheet.Cells[rowNo, column].Value = attr.LookUp;
                                column++;
                            }
                            else
                            {
                                worksheet.Cells[rowNo, column].Value = attr.AttributeValue;
                                column++;
                            }
                        }
                        rowNo++;
                    }
                }




                pack.Save();
            }

            string FileName = $"Report_Give-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }

        [HttpGet]
        public IActionResult Delete(int id, int tenant)
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
        public async Task<IActionResult> Delete(int id, int tenant, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Report has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Report/Index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
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
                var reportModel = service.GetAllByCompanyId(id, null, null);
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
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

namespace Aaina.Web.Controllers
{
    public class LookController : BaseController
    {
        private readonly ILookService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LookController(ILookService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService, IHostingEnvironment hostingEnvironment)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
            this._hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(int tenant)
        {
            AddPageHeader("Look", "List");
            ViewBag.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            return View();
        }

        public IActionResult All(int tenant)
        {
            var all = service.GetAll(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            return PartialView("_all", all);
        }

        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LookDto model, int tenant)
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
            AddPageHeader("Look", "Add/Edit");
            LookDto model = new LookDto()
            {
                LookScheduler = new LookSchedulerDto()
                {
                    Type = (int)ScheduleType.OneTime
                }
            };
            string gameName = string.Empty;

            var game = await gameService.GetDetailsId(tenant);
            gameName = game.Name;


            model.GameId = tenant;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            model.Game = gameName;
            model.TypeList = Enum.GetValues(typeof(LookType)).Cast<LookType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.CalculatiotTypeList = Enum.GetValues(typeof(LookCalculation)).Cast<LookCalculation>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.IsSchedule && !string.IsNullOrEmpty(model.LookScheduler.DaysOfWeek))
            {
                model.LookScheduler.DaysOfWeekList = model.LookScheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.AttributeList = attributeService.GetAllWithSub(CurrentUser.CompanyId);
            model.GameList = gameService.GetAllByParentId(tenant, CurrentUser.CompanyId);
            model.AllRecord = service.GetAllByGame(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            model.PlayerList = await gameService.GetGamlePlayer(CurrentUser.CompanyId);
            model.GroupPlayerList = userService.GetAllByDropByCompanyId(CurrentUser.CompanyId);
            model.TeamList = teamService.GetAllDrop(null, CurrentUser.CompanyId);
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }


        private async Task<IActionResult> AddEdit(LookDto model, int tenant)
        {
            model.CompanyId = CurrentUser.CompanyId;
            model.CreatedBy = CurrentUser.UserId;

            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }


            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            model.LookAttribute = model.LookAttribute.Where(x => x.IsAdded).ToList();
            model.LookSubAttribute = model.LookSubAttribute.Where(x => x.IsAdded).ToList();
            model.LookGame = model.LookGame.Where(x => x.IsAdded).ToList();
            model.LookPlayers = model.LookPlayers.Where(x => x.IsAdded).ToList();
            model.LookTeam = model.LookTeam.Where(x => x.IsAdded).ToList();
            model.LookUser = model.LookUser.Where(x => x.IsAdded).ToList();
            List<LookGroupDto> lookGroup = new List<LookGroupDto>();
            foreach (var item in model.LookGroup)
            {
                if (item.LookGroupPlayer.Any(a => a.IsAdded))
                    lookGroup.Add(new LookGroupDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        LookGroupPlayer = item.LookGroupPlayer.Where(w => w.IsAdded).ToList()
                    });
            }

            model.LookGroup = lookGroup;

            if (model.IsSchedule && model.LookScheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.LookScheduler.DaysOfWeek = string.Join(",", model.LookScheduler.DaysOfWeekList);
            }

            if (model.IsSchedule && model.LookScheduler.DailyFrequency == (int)ScheduleDailyFrequency.Every)
            {
                bool isValid = true;
                if (!model.LookScheduler.RecurseEvery.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Recurse Every is required");
                }

                if (!model.LookScheduler.OccursEveryValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Value is required");
                }

                if (!model.LookScheduler.OccursEveryTimeUnit.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Time Unit is required");
                }

                if (!isValid)
                {
                    return CreateModelStateErrors();
                }
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
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/look/index", IsSuccess = true });

        }



        [HttpGet]
        public IActionResult Delete(int id, int tenant)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Team?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Team" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int tenant, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Look has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/look/index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ShareLookExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var lookModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareLookExportExcel(lookModel, out FileName);
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

        private FileInfo ShareLookExportExcel(List<LookDto> lookModel, out string FileName)
        {
            string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Look List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Look List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Look List";
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
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Frequency";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 4;
                worksheet.Cells[rowNo, column].Value = "Created By";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 5;
                worksheet.Cells[rowNo, column].Value = "Created On";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                rowNo++;


                foreach (var row in lookModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.IsActive == true ? "Active" : "InActive";

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.IsSchedule ? (row.LookScheduler.Type == (int)ScheduleType.Recurring ? ((ScheduleFrequency)row.LookScheduler.Frequency).ToString() : "One time") : "";

                    column = 4;
                    worksheet.Cells[rowNo, column].Value = CurrentUser.Name;

                    column = 5;
                    worksheet.Cells[rowNo, column].Value = row.CreatedDate.HasValue ? row.CreatedDate.Value.ToString("dd/MM/yyyy") : "";


                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
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
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Wkhtmltopdf.NetCore;

namespace Aaina.Web.Controllers
{
    public class FeedBackController : BaseController
    {
        private readonly IGameService service;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly ILookService lookService;
        private readonly IWeightagePresetService weightagePresetService;
        private readonly IFilterService filterService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IGeneratePdf generatePdf;
        private readonly IAttributeService attributeService;
        private readonly IWeightageService weightageService;
        public FeedBackController(IGameService service, IRoleService roleService, IUserLoginService userService, ILookService lookService, IWeightagePresetService weightagePresetService, IFilterService filterService, IHostingEnvironment hostingEnvironment, IGeneratePdf generatePdf, IAttributeService attributeService, IWeightageService weightageService)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.weightagePresetService = weightagePresetService;
            this.lookService = lookService;
            this.filterService = filterService;
            this._hostingEnvironment = hostingEnvironment;
            this.generatePdf = generatePdf;
            this.attributeService = attributeService;
            this.weightageService = weightageService;
            FileOutputUtil.OutputDir = new DirectoryInfo(hostingEnvironment.WebRootPath + "/TempExcel");
        }

        public async Task<IActionResult> View(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterFromDate, DateTime? filterToDate)
        {


            filterFromDate = filterFromDate.HasValue ? filterFromDate.Value : DateTime.Now;
            filterToDate = filterToDate.HasValue ? filterToDate.Value : DateTime.Now;

            GameGridFeedbackDto model = new GameGridFeedbackDto()
            {
                GameId = tenant,
                PresetId = presetId,
                FilterId = filterId,
                AttributeId = atterbuteId,
                FilterFromDate = filterFromDate,
                FilterToDate = filterToDate
            };
            model.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            List<int> filterForIds = new List<int>();
            if (filterId.HasValue)
            {
                var filterResponse = filterService.EmotionsForId(filterId.Value);
                filterForIds = filterResponse.Item1;
                model.IsSelf = filterResponse.Item2;
            }
            var deails = await service.GetDetailsId(tenant);
            model.GameName = deails.Name;

            var lookList = lookService.GetAllDrop(tenant, CurrentUser.CompanyId);
            //if (!lookId.HasValue && lookList.Any())
            //{
            //    lookId = int.Parse(lookList.FirstOrDefault().Id);

            //}
            model.LookList = lookList;
            int typeId = 0;
            if (lookId.HasValue || filterId.HasValue)
            {

                model.LookId = lookId;


                if (filterId.HasValue)
                {
                    model.AttributeList = lookService.GetAttributeDropByfilter(filterId.Value);
                    typeId = (await filterService.GetById(filterId.Value)).EmotionsFor.Value;
                }
                else
                if (lookId.HasValue)
                {
                    model.AttributeList = lookService.GetAttributeDrop(model.LookId.Value);
                    typeId = (await lookService.GetDetailsId(lookId.Value)).TypeId;
                }

                if (atterbuteId.HasValue && model.AttributeList.Any(a => int.Parse(a.Id) == atterbuteId))
                {
                    model.AttributeId = atterbuteId;
                }
                // var look = await lookService.GetDetailsId(lookId.Value);

                if (typeId == (int)LookType.Game)
                {
                    model.AllGames = service.GetAllByParentId(tenant, CurrentUser.CompanyId, filterForIds);
                    var feedback = await lookService.GetGameFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate.Value, filterToDate.Value);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterFromDate;
                    model.FilterToDate = filterToDate;
                    model.AttributeNames = feedback.Item2;
                    model.FilterName = feedback.Item3;
                }

                else if (typeId == (int)LookType.Team)
                {

                    model.AllGames = lookService.GetLookByLookId(lookId, filterForIds);

                    var feedback = await lookService.GetTeamFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate.Value, filterToDate.Value);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterFromDate;
                    model.FilterToDate = filterToDate;
                    model.AttributeNames = feedback.Item2;
                    model.FilterName = feedback.Item3;
                }

                else if (typeId == (int)LookType.User)
                {
                    model.AllGames = lookService.GetUserByLookId(lookId, filterForIds);
                    var feedback = await lookService.GetUserFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate.Value, filterToDate.Value);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterFromDate;
                    model.FilterToDate = filterToDate;
                    model.AttributeNames = feedback.Item2;
                    model.FilterName = feedback.Item3;
                }

                model.LookGroupList = lookService.GetGroupAllDrop(lookId, filterId, model.AttributeId);

            }
            model.FilterList = filterService.GetAll(CurrentUser.CompanyId, lookId.HasValue ? typeId : 0).Select(s => new SelectedItemDto()
            {
                Id = s.Id.ToString(),
                Name = s.Name
            }).ToList();

            return View(model);
        }


        public async Task<IActionResult> WeightagePreset(int? id, int parentId)
        {
            var deails = await service.GetDetailsId(parentId);
            WeightagePresetDto model = new WeightagePresetDto()
            {
                GameId = parentId,
                Name = $"Default {deails.Name}",
                GameName = deails.Name
            };



            if (!id.HasValue)
            {
                model.PresetList = weightagePresetService.GetAllDropdown(CurrentUser.CompanyId, parentId);
                List<int> allGameId = service.GetAllIdByParentId(parentId, CurrentUser.CompanyId);
                model.WeightagePresetDetails = await service.GetOnlyGamlePlayer(CurrentUser.CompanyId, allGameId);

                if (!model.PresetList.Any())
                {
                    model.IsDefault = true;
                    id = await weightagePresetService.Add(model);
                    model = await weightagePresetService.GetById(id.Value);
                    model.PresetList = weightagePresetService.GetAllDropdown(CurrentUser.CompanyId, parentId);
                    model.PresetId = id.Value;
                }
            }
            else
            {
                model = await weightagePresetService.GetById(id.Value);
                model.PresetId = id.Value;
                model.PresetList = weightagePresetService.GetAllDropdown(CurrentUser.CompanyId, parentId);
            }
            model.RoleList = roleService.GetAll(CurrentUser.CompanyId);
            model.PlayerList = userService.GetAllByDropByCompanyId(CurrentUser.CompanyId);
            model.GameList = service.GetAllByParentId(parentId, CurrentUser.CompanyId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WeightagePreset(WeightagePresetDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("", $"Name is Required");
            }

            if (!model.WeightagePresetDetails.Any())
            {
                ModelState.AddModelError("", $"Invalid user's weightage");
                return CreateModelStateErrors();
            }

            if (await weightagePresetService.IsExist(CurrentUser.CompanyId, model.Name, null))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            model.WeightagePresetDetails = model.WeightagePresetDetails.Where(s => s.RoleId.HasValue && s.GameId.HasValue && s.Weightage.HasValue && s.UserId > 0).ToList();
            int id = 0;
            try
            {
                id = await weightagePresetService.Add(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("WeightagePreset", new { parentId = model.GameId, id = id }), IsSuccess = true });
        }

        public async Task<IActionResult> FeedbackExport(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterFromDate,
            DateTime? filterToDate)
        {
            filterFromDate = filterFromDate.HasValue ? filterFromDate.Value : DateTime.Now;
            filterToDate = filterToDate.HasValue ? filterToDate.Value : DateTime.Now;
            var feeedbackModel = await GetFeedbackData(tenant, lookId, presetId, atterbuteId, filterId, filterFromDate.Value, filterToDate.Value);
            string FileName = string.Empty;
            var newFile = FeedbackExportExcel(feeedbackModel, out FileName);

            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }

        private FileInfo FeedbackExportExcel(GameGridFeedbackDto feeedbackModel, out string FileName)
        {

            string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"{feeedbackModel.GameName}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            feeedbackModel.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"{feeedbackModel.GameName}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = feeedbackModel.GameName;
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
                if (!feeedbackModel.IsSelf)
                {
                    worksheet.Cells[rowNo, column].Value = "Overall";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column = 3;

                    worksheet.Cells[rowNo, column].Value = "Overall Rating";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column = 4;


                    worksheet.Cells[rowNo, column].Value = "Overall Percentage";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    column = 5;
                }

                if (feeedbackModel.LookGroupList.Count > 0)
                {

                    foreach (var col in feeedbackModel.LookGroupList)
                    {
                        worksheet.Cells[rowNo, column].Value = col.Name;
                        worksheet.Cells[rowNo, column].AutoFitColumns(60);
                        worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                        worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        column++;

                        worksheet.Cells[rowNo, column].Value = $"{col.Name} Rating";
                        worksheet.Cells[rowNo, column].AutoFitColumns(60);
                        worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                        worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        column++;


                        worksheet.Cells[rowNo, column].Value = $"{col.Name} Percentage";
                        worksheet.Cells[rowNo, column].AutoFitColumns(60);
                        worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                        worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        column++;
                    }

                }
                rowNo++;


                foreach (var row in feeedbackModel.AllGames)
                {
                    column = 1;
                    var rowData = feeedbackModel.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);
                    var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{folder}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}") : "-";

                    worksheet.Cells[rowNo, column].Value = row.Name;
                    column++;
                    if (!feeedbackModel.IsSelf)
                    {
                        if (feeback.Contains("-mini."))
                        {
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                            var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.From.Row = rowNo - 1;
                            pic.From.Column = column - 1;
                            //pic.To.Row = rowNo;
                            //pic.To.Column = column;
                            pic.SetSize(100);

                        }
                        else
                        {
                            worksheet.Cells[rowNo, column].Value = feeback;
                        }

                        column++;
                        string Feedback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{ Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}") : "-";
                        var percentage = "";
                        if (rowData != null)
                        {
                            if (!rowData.IsWeighted)
                            {
                                percentage = rowData != null && rowData.Percentage > 0 && Math.Round(rowData.Percentage, MidpointRounding.AwayFromZero) > 0 ? Math.Round(rowData.Percentage, MidpointRounding.AwayFromZero).ToString() + "%" : "0";
                            }
                        }
                        worksheet.Cells[rowNo, column].Value = Feedback;
                        column++;
                        worksheet.Cells[rowNo, column].Value = percentage;
                        column++;
                    }

                    foreach (var item in feeedbackModel.LookGroupList)
                    {
                        var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                    (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folder}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
                        if (feeback1.Contains("-mini."))
                        {
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
                            var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.From.Row = rowNo - 1;
                            pic.From.Column = column - 1;
                            //pic.To.Row = rowNo;
                            //pic.To.Column = column;
                            pic.SetSize(100);
                        }
                        else
                        {
                            worksheet.Cells[rowNo, column].Value = feeback1;
                        }

                        column++;
                        var percentage = "";
                        string Feedback = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                    (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : $"{ Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}") : "-";
                        if (rowData != null)
                        {
                            if (!rowData.IsWeighted)
                            {
                                percentage = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Percentage > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ? Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Percentage, MidpointRounding.AwayFromZero).ToString() + "%" : "0";
                            }
                        }
                        worksheet.Cells[rowNo, column].Value = Feedback;
                        column++;
                        worksheet.Cells[rowNo, column].Value =  percentage;
                        column++;
                    }
                    rowNo++;
                    if (row.ChildGame.Any())
                    {
                        rowNo = SetRowFeedbackExport(rowNo, worksheet, row.ChildGame, feeedbackModel, folder);
                    }

                }
                pack.Save();
            }
            return newFile;
        }

        private int SetRowFeedbackExport(int rowNo, ExcelWorksheet worksheet, List<GameGridDto> rows, GameGridFeedbackDto feeedbackModel, string folderName)
        {
            int column = 1;
            foreach (var row in rows)
            {
                column = 1;
                var rowData = feeedbackModel.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);
                var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{folderName}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}") : "-";

                worksheet.Cells[rowNo, column].Value = row.Name;
                column++;

                if (!feeedbackModel.IsSelf)
                {
                    if (feeback.Contains("-mini."))
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
                        //pic.From.Row = rowNo-1;
                        //pic.From.Column = column-1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    else
                    {
                        worksheet.Cells[rowNo, column].Value = feeback;
                    }
                    column++;

                    worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero).ToString() : "-";
                    column++;
                }

                foreach (var item in feeedbackModel.LookGroupList)
                {
                    var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folderName}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
                    if (feeback1.Contains("-mini.png"))
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
                        // pic.From.Row = rowNo-1;
                        // pic.From.Column = column-1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    else
                    {
                        worksheet.Cells[rowNo, column].Value = feeback1;
                    }
                    column++;

                    worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                (rowData.IsQuantity ? Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) : Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)).ToString() : "-";
                    column++;
                }
                rowNo++;
                if (row.ChildGame.Any())
                {
                    rowNo = SetRowFeedbackExport(rowNo, worksheet, row.ChildGame, feeedbackModel, folderName);
                }

            }

            return rowNo;
        }
        private async Task<GameGridFeedbackDto> GetFeedbackData(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime filterFromDate,
            DateTime filterToDate)
        {

            GameGridFeedbackDto model = new GameGridFeedbackDto() { GameId = tenant, PresetId = presetId };

            List<int> filterForIds = new List<int>();
            if (filterId.HasValue)
            {
                var filterresponse = filterService.EmotionsForId(filterId.Value);
                filterForIds = filterresponse.Item1;
                model.IsSelf = filterresponse.Item2;
            }
            var deails = await service.GetDetailsId(tenant);
            model.GameName = deails.Name;


            int typeId = 0;
            if (lookId.HasValue || filterId.HasValue)
            {

                model.LookId = lookId;


                if (filterId.HasValue)
                {
                    model.AttributeList = lookService.GetAttributeDropByfilter(filterId.Value);
                    typeId = (await filterService.GetById(filterId.Value)).EmotionsFor.Value;
                }
                else
                if (lookId.HasValue)
                {
                    model.AttributeList = lookService.GetAttributeDrop(model.LookId.Value);
                    typeId = (await lookService.GetDetailsId(lookId.Value)).TypeId;
                }

                if (atterbuteId.HasValue && model.AttributeList.Any(a => int.Parse(a.Id) == atterbuteId))
                {
                    model.AttributeId = atterbuteId;
                }
                // var look = await lookService.GetDetailsId(lookId.Value);

                if (typeId == (int)LookType.Game)
                {
                    model.AllGames = service.GetAllByParentId(tenant, CurrentUser.CompanyId, filterForIds);
                    var feedback = await lookService.GetGameFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate, filterToDate);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterToDate;
                    model.AttributeNames = feedback.Item2;
                }

                else if (typeId == (int)LookType.Team)
                {

                    model.AllGames = lookService.GetLookByLookId(lookId, filterForIds);
                    var feedback = await lookService.GetTeamFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate, filterToDate);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterToDate;
                    model.AttributeNames = feedback.Item2;
                }

                else if (typeId == (int)LookType.User)
                {
                    model.AllGames = lookService.GetUserByLookId(lookId, filterForIds);
                    var feedback = await lookService.GetUserFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterFromDate, filterToDate);
                    model.Feedbacks = feedback.Item1;
                    model.FilterFromDate = filterFromDate;
                    model.AttributeNames = feedback.Item2;
                }

                model.LookGroupList = lookService.GetGroupAllDrop(lookId, filterId, model.AttributeId);

            }

            return model;
        }

        [HttpPost]
        public async Task<IActionResult> ShareExcel(List<string> users, int id, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterFromDate,
            DateTime? filterToDate)
        {
            try
            {
                filterFromDate = filterFromDate.HasValue ? filterFromDate.Value : DateTime.Now;
                filterToDate = filterToDate.HasValue ? filterToDate.Value : DateTime.Now;

                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var feeedbackModel = await GetFeedbackData(id, lookId, presetId, atterbuteId, filterId, filterFromDate.Value, filterToDate.Value);
                string FileName = string.Empty;
                var newFile = FeedbackExportExcel(feeedbackModel, out FileName);
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

        [HttpPost]
        public async Task<IActionResult> SharePDF(List<string> users, int id, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterFromDate,
            DateTime? filterToDate)
        {
            try
            {
                filterFromDate = filterFromDate.HasValue ? filterFromDate.Value : DateTime.Now;
                filterToDate = filterToDate.HasValue ? filterToDate.Value : DateTime.Now;


                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var feeedbackModel = await GetFeedbackData(id, lookId, presetId, atterbuteId, filterId, filterToDate.Value, filterToDate.Value);
                string FileName = string.Empty;
                var newFile = FeedbackExportExcel(feeedbackModel, out FileName);
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

        [HttpPost]
        public async Task<IActionResult> ShareGamePlayerExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var gameModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareGamePlayerExportExcel(gameModel, out FileName);
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


        private FileInfo ShareGamePlayerExportExcel(List<GameDto> gameModel, out string FileName)
        {

            string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Game Library List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Game Library List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Players List";
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
                worksheet.Cells[rowNo, column].Value = "Start Date";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 3;
                worksheet.Cells[rowNo, column].Value = "End Date";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 4;
                worksheet.Cells[rowNo, column].Value = "Contact Person";
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
                worksheet.Cells[rowNo, column].Value = "Last Update";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                rowNo++;


                foreach (var row in gameModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.FromDate;

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.Todate;

                    column = 4;
                    worksheet.Cells[rowNo, column].Value = row.ContactPerson;
                    column = 5;
                    worksheet.Cells[rowNo, column].Value = row.IsActive == true ? "Active" : "InActive";

                    column = 6;
                    worksheet.Cells[rowNo, column].Value = row.LastUpdate.HasValue ? row.LastUpdate.Value.ToString("dd/MM/yyyy") : "";

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }

    }
}
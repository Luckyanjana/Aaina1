﻿using System;
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
    public class GameController : BaseController
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
        public GameController(IGameService service, IRoleService roleService, IUserLoginService userService, ILookService lookService, IWeightagePresetService weightagePresetService, IFilterService filterService, IHostingEnvironment hostingEnvironment, IGeneratePdf generatePdf, IAttributeService attributeService, IWeightageService weightageService)
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
        public IActionResult Index(int tenant)
        {
            AddPageHeader("Game", "List");
            var all = service.GetAll(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            return View(all);
        }



        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GameDto model, int tenant)
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
            AddPageHeader("Game", "Add/Edit");
            GameDto model = new GameDto();
            model.ParentId = tenant;

            if (copyId.HasValue)
            {
                id = copyId;
            }

            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
                model.ParentId = tenant;
            }
            else if (tenant > 0)
            {
                var model1 = await service.GetById(tenant);
                model.FromDate = model1.FromDate;
                model.Todate = model1.Todate;
                model.Location = model1.Location;
                model.ContactNumber = model1.ContactNumber;
                model.ContactPerson = model1.ContactPerson;
                model.ClientName = model1.ClientName;
                model.GamePlayers = model1.GamePlayers.Select(x => new GamePlayerDto()
                {
                    IsAdded = true,
                    RoleId = x.RoleId,
                    TypeId = x.TypeId,
                    UserId = x.UserId
                }).ToList();

            }


            model.RoleList = roleService.GetAll(CurrentUser.CompanyId).Select(c => new SelectedItemDto() { Name = c.Name, Id = c.Id.Value.ToString() }).ToList();

            model.UserList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            {
                Name = $"{c.Fname} {c.Lname}",
                Id = c.Id.ToString(),
                Additional = c.PlayerType.ToString()
            }).ToList();
            model.ParentList = service.GetAllDrop(id, CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            {
                Name = c.Name,
                Id = c.Id.ToString()
            }).ToList();
            model.AllGame = service.GetAll(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            model.AllForChart = await service.GetAllForChart(tenant, CurrentUser.CompanyId);
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }
        private async Task<IActionResult> AddEdit(GameDto model, int tenant)
        {
            model.CompanyId = CurrentUser.CompanyId;
            model.CreatedBy = CurrentUser.UserId;

            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.GamePlayers.Any(a => a.IsAdded) && model.GamePlayers.Any(a => a.IsAdded && !a.RoleId.HasValue))
            {
                ModelState.AddModelError("", $"Please assign role to user this game");
                return CreateModelStateErrors();
            }

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            model.GamePlayers = model.GamePlayers.Where(x => x.IsAdded).ToList();

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                model.Id= await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            if (tenant == 0)
            {
                tenant = model.Id.Value;
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Game/index", IsSuccess = true });

        }


        //public async Task<IActionResult> GameFeebBack(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterDate)
        //{
        //    var menu = PermissionHelper.GetPermission(CurrentUser.UserId);
        //    LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);
        //    if (string.IsNullOrEmpty(menu) || leftMenu == null || leftMenu.LeftMenu==null || !leftMenu.LeftMenu.Any())
        //    {
        //        leftMenu = new LeftMenuDto();
        //        var gamemenu = await service.GetMenuNLevel(CurrentUser.CompanyId);
        //        leftMenu.LeftMenu = gamemenu;
        //        leftMenu.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
        //    }
        //    leftMenu.LeftMenuStatic = new List<MenuPermissionListDto>();           
        //    PermissionHelper.SetPermission(JsonConvert.SerializeObject(leftMenu), CurrentUser.UserId);

        //    filterDate = filterDate.HasValue ? filterDate.Value : DateTime.Now;
        //    GameGridFeedbackDto model = new GameGridFeedbackDto()
        //    {
        //        GameId = tenant,
        //        PresetId = presetId,
        //        FilterId = filterId,
        //        AttributeId = atterbuteId,
        //        FilterDate = filterDate
        //    };
        //    model.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
        //    List<int> filterForIds = new List<int>();
        //    if (filterId.HasValue)
        //    {
        //        var filterResponse = filterService.EmotionsForId(filterId.Value);
        //        filterForIds = filterResponse.Item1;
        //        model.IsSelf = filterResponse.Item2;
        //    }
        //    var deails = await service.GetDetailsId(tenant);
        //    model.GameName = deails.Name;

        //    var lookList = lookService.GetAllDrop(tenant, CurrentUser.CompanyId);
        //    //if (!lookId.HasValue && lookList.Any())
        //    //{
        //    //    lookId = int.Parse(lookList.FirstOrDefault().Id);

        //    //}
        //    model.LookList = lookList;
        //    int typeId = 0;
        //    if (lookId.HasValue || filterId.HasValue)
        //    {

        //        model.LookId = lookId;
                

        //        if (filterId.HasValue)
        //        {
        //            model.AttributeList = lookService.GetAttributeDropByfilter(filterId.Value);
        //            typeId = (await filterService.GetById(filterId.Value)).EmotionsFor.Value;
        //        }else
        //        if (lookId.HasValue)
        //        {
        //            model.AttributeList = lookService.GetAttributeDrop(model.LookId.Value);
        //            typeId = (await lookService.GetDetailsId(lookId.Value)).TypeId;
        //        }
                
        //        if (atterbuteId.HasValue && model.AttributeList.Any(a => int.Parse(a.Id) == atterbuteId))
        //        {
        //            model.AttributeId = atterbuteId;
        //        }
        //        // var look = await lookService.GetDetailsId(lookId.Value);

        //        if (typeId == (int)LookType.Game)
        //        {
        //            model.AllGames = service.GetAllByParentId(tenant, CurrentUser.CompanyId, filterForIds);
        //            model.Feedbacks = (await lookService.GetGameFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate.Value)).Item1;
        //        }

        //        else if (typeId == (int)LookType.Team)
        //        {

        //            model.AllGames = lookService.GetLookByLookId(lookId, filterForIds);


        //            model.Feedbacks = (await lookService.GetTeamFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate.Value)).Item1;
        //        }

        //        else if (typeId == (int)LookType.User)
        //        {
        //            model.AllGames = lookService.GetUserByLookId(lookId, filterForIds);
        //            model.Feedbacks = (await lookService.GetUserFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate.Value)).Item1;
        //        }

        //        model.LookGroupList = lookService.GetGroupAllDrop(lookId, filterId, model.AttributeId);

        //    }
        //    model.FilterList = filterService.GetAll(CurrentUser.CompanyId, lookId.HasValue ? typeId : 0).Select(s => new SelectedItemDto()
        //    {
        //        Id = s.Id.ToString(),
        //        Name = s.Name
        //    }).ToList();

        //    return View(model);
        //}


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

        [HttpGet]
        public IActionResult Delete(int id, int tenant, string action)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Game?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Game" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int tenant, string action, IFormCollection FormCollection)
        {
            action = !string.IsNullOrEmpty(action) ? action : "index";
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Game has been updated successfully.", false);
                //return Redirect($"/{tenant}/Game/index");
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Game/index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        [HttpGet]
        public IActionResult GetClientname(int id)
        {
            string name = service.GetClientName(id);
            return Json(new { ApplyForChild = true, ClientName = name });
        }

        [HttpGet]
        public async Task<IActionResult> GetClientNameForChile(int id)
        {
            var game = await service.GetById(id);
            return Json(new { ApplyForChild = true, game = game });
        }

        public async Task<IActionResult> GetGameist()
        {
            var TeamList = service.GetAllDrop(null, CurrentUser.CompanyId);
            return Json(TeamList);
        }



        //public async Task<IActionResult> FeedbackExport(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterDate)
        //{
        //    filterDate = filterDate.HasValue ? filterDate.Value : DateTime.Now;
        //    var feeedbackModel = await GetFeedbackData(tenant, lookId, presetId, atterbuteId, filterId, filterDate.Value);
        //    string FileName = string.Empty;
        //    var newFile = FeedbackExportExcel(feeedbackModel, out FileName);

        //    return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        //}

        //private FileInfo FeedbackExportExcel(GameGridFeedbackDto feeedbackModel, out string FileName)
        //{

        //    string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
        //    string filePath = "";
        //    FileName = $"{feeedbackModel.GameName}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
        //    feeedbackModel.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);

        //    int rowNo = 1;
        //    var newFile = FileOutputUtil.CreateFile($"{feeedbackModel.GameName}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
        //    using (ExcelPackage pack = new ExcelPackage(newFile))
        //    {
        //        ExcelWorksheet worksheet;
        //        string sheetName = feeedbackModel.GameName;
        //        var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
        //        if (sheet == null)
        //            worksheet = pack.Workbook.Worksheets.Add(sheetName);
        //        else
        //            worksheet = sheet;


        //        int column = 1;
        //        worksheet.Cells[rowNo, column].Value = "Name";
        //        worksheet.Cells[rowNo, column].AutoFitColumns(60);
        //        worksheet.Cells[rowNo, column].Style.Font.Bold = true;
        //        worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        column = 2;
        //        if (!feeedbackModel.IsSelf)
        //        {
        //            worksheet.Cells[rowNo, column].Value = "Overall";
        //            worksheet.Cells[rowNo, column].AutoFitColumns(60);
        //            worksheet.Cells[rowNo, column].Style.Font.Bold = true;
        //            worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //            worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            column = 3;

        //            worksheet.Cells[rowNo, column].Value = "Overall Rating";
        //            worksheet.Cells[rowNo, column].AutoFitColumns(60);
        //            worksheet.Cells[rowNo, column].Style.Font.Bold = true;
        //            worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //            worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            column = 4;
        //        }

        //        if (feeedbackModel.LookGroupList.Count > 0)
        //        {

        //            foreach (var col in feeedbackModel.LookGroupList)
        //            {
        //                worksheet.Cells[rowNo, column].Value = col.Name;
        //                worksheet.Cells[rowNo, column].AutoFitColumns(60);
        //                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
        //                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //                column++;

        //                worksheet.Cells[rowNo, column].Value = $"{col.Name} Rating";
        //                worksheet.Cells[rowNo, column].AutoFitColumns(60);
        //                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
        //                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //                column++;
        //            }

        //        }
        //        rowNo++;


        //        foreach (var row in feeedbackModel.AllGames)
        //        {
        //            column = 1;
        //            var rowData = feeedbackModel.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);
        //            var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{folder}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}") : "-";

        //            worksheet.Cells[rowNo, column].Value = row.Name;
        //            column++;
        //            if (!feeedbackModel.IsSelf)
        //            {
        //                if (feeback.Contains("-mini."))
        //                {
        //                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);                            
        //                    var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
        //                    pic.From.Row = rowNo-1;
        //                    pic.From.Column = column-1;
        //                    //pic.To.Row = rowNo;
        //                    //pic.To.Column = column;
        //                    pic.SetSize(100);

        //                }
        //                else
        //                {
        //                    worksheet.Cells[rowNo, column].Value = feeback;
        //                }

        //                column++;

        //                worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{ Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}") : "-";
        //                column++;
        //            }

        //            foreach (var item in feeedbackModel.LookGroupList)
        //            {
        //                var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
        //            (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folder}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
        //                if (feeback1.Contains("-mini."))
        //                {
        //                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
        //                    var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
        //                    pic.From.Row = rowNo-1;
        //                    pic.From.Column = column-1;
        //                    //pic.To.Row = rowNo;
        //                    //pic.To.Column = column;
        //                    pic.SetSize(100);
        //                }
        //                else
        //                {
        //                    worksheet.Cells[rowNo, column].Value = feeback1;
        //                }

        //                column++;

        //                worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
        //            (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : $"{ Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}") : "-";
        //                column++;
        //            }
        //            rowNo++;
        //            if (row.ChildGame.Any())
        //            {
        //                rowNo = SetRowFeedbackExport(rowNo, worksheet, row.ChildGame, feeedbackModel, folder);
        //            }

        //        }
        //        pack.Save();
        //    }
        //    return newFile;
        //}

        //private int SetRowFeedbackExport(int rowNo, ExcelWorksheet worksheet, List<GameGridDto> rows, GameGridFeedbackDto feeedbackModel, string folderName)
        //{
        //    int column = 1;
        //    foreach (var row in rows)
        //    {
        //        column = 1;
        //        var rowData = feeedbackModel.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);
        //        var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : @$"{folderName}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}") : "-";

        //        worksheet.Cells[rowNo, column].Value = row.Name;
        //        column++;

        //        if (!feeedbackModel.IsSelf)
        //        {
        //            if (feeback.Contains("-mini."))
        //            {
        //                System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
        //                var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
        //                pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
        //                //pic.From.Row = rowNo-1;
        //                //pic.From.Column = column-1;
        //                //pic.To.Row = rowNo;
        //                //pic.To.Column = column;
        //                pic.SetSize(100);
        //            }
        //            else
        //            {
        //                worksheet.Cells[rowNo, column].Value = feeback;
        //            }
        //            column++;

        //            worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero).ToString() : "-";
        //            column++;
        //        }

        //        foreach (var item in feeedbackModel.LookGroupList)
        //        {
        //            var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
        //        (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folderName}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
        //            if (feeback1.Contains("-mini.png"))
        //            {
        //                System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
        //                var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
        //                pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
        //               // pic.From.Row = rowNo-1;
        //               // pic.From.Column = column-1;
        //                //pic.To.Row = rowNo;
        //                //pic.To.Column = column;
        //                pic.SetSize(100);
        //            }
        //            else
        //            {
        //                worksheet.Cells[rowNo, column].Value = feeback1;
        //            }
        //            column++;

        //            worksheet.Cells[rowNo, column].Value = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
        //        (rowData.IsQuantity ? Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) : Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)).ToString() : "-";
        //            column++;
        //        }
        //        rowNo++;
        //        if (row.ChildGame.Any())
        //        {
        //            rowNo = SetRowFeedbackExport(rowNo, worksheet, row.ChildGame, feeedbackModel, folderName);
        //        }

        //    }

        //    return rowNo;
        //}
        //private async Task<GameGridFeedbackDto> GetFeedbackData(int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime filterDate)
        //{

        //    GameGridFeedbackDto model = new GameGridFeedbackDto() { GameId = tenant, PresetId = presetId };

        //    List<int> filterForIds = new List<int>();
        //    if (filterId.HasValue)
        //    {
        //        var filterresponse = filterService.EmotionsForId(filterId.Value);
        //        filterForIds = filterresponse.Item1;
        //        model.IsSelf = filterresponse.Item2;
        //    }
        //    var deails = await service.GetDetailsId(tenant);
        //    model.GameName = deails.Name;

           
        //    int typeId = 0;
        //    if (lookId.HasValue || filterId.HasValue)
        //    {

        //        model.LookId = lookId;


        //        if (filterId.HasValue)
        //        {
        //            model.AttributeList = lookService.GetAttributeDropByfilter(filterId.Value);
        //            typeId = (await filterService.GetById(filterId.Value)).EmotionsFor.Value;
        //        }
        //        else
        //        if (lookId.HasValue)
        //        {
        //            model.AttributeList = lookService.GetAttributeDrop(model.LookId.Value);
        //            typeId = (await lookService.GetDetailsId(lookId.Value)).TypeId;
        //        }

        //        if (atterbuteId.HasValue && model.AttributeList.Any(a => int.Parse(a.Id) == atterbuteId))
        //        {
        //            model.AttributeId = atterbuteId;
        //        }
        //        // var look = await lookService.GetDetailsId(lookId.Value);

        //        if (typeId == (int)LookType.Game)
        //        {
        //            model.AllGames = service.GetAllByParentId(tenant, CurrentUser.CompanyId, filterForIds);
        //            model.Feedbacks = (await lookService.GetGameFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate)).Item1;
        //        }

        //        else if (typeId == (int)LookType.Team)
        //        {

        //            model.AllGames = lookService.GetLookByLookId(lookId, filterForIds);


        //            model.Feedbacks = (await lookService.GetTeamFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate)).Item1;
        //        }

        //        else if (typeId == (int)LookType.User)
        //        {
        //            model.AllGames = lookService.GetUserByLookId(lookId, filterForIds);
        //            model.Feedbacks = (await lookService.GetUserFeedback(tenant, lookId, presetId, model.AttributeId, filterId, filterDate)).Item1;
        //        }

        //        model.LookGroupList = lookService.GetGroupAllDrop(lookId, filterId, model.AttributeId);

        //    }

        //    return model;
        //}
        ////9799898887

        //[HttpPost]
        //public async Task<IActionResult> ShareExcel(List<string> users, int id, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterDate)
        //{
        //    try
        //    {
        //        filterDate = filterDate.HasValue ? filterDate.Value : DateTime.Now;
        //        string allusersEmails = string.Join(";", users);
        //        string html = "meeting link";
        //        var attachments = new Dictionary<string, byte[]>();
        //        var feeedbackModel = await GetFeedbackData(id, lookId, presetId, atterbuteId, filterId, filterDate.Value);
        //        string FileName = string.Empty;
        //        var newFile = FeedbackExportExcel(feeedbackModel, out FileName);
        //        // attachments.Add(MimeEntity.Load(newFile.FullName));
        //        byte[] myFileAsByteArray = Common.Common.FileToByteArray(newFile.FullName);
        //        attachments.Add(newFile.FullName, myFileAsByteArray);

        //        Common.Common.SendMailWithAttachment(allusersEmails, "Share", html, attachments);
        //        ShowSuccessMessage("Success!", $"Share successfully.", false);
        //        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = "", Message = "Share successfully.", IsSuccess = true });

        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> SharePDF(List<string> users, int id, int? lookId, int? presetId, int? atterbuteId, int? filterId, DateTime? filterDate)
        //{
        //    try
        //    {
        //        filterDate = filterDate.HasValue ? filterDate.Value : DateTime.Now;
        //        string allusersEmails = string.Join(";", users);
        //        string html = "meeting link";
        //        var attachments = new Dictionary<string, byte[]>();
        //        var feeedbackModel = await GetFeedbackData(id, lookId, presetId, atterbuteId, filterId, filterDate.Value);
        //        string FileName = string.Empty;
        //        var newFile = FeedbackExportExcel(feeedbackModel, out FileName);
        //        // attachments.Add(MimeEntity.Load(newFile.FullName));
        //        byte[] myFileAsByteArray = Common.Common.FileToByteArray(newFile.FullName);
        //        attachments.Add(newFile.FullName, myFileAsByteArray);

        //        Common.Common.SendMailWithAttachment(allusersEmails, "Share", html, attachments);
        //        ShowSuccessMessage("Success!", $"Share successfully.", false);
        //        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = "", Message = "Share successfully.", IsSuccess = true });

        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
        //    }
        //}

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
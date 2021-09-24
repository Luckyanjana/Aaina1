using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly ILookService lookService;
        private readonly IGameService gameService;
        private readonly IGameFeedbackService service;
        private readonly IWeightageService weightageService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFilterService filterService;
        public ProjectController(IGameFeedbackService service, ILookService lookService, IGameService gameService, IWeightageService weightageService, IHostingEnvironment hostingEnvironment)
        {
            this.service = service;
            this.lookService = lookService;
            this.gameService = gameService;
            this.weightageService = weightageService;
            this._hostingEnvironment = hostingEnvironment;
            FileOutputUtil.OutputDir = new DirectoryInfo(hostingEnvironment.WebRootPath + "/TempExcel");
        }

        public IActionResult Index()
        {
            AddPageHeader("Game", "List");
            var all = gameService.GetAll(CurrentUser.CompanyId);
            return View(all);
        }

        public async Task<IActionResult> GameFeebBack(int tenant, int? lookId)
        {


            var menu = PermissionHelper.GetPermission(CurrentUser.UserId);
            LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);
            if (string.IsNullOrEmpty(menu) || leftMenu == null || leftMenu.LeftMenu == null || !leftMenu.LeftMenu.Any())
            {
                leftMenu = new LeftMenuDto();
                var gamemenu = await gameService.GetMenuNLevel(CurrentUser.CompanyId);
                leftMenu.LeftMenu = gamemenu;
                leftMenu.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
                leftMenu.LeftUserMenuStatic = await gameService.GetUserMenuStatic(CurrentUser.UserId);
            }

            leftMenu.LeftMenuStatic = await gameService.GetMenuStatic(CurrentUser.UserId, tenant);

            PermissionHelper.SetPermission(JsonConvert.SerializeObject(leftMenu), CurrentUser.UserId);


            GameGridFeedbackDto model = new GameGridFeedbackDto() { GameId = tenant };
            model.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            var deails = await gameService.GetDetailsId(tenant);
            model.GameName = deails.Name;

            var lookList = lookService.GetAllDrop(tenant, CurrentUser.CompanyId, CurrentUser.UserId);
            if (!lookId.HasValue && lookList.Any())
            {
                lookId = int.Parse(lookList.FirstOrDefault().Id);

            }

            if (lookId.HasValue)
            {
                model.LookList = lookList;
                model.LookId = lookId.Value;

                var look = await lookService.GetDetailsId(lookId.Value);

                if (look.TypeId == (int)LookType.Game)
                {
                    model.AllGames = gameService.GetAllByParentId(tenant, CurrentUser.CompanyId);
                    model.Feedbacks = (await lookService.GetGameFeedback(tenant, lookId.Value, null, model.AttributeId, null, DateTime.Now, DateTime.Now)).Item1;
                }

                else if (look.TypeId == (int)LookType.Team)
                {
                    model.AllGames = lookService.GetLookByLookId(lookId.Value);
                    model.Feedbacks = (await lookService.GetTeamFeedback(tenant, lookId.Value, null, model.AttributeId, null, DateTime.Now, DateTime.Now)).Item1;
                }

                else if (look.TypeId == (int)LookType.User)
                {
                    model.AllGames = lookService.GetUserByLookId(lookId.Value);
                    model.Feedbacks = (await lookService.GetUserFeedback(tenant, lookId.Value, null, model.AttributeId, null, DateTime.Now, DateTime.Now)).Item1;
                }
                model.LookGroupList = lookService.GetGroupAllDrop(lookId.Value, null, null);

            }
            ViewBag.id = tenant;
            return View(model);
        }

        public async Task<IActionResult> Feedback(int tenant, int? lookId, string pid)
        {
            LookFeebbackDto model = new LookFeebbackDto();
            List<int> gameId = new List<int>();

            if (!string.IsNullOrEmpty(pid))
            {
                gameId = pid.Split(",").Select(x => int.Parse(x)).ToList();
            }

            if (lookId.HasValue)
            {
                model = lookService.GetLookFeedbackByLookId(lookId.Value, CurrentUser.CompanyId, gameId);

                if (await service.IsDraftExist(lookId.Value, CurrentUser.UserId) || await service.IsDraftExistTeam(lookId.Value, CurrentUser.UserId) || await service.IsDraftExistUser(lookId.Value, CurrentUser.UserId))
                //if (true)
                {
                    if (model.TypeId == (int)LookType.Game)
                    {
                        GameFeedbackDto feedbackDto = await service.GetGameByLookId(lookId.Value, gameId, CurrentUser.UserId);
                        model.feedbackId = feedbackDto.Id;
                        model.GameFeedbackDetails = feedbackDto.GameFeedbackDetails;
                    }
                    else if (model.TypeId == (int)LookType.Team)
                    {
                        GameFeedbackDto feedbackDto = await service.GetTeamByLookId(lookId.Value, gameId, CurrentUser.UserId);
                        model.feedbackId = feedbackDto.Id;
                        model.GameFeedbackDetails = feedbackDto.GameFeedbackDetails;
                    }
                    else if (model.TypeId == (int)LookType.User)
                    {
                        GameFeedbackDto feedbackDto = await service.GetUserByLookId(lookId.Value, gameId, CurrentUser.UserId);
                        model.feedbackId = feedbackDto.Id;
                        model.GameFeedbackDetails = feedbackDto.GameFeedbackDetails;
                    }

                }
            }
            else
            {
                var gdata = await gameService.GetDetailsId(tenant);
                model.Game = gdata.Name;
                model.GameId = gdata.Id;
            }

            //model.LookList = lookService.GetAllDrop(gid, CurrentUser.CompanyId, CurrentUser.UserId);
            model.LookList = lookService.GetAllDropPrimission(tenant, CurrentUser.CompanyId, CurrentUser.UserId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(GameFeedbackDto model)
        {
            if (model.GameFeedbackDetails.Count(a => a.GameId.HasValue && a.AttributeId.HasValue && a.SubAttributeId.HasValue && a.Feedback.HasValue) == 0)
            {
                ModelState.AddModelError("", "Feedback not valid");
                return CreateModelStateErrors();
            }
            model.CompanyId = CurrentUser.CompanyId;
            model.UserId = CurrentUser.UserId;
            if (model.TypeId == (int)LookType.Game)
            {
                await service.AddUpdateGameAsync(model);
            }
            else if (model.TypeId == (int)LookType.Team)
            {
                await service.AddUpdateTeamAsync(model);
            }
            else if (model.TypeId == (int)LookType.User)
            {
                await service.AddUpdateUserAsync(model);
            }
            return Json(new RequestOutcome<string> { RedirectUrl = Url.Action("Feedback", new { tenant = model.GameId }), IsSuccess = true });

        }


        [HttpPost]
        public async Task<IActionResult> ShareExcel(List<string> users, int tenant, int? lookId, int? presetId, int? atterbuteId, int? filterId)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var feeedbackModel = await GetFeedbackData(tenant, lookId, presetId, atterbuteId, filterId);
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
                worksheet.Cells[rowNo, column].Value = "Game";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 2;
                worksheet.Cells[rowNo, column].Value = "Overall";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 3;

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
                    if (feeback.Contains("-mini."))
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.From.Row = rowNo-1;
                        pic.From.Column = column-1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    else
                    {
                        worksheet.Cells[rowNo, column].Value = feeback;
                    }

                    column++;

                    foreach (var item in feeedbackModel.LookGroupList)
                    {
                        var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
          (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folder}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
                        if (feeback1.Contains("-mini."))
                        {
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
                            var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.From.Row = rowNo-1;
                            pic.From.Column = column-1;
                            //pic.To.Row = rowNo;
                            //pic.To.Column = column;
                            pic.SetSize(100);
                        }
                        else
                        {
                            worksheet.Cells[rowNo, column].Value = feeback1;
                        }

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
                if (feeback.Contains("-mini."))
                {
                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                    var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                    pic.From.Row = rowNo-1;
                    pic.From.Column = column-1;
                    //pic.To.Row = rowNo;
                    //pic.To.Column = column;
                    pic.SetSize(100);
                }
                else
                {
                    worksheet.Cells[rowNo, column].Value = feeback;
                }
                column++;

                foreach (var item in feeedbackModel.LookGroupList)
                {
                    var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
          (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : @$"{folderName}{this.GetEmojiNameMini(feeedbackModel.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}") : "-";
                    if (feeback1.Contains("-mini.png"))
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.From.Row = rowNo-1;
                        pic.From.Column = column-1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    else
                    {
                        worksheet.Cells[rowNo, column].Value = feeback1;
                    }
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

        private async Task<GameGridFeedbackDto> GetFeedbackData(int id, int? lookId, int? presetId, int? atterbuteId, int? filterId)
        {

            GameGridFeedbackDto model = new GameGridFeedbackDto() { GameId = id, PresetId = presetId };

            List<int> filterForIds = new List<int>();
            if (filterId.HasValue)
            {
                var filterResponse = filterService.EmotionsForId(filterId.Value);
                filterForIds = filterResponse.Item1;
                model.IsSelf = filterResponse.Item2;
            }
            var deails = await gameService.GetDetailsId(id);
            model.GameName = deails.Name;

            if (lookId.HasValue)
            {
                model.LookId = lookId.Value;

                var look = await lookService.GetDetailsId(lookId.Value);

                if (look.TypeId == (int)LookType.Game)
                {
                    model.AllGames = gameService.GetAllByParentId(id, CurrentUser.CompanyId, filterForIds);
                    model.Feedbacks = (await lookService.GetGameFeedback(id, lookId.Value, presetId, atterbuteId, filterId, DateTime.Now, DateTime.Now)).Item1;
                }

                else if (look.TypeId == (int)LookType.Team)
                {
                    model.AllGames = lookService.GetLookByLookId(lookId.Value, filterForIds);
                    model.Feedbacks = (await lookService.GetTeamFeedback(id, lookId.Value, presetId, atterbuteId, filterId, DateTime.Now, DateTime.Now)).Item1;
                }

                else if (look.TypeId == (int)LookType.User)
                {
                    model.AllGames = lookService.GetUserByLookId(lookId.Value, filterForIds);
                    model.Feedbacks = (await lookService.GetUserFeedback(id, lookId.Value, presetId, atterbuteId, filterId, DateTime.Now, DateTime.Now)).Item1;
                }

                model.LookGroupList = lookService.GetGroupAllDrop(lookId.Value, filterId, atterbuteId);

            }

            return model;
        }

    }
}
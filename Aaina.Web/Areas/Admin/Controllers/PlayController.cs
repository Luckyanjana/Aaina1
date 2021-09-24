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
    public class PlayController : BaseController
    {
        private readonly IPlayService service;
        private readonly IGameService gameService;
        private readonly IUserLoginService userService;
        private readonly IHostingEnvironment env;
        private readonly IWeightageService emojiservice;
        public PlayController(IPlayService service, IGameService gameService, IUserLoginService userService, IHostingEnvironment env, IWeightageService emojiservice)
        {
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.env = env;
            this.emojiservice = emojiservice;
        }
        public IActionResult Index(int? gid, string v = "action")
        {
            PlayMainDto mainDto = new PlayMainDto();
            int type = (int)PlayType.Play;
            bool isToday = true;
            switch (v)
            {
                case "today":
                    type = (int)PlayType.Play;
                    isToday = true;
                    break;
                case "action":
                    type = (int)PlayType.Play;
                    isToday = false;
                    break;
                case "feedback":
                    type = (int)PlayType.Feedback;
                    isToday = false;
                    break;
                default:
                    break;
            }
            mainDto.Result = service.GetAll(CurrentUser.CompanyId, type, isToday, null, gid);
            mainDto.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(x=> new SelectListDto() {
            Text=x.Name,
            Value=int.Parse(x.Id)
            }).ToList();
            mainDto.ViewType = v;
            mainDto.GameId = gid;
            mainDto.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, null).Select(x => new SelectListDto()
            {
                Text = x.Name,
                Value = int.Parse(x.Id)
            }).ToList(); ;

            return View(mainDto);
        }

        public async Task<IActionResult> AddEdit(int? gid, int? id, int? t = 1)
        {
            ViewBag.gid = gid;
            PlayDto model = new PlayDto()
            {
                Type = t.HasValue ? t.Value : (int)PlayType.Play
            };

            if (id.HasValue)
            {
                model = service.GetById(id.Value);

                model.SubGameList = gameService.GetAllDropByParent(model.GameId.Value);
            }


            model.GameList = gameService.GetAllDropSecondParent(CurrentUser.CompanyId);
            model.ParentList = service.GetAllParentDrop(CurrentUser.CompanyId, t.Value, gid);
            model.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, null).ToList();
            model.PriorityList = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();

            model.FeedbackList = Enum.GetValues(typeof(FeedbackType)).Cast<FeedbackType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();

            return View("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(int? gid, PlayDto model)
        {
            if (model.Type == 0)
            {
                model.Type = (int)PlayType.Play;
            }

            model.CompanyId = CurrentUser.CompanyId;
            model.UserId = CurrentUser.UserId;
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
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
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", new { gid = gid }), IsSuccess = true });

        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(PlayDelegateDto model)
        {
            service.UpdateStatus(model);
            ShowSuccessMessage("Success!", $"Status has been added successfully.", false);

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }


        [HttpPost]
        public async Task<IActionResult> MoveToday(string id)
        {
            List<int> ids = new List<int>();
            if (!string.IsNullOrEmpty(id))
            {
                ids = id.Split(",").Select(x => int.Parse(x)).ToList();
            }
            service.MoveToday(ids);
            ShowSuccessMessage("Success!", $"Moved today sucessfully successfully.", false);

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int? gid, int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Play?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Play" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? gid, int id, IFormCollection FormCollection)
        {
            try
            {
                await service.Delete(id);
                ShowSuccessMessage("Success!", $"Play has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", new { gid = gid }), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }

        public IActionResult GetsubGameByGameId(int id)
        {
            var subGameList = gameService.GetAllDropByParent(id);
            return Json(subGameList);
        }

        [HttpPost]
        public async Task<IActionResult> SharePlayExcel(List<string> users, int id, int? gid, string v = "today")
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                int type = (int)PlayType.Play;
                bool isToday = true;
                switch (v)
                {
                    case "today":
                        type = (int)PlayType.Play;
                        isToday = true;
                        break;
                    case "action":
                        type = (int)PlayType.Play;
                        isToday = false;
                        break;
                    case "feedback":
                        type = (int)PlayType.Feedback;
                        isToday = false;
                        break;
                    default:
                        break;
                }
                var playModel = service.GetAll(id, type, isToday, null, gid);
                string FileName = string.Empty;
                var newFile = SharePlayExportExcel(playModel, out FileName, v);
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

        private FileInfo SharePlayExportExcel(List<PlayDto> playModel, out string FileName, string v = "today")
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = string.Empty;
            FileInfo newFile = null;
            string sheetName = string.Empty;
            var emojiModel = emojiservice.GetAll(CurrentUser.CompanyId);
            switch (v)
            {
                case "today":
                    FileName = $"Today Plans & Status -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    newFile = FileOutputUtil.CreateFile($"Today Plans & Status -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
                    sheetName = "Today Plans & Status";
                    break;
                case "action":
                    FileName = $"Action Plays -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    newFile = FileOutputUtil.CreateFile($"Action Plays -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
                    sheetName = "Action Plays";
                    break;
                case "feedback":
                    FileName = $"FeedBack Plays -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    newFile = FileOutputUtil.CreateFile($"FeedBack Plays -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
                    sheetName = "FeedBack Plays";
                    break;
                default:
                    break;
            }

            int rowNo = 1;

            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
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
                worksheet.Cells[rowNo, column].Value = "Sub Game";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 3;
                worksheet.Cells[rowNo, column].Value = "Play";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 4;
                worksheet.Cells[rowNo, column].Value = "Description";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 5;
                worksheet.Cells[rowNo, column].Value = "Person Involved";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 6;
                worksheet.Cells[rowNo, column].Value = "Accountable Person";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                if (v == "feedback")
                {
                    column = 7;
                    worksheet.Cells[rowNo, column].Value = "Dependancy";
                    worksheet.Cells[rowNo, column].AutoFitColumns(60);
                    worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                    worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                column = v == "feedback" ? 8 : 7;
                worksheet.Cells[rowNo, column].Value = "Priority";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 9 : 8;
                worksheet.Cells[rowNo, column].Value = "Realtime Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 10 : 9;
                worksheet.Cells[rowNo, column].Value = "Planned Start";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 11 : 10;
                worksheet.Cells[rowNo, column].Value = "Deadline";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 12 : 11;
                worksheet.Cells[rowNo, column].Value = "Actual Start";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 13 : 12;
                worksheet.Cells[rowNo, column].Value = "Actual End";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = v == "feedback" ? 14 : 13;
                worksheet.Cells[rowNo, column].Value = "Emotion";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                rowNo++;

                foreach (var row in playModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Game;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.SubGame;
                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.Name;
                    column = 4;
                    worksheet.Cells[rowNo, column].Value = row.Description;
                    column = 5;
                    worksheet.Cells[rowNo, column].Value = row.PersonInvolvedStr;
                    column = 6;
                    worksheet.Cells[rowNo, column].Value = row.Accountable;
                    if (v == "feedback")
                    {
                        column = 7;
                        worksheet.Cells[rowNo, column].Value = row.Dependancy;
                    }
                    column = v == "feedback" ? 8 : 7;
                    worksheet.Cells[rowNo, column].Value = ((PriorityType)row.Priority).GetEnumDescription();
                    column = v == "feedback" ? 9 : 8;
                    worksheet.Cells[rowNo, column].Value = ((StatusType)row.Status).GetEnumDescription();

                    column = v == "feedback" ? 10 : 9;
                    worksheet.Cells[rowNo, column].Value = row.StartDate.HasValue ? row.StartDate.Value.ToString("dd/MM/yyyy") : "";

                    column = v == "feedback" ? 11 : 10;
                    worksheet.Cells[rowNo, column].Value = row.DeadlineDate.HasValue ? row.DeadlineDate.Value.ToString("dd/MM/yyyy") : "";

                    column = v == "feedback" ? 12 : 11;
                    worksheet.Cells[rowNo, column].Value = row.ActualStartDate.HasValue ? row.ActualStartDate.Value.ToString("dd/MM/yyyy") : "";

                    column = v == "feedback" ? 13 : 12;
                    worksheet.Cells[rowNo, column].Value = row.ActualEndDate.HasValue ? row.ActualEndDate.Value.ToString("dd/MM/yyyy") : "";

                    column = v == "feedback" ? 14 : 13;
                    var feeback = @$"{folder}{this.GetEmojiNameMini(emojiModel, Math.Round(row.Emotion.HasValue ? row.Emotion.Value : 1.0, MidpointRounding.AwayFromZero))}";
                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                    var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                    pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
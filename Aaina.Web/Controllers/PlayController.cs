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
    public class PlayController : BaseController
    {
        private readonly IPlayService service;
        private readonly IGameService gameService;
        private readonly IUserLoginService userService;
        private readonly IFilterService filterService;
        private readonly ILookService lookService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWeightageService weightageService;
        public PlayController(IPlayService service, IGameService gameService, IUserLoginService userService, ILookService lookService, IWeightageService weightageService, IFilterService filterService, IHostingEnvironment hostingEnvironment)
        {
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.filterService = filterService;
            this.lookService = lookService;
            this._hostingEnvironment = hostingEnvironment;
            this.weightageService = weightageService;
        }

        public IActionResult Index(int tenant, int? uid, string v = "action", string Filter = "Individual")
        {
            PlayMainDto mainDto = new PlayMainDto();
            int type = (int)PlayType.Play;
            bool isToday = false ;
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
            // mainDto.Result = service.GetAll(CurrentUser.CompanyId, type, isToday, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : uid, tenant);
            mainDto.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto() { Text = c.Name, Value = int.Parse(c.Id) }).ToList();
            mainDto.ViewType = v;
            mainDto.GameId = tenant;
            mainDto.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, CurrentUser.UserId).Select(c => new SelectListDto() { Text = c.Name, Value = int.Parse(c.Id) }).ToList();
            mainDto.PriorityList = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            mainDto.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            mainDto.UserId = uid;
            mainDto.ValueType = Filter;
            //mainDto.PlayGridDtoList = service.GetPlayList(isToday, type, tenant, CurrentUser.CompanyId, uid);
            return View(mainDto);
        }

        public async Task<IActionResult> Add(int tenant, int? id, int? t = 1, bool isAgenda = false)
        {
            return await AddEdit(tenant, id, t, isAgenda);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int tenant, int? id, int? t = 1)
        {
            return await AddEdit(tenant, id, t);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int tenant, int? id, int? t = 1)
        {
            return await ViewDetail(tenant, id, t);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int tenant, PlayDto model)
        {
            return await AddEdit(tenant, model);
        }


        private async Task<IActionResult> AddEdit(int tenant, int? id, int? t = 1, bool isAgenda = false)
        {

            PlayDto model = new PlayDto()
            {
                Type = t.HasValue ? t.Value : (int)PlayType.Play,
                AccountableId = CurrentUser.UserId,
                GameId = tenant,
                IsAgenda = isAgenda
            };

            if (id.HasValue)
            {
                model = service.GetById(id.Value);


            }

            model.SubGameList = gameService.GetAllDropByParent(model.GameId.Value);
            model.ParentList = service.GetAllParentDrop(CurrentUser.CompanyId, t.Value, tenant);
            model.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, null).ToList();
            model.PriorityList = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();

            model.FeedbackList = Enum.GetValues(typeof(FeedbackType)).Cast<FeedbackType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            if (isAgenda)
            {
                return PartialView("_AddEditWithAgenda", model);
            }
            else
            {
                return View("_AddEdit", model);
            }

        }

        private async Task<IActionResult> AddEdit(int? tenant, PlayDto model)
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

            PreSessionAgendaDto details = new PreSessionAgendaDto();
            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                int id = await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
                details = service.GetPlayAction(id);
            }
            return NewtonSoftJsonResult(new RequestOutcome<PreSessionAgendaDto> { RedirectUrl = $"/{tenant}/Play/Index", IsSuccess = true, Data = details });

        }

        private async Task<IActionResult> ViewDetail(int tenant, int? id, int? t = 1, bool isAgenda = false)
        {

            PlayDto model = new PlayDto()
            {
                Type = t.HasValue ? t.Value : (int)PlayType.Play,
                AccountableId = CurrentUser.UserId,
                GameId = tenant,
                IsAgenda = isAgenda
            };

            if (id.HasValue)
            {
                model = service.GetById(id.Value);


            }

            model.SubGameList = gameService.GetAllDropByParent(model.GameId.Value);
            model.ParentList = service.GetAllParentDrop(CurrentUser.CompanyId, t.Value, tenant);
            model.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, null).ToList();
            model.PriorityList = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.FeedbackList = Enum.GetValues(typeof(FeedbackType)).Cast<FeedbackType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.PlayFeedBack = service.GetAllFeedBack((model.Id.HasValue ? model.Id.Value : 0));
            model.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            return View("_Detail", model);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(PlayDelegateDto model)
        {
            service.UpdateStatus(model);
            ShowSuccessMessage("Success!", $"Status has been added successfully.", false);

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateFeedback(PlayFeedbackDto model)
        {
            model.CreatedBy = CurrentUser.UserId;
            await service.AddFeedBack(model);
            ShowSuccessMessage("Success!", $"Feedback has been added successfully.", false);

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
        public async Task<IActionResult> Approve(int? tenant, int id)
        {
            try
            {
                await service.Approve(id);
                ShowSuccessMessage("Success!", $"Play game level has been approved.", false);

                return Redirect($"/{tenant}/Play/Index");
                //return RedirectToAction("Index", new { tenant = tenant });
                //return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        [HttpGet]
        public IActionResult Delete(int? tenant, int id)
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
        public async Task<IActionResult> Delete(int? tenant, int id, IFormCollection FormCollection)
        {
            try
            {
                await service.Delete(id);
                ShowSuccessMessage("Success!", $"Play has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Play/Index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        public IActionResult GetsubGameByGameId(int id)
        {
            var subGameList = gameService.GetAllDropByParent(id);
            return Json(subGameList);
        }



        [HttpPost]
        public async Task<IActionResult> SharePlayExcel(List<string> users, int id, int? tenant, string v = "today")
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
                var playModel = service.GetAll(id, type, isToday, null, tenant);
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

        public async Task<IActionResult> ExcelExport(int tenant, string v = "today")
        {
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
            var playModel = service.GetAll(CurrentUser.CompanyId, type, isToday, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            string FileName = string.Empty;
            var newFile = SharePlayExportExcel(playModel, out FileName, v);

            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }

        private FileInfo SharePlayExportExcel(List<PlayDto> playModel, out string FileName, string v = "today")
        {
            string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath  = "";
            FileName = string.Empty;
            FileInfo newFile = null;
            string sheetName = string.Empty;
            var emojiModel = weightageService.GetAll(CurrentUser.CompanyId);
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
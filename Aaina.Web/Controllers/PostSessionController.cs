using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Controllers
{
    public class PostSessionController : BaseController
    {
        private readonly IPostSessionService postSessionService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWeightageService weightageService;
        private readonly IDropBoxService dropBoxService;
        private readonly IUserLoginService userService;
        public PostSessionController(IPostSessionService preSessionService, IWeightageService weightageService, IHostingEnvironment hostingEnvironment, IDropBoxService dropBoxService, IUserLoginService userService)
        {
            this.postSessionService = preSessionService;
            this.weightageService = weightageService;
            this._hostingEnvironment = hostingEnvironment;
            this.dropBoxService = dropBoxService;
            this.userService = userService;
            FileOutputUtil.OutputDir = new DirectoryInfo(hostingEnvironment.WebRootPath + "/TempExcel");

        }
        public async Task<IActionResult> Index(int tenant, int sessionId, DateTime start, DateTime end)
        {
            PostSessionDto model = await postSessionService.GetByForPostId(sessionId, start, end, CurrentUser.UserId);
            model.PriorityList = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, CurrentUser.UserId).ToList();
            model.tenant = tenant;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PostSessionDto model)
        {
            model.CreatedBy = CurrentUser.UserId;
            model.CompanyId = CurrentUser.CompanyId;
            await postSessionService.AddUpdateAsync(model);           
            string url = $"/{model.tenant}/PostSession/index?sessionId={model.SessionId}&start={model.StartDate.ToString("MM/dd/yyy HH:mm")}&end={model.EndDate.Value.ToString("MM/dd/yyy HH:mm")}";
              // return RedirectToAction("index", "PostSession",new { tenant=model.tenant, sessionId =model.SessionId,start=model.StartDate.ToString("MM/dd/yyy HH:mm"),end= model.EndDate.Value.ToString("MM/dd/yyy HH:mm") });
            return Redirect(url);
        }

        public async Task<IActionResult> Export(int sessionId, DateTime start, DateTime end)
        {
            var emojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            PostSessionDto model = await postSessionService.GetByForPostId(sessionId, start, end, CurrentUser.UserId);
            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"{model.SessionName}-PostSession-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = model.SessionName;
                var sheet = pack.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                if (sheet == null)
                    worksheet = pack.Workbook.Worksheets.Add(sheetName);
                else
                    worksheet = sheet;


                int column = 1;
                worksheet.Cells[rowNo, column].Value = "Added On";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 2;
                worksheet.Cells[rowNo, column].Value = "Agenda Item";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Game";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 4;
                worksheet.Cells[rowNo, column].Value = "Sub Game";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 5;
                worksheet.Cells[rowNo, column].Value = "Accountability";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                column = 6;
                worksheet.Cells[rowNo, column].Value = "Dependency";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 7;
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 8;
                worksheet.Cells[rowNo, column].Value = "Priority";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                column = 9;
                worksheet.Cells[rowNo, column].Value = "Action Item";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 10;
                worksheet.Cells[rowNo, column].Value = "Deadline";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 11;
                worksheet.Cells[rowNo, column].Value = "Remarks";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 12;
                worksheet.Cells[rowNo, column].Value = "Emotions";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 13;
                worksheet.Cells[rowNo, column].Value = "Coordinate Emotion";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                column = 14;
                worksheet.Cells[rowNo, column].Value = "Decision Maker Emotion";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 15;
                worksheet.Cells[rowNo, column].Value = "Last Updated";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                rowNo++;

                string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";

                foreach (var row in model.PostSessionAgenda)
                {
                    column = 1;

                    var feeback = row.Emotions.HasValue ? @$"{folder}{this.GetEmojiNameMini(emojiList, row.Emotions.Value)}" : "-";
                    var feeback1 = row.CoordinateEmotion.HasValue ? @$"{folder}{this.GetEmojiNameMini(emojiList, row.CoordinateEmotion.Value)}" : "-";
                    var feeback2 = row.DecisionMakerEmotion.HasValue ? @$"{folder}{this.GetEmojiNameMini(emojiList, row.DecisionMakerEmotion.Value)}" : "-";

                    worksheet.Cells[rowNo, column].Value = row.AddedOn.ToString("dd/MM/yyyy");
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Name;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Game;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.SubGame;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Accountable;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Dependancy;
                    column++;
                   
                    worksheet.Cells[rowNo, column].Value = row.StatusStr;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Prioritystr;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Description;
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.DeadlineDate.ToString("dd MMM");
                    column++;

                    worksheet.Cells[rowNo, column].Value = row.Remarks;
                    column++;

                    if (row.Emotions.HasValue)
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);                        
                        pic.From.Row = rowNo - 1;
                        pic.From.Column = column - 1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    column++;

                    if (row.CoordinateEmotion.HasValue)
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback1);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
                        pic.From.Row = rowNo - 1;
                        pic.From.Column = column - 1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    column++;

                    if (row.DecisionMakerEmotion.HasValue)
                    {
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback2);
                        var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                        pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);
                        pic.From.Row = rowNo - 1;
                        pic.From.Column = column - 1;
                        //pic.To.Row = rowNo;
                        //pic.To.Column = column;
                        pic.SetSize(100);
                    }
                    column++;

                    worksheet.Cells[rowNo, column].Value = model.ModifiedDate.HasValue?model.ModifiedDate.Value.ToString("dd/MM/yyyy"):string.Empty;
                    column++;

                    rowNo++;
                }
                pack.Save();
            }

            string FileName = $"{model.SessionName}-PostSession-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(System.IO.File.ReadAllBytes(newFile.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }

        
    }
}
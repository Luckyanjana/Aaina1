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
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Controllers
{
    public class WeightageController : BaseController
    {
        private readonly IWeightageService service;
        private readonly IHostingEnvironment env;
        public WeightageController(IWeightageService service, IHostingEnvironment env)
        {
            this.service = service;
            this.env = env;

        }
        public IActionResult Index()
        {
            AddPageHeader("Emoji", "List");
            var all = service.GetAll(CurrentUser.CompanyId);
            return View(all);
        }

        public async Task<IActionResult> AddEdit(int? id, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }
            WeightageDto model = new WeightageDto();
            
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return PartialView("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(WeightageDto model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", $"Invalid");
                return CreateModelStateErrors();
            }

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }


            if (await service.IsExistRating(CurrentUser.CompanyId, model.Rating.Value, model.Id))
            {
                ModelState.AddModelError("", $"Rating already register!");
                return CreateModelStateErrors();
            }

            var emojiImg = this.Request.Form.Files["emojiImg"];
            if (emojiImg != null)
            {
                var profileExten = new[] { ".jpg", ".png", ".jpeg" };
                var ext = Path.GetExtension(emojiImg.FileName).ToLower();
                if (!profileExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Imoji image not valid, Please choose jpg,png,jpeg format");
                    return CreateModelStateErrors();
                }

            }
            else if (!model.Id.HasValue)
            {
                ModelState.AddModelError("", $"Imoji image is required");
                return CreateModelStateErrors();
            }

            model.Emoji = emojiImg != null ? await this.UploadProfile(env, CurrentUser.CompanyId, emojiImg, $"{model.Emoji}", "EmojiImages", new[] { ".png", ".jpg", ".jpeg" }, model.Rating.Value.ToString()) : model.Emoji;

            if (emojiImg != null)
                 await this.UploadProfile(env, CurrentUser.CompanyId, emojiImg, $"{model.Emoji}-mini", "EmojiImages", new[] { ".png", ".jpg", ".jpeg" }, $"{model.Rating.Value.ToString()}-mini");


            if (model.Id > 0)
            {
                await service.Update(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                await service.Add(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Emoji?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Weightage" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Emoji has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ShareEmojiExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var emojiModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareEmojiExportExcel(emojiModel, out FileName);
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

        private FileInfo ShareEmojiExportExcel(List<WeightageDto> emojiModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Emoji List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Emoji List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Emoji List";
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
                worksheet.Cells[rowNo, column].Value = "Rating";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Emoji";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 4;
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                rowNo++;


                foreach (var row in emojiModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.Rating;

                    column = 3;
                    var feeback = @$"{folder}{this.GetEmojiNameMiniFromName(row.Emoji)}";
                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(feeback);
                    var pic = worksheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                    pic.SetPosition(rowNo - 1, rowNo - 1, column - 1, column);

                    column = 4;
                    worksheet.Cells[rowNo, column].Value = row.IsActive == true ? "Active" : "InActive";

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }

    }
}
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
using Microsoft.Extensions.Logging.Abstractions;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Controllers
{
    public class AttributeController : BaseController
    {
        private readonly IAttributeService service;
        private readonly IGameService GameService;
        private readonly IHostingEnvironment env;
        public AttributeController(IAttributeService service, IGameService GameService, IHostingEnvironment env)
        {
            this.service = service;
            this.GameService = GameService;
            this.env = env;
        }
        public IActionResult Index()
        {
            AddPageHeader("Attribute", "List");
            
            return View();
        }

        public async Task<IActionResult> AddEdit(int? id, int? copyId)
        {
            AttributeDto model = new AttributeDto();
            ViewBag.GameList = this.GameService.GetAllDrop(null, CurrentUser.CompanyId);

            if (copyId.HasValue)
            {
                id = copyId;
            }

            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            model.UnitList = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().Select(s => new SelectListDto()
            {
                Text = s.GetEnumDescription(),
                Value = (int)s
            }).ToList();

            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return PartialView("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(AttributeDto model)
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
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Attribute?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Attribute" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Attribute has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShareAttributeExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var attributeModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareAttributeExportExcel(attributeModel, out FileName);
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

        private FileInfo ShareAttributeExportExcel(List<AttributeDto> attributeModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Attribute List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Attribute List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Attribute List";
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
                worksheet.Cells[rowNo, column].Value = "Description";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "Status";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                rowNo++;


                foreach (var row in attributeModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.Desciption;

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.IsActive == true ? "Active" : "InActive";

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
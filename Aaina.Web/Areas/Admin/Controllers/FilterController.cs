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
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class FilterController : BaseController
    {
        private readonly IFilterService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly IHostingEnvironment env;
        public FilterController(IFilterService service, IGameService gameService, IUserLoginService userService, ITeamService teamService, IAttributeService attributeService, IHostingEnvironment env)
        {
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.teamService = teamService;
            this.attributeService = attributeService;
            this.env = env;
        }
        public IActionResult Index()
        {
            var all = service.GetAll(CurrentUser.CompanyId);
            return View(all);
        }

        public async Task<IActionResult> AddEdit(int? id, int? copyId)
        {

            FilterDto model = new FilterDto();

            if (copyId.HasValue)
            {
                id = copyId;
            }

            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
                model.StartTime = model.StartDateTime.Value.TimeOfDay;
                model.EndTime = model.EndDateTime.Value.TimeOfDay;

                if (model.EmotionsFor == (int)EmotionsFor.Game)
                {
                    model.EmotionsForList = gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = c.Name,
                        Value = int.Parse(c.Id)
                    }).ToList();

                }
                else if (model.EmotionsFor == (int)EmotionsFor.Team)
                {
                    model.EmotionsForList = teamService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = c.Name,
                        Value = int.Parse(c.Id)
                    }).ToList();
                }
                else
                {
                    model.EmotionsForList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectListDto()
                    {
                        Text = $"{c.Fname} {c.Lname}",
                        Value = c.Id
                    }).ToList();
                }


            }
            else
            {


                model.EmotionsForList = gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
                {
                    Text = c.Name,
                    Value = int.Parse(c.Id)
                }).ToList();


            }


            model.EmotionsFromList = teamService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectListDto()
            {
                Text = c.Name,
                Value = int.Parse(c.Id)
            }).ToList();

            model.EmotionsFromPList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectListDto()
            {
                Text = $"{c.Fname} {c.Lname}",
                Value = c.Id
            }).ToList();

            model.PlayerListList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectListDto()
            {
                Text = $"{c.Fname} {c.Lname}",
                Value = c.Id
            }).ToList();

            model.AttributeList = attributeService.GetAll(CurrentUser.CompanyId).Select(s => new SelectListDto()
            {
                Text = s.Name,
                Value = s.Id.Value
            }).ToList();
            model.CalculatiotTypeList = Enum.GetValues(typeof(LookCalculation)).Cast<LookCalculation>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.EmotionsForTypeList = Enum.GetValues(typeof(EmotionsFor)).Cast<EmotionsFor>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.EmotionsFromTypeList = Enum.GetValues(typeof(EmotionsFrom)).Cast<EmotionsFrom>().Select(c => new SelectListDto() { Text = c.GetEnumDescription(), Value = (int)c }).ToList();
            model.AllRecord = service.GetAll(CurrentUser.CompanyId);
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(FilterDto model)
        {

            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (!model.Players.Any(a => a.IsCalculation || a.IsView))
            {
                ModelState.AddModelError("", $"Please select user this filter");
                return CreateModelStateErrors();
            }

            if (!model.FromIds.Any() && !model.FromPIds.Any())
            {
                ModelState.AddModelError("", $"Please team or user for emotions from");
                return CreateModelStateErrors();
            }

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.StartDateTime.HasValue && model.StartTime.HasValue)
                model.StartDateTime = new DateTime(model.StartDateTime.Value.Year, model.StartDateTime.Value.Month, model.StartDateTime.Value.Day, model.StartTime.Value.Hours, model.StartTime.Value.Minutes, model.StartTime.Value.Seconds);

            if (model.EndDateTime.HasValue && model.EndTime.HasValue)
                model.EndDateTime = new DateTime(model.EndDateTime.Value.Year, model.EndDateTime.Value.Month, model.EndDateTime.Value.Day, model.EndTime.Value.Hours, model.EndTime.Value.Minutes, model.EndTime.Value.Seconds);

            model.Players = model.Players.Where(x => x.IsView || x.IsCalculation).ToList();

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
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("AddEdit"), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Filter?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Filter" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                await service.Delete(id);
                ShowSuccessMessage("Success!", $"Filter has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("AddEdit"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ShareFilterExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var filterModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareFilterExportExcel(filterModel, out FileName);
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

        private FileInfo ShareFilterExportExcel(List<FilterDto> filterModel, out string FileName)
        {
            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Filter List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Filter List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Filter List";
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
                worksheet.Cells[rowNo, column].Value = "For";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                column = 3;
                worksheet.Cells[rowNo, column].Value = "From";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                rowNo++;

                foreach (var row in filterModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = ((EmotionsFor)row.EmotionsFor).GetEnumDescription();

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = ((EmotionsFrom)row.EmotionsFrom).GetEnumDescription();

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }
    }
}
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
    public class TeamController : BaseController
    {
        private readonly ITeamService service;
        private readonly IGameService gameService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public TeamController(ITeamService service, IGameService gameService, IRoleService roleService, IHostingEnvironment hostingEnvironment, IUserLoginService userService)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this._hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(int tenant)
        {
            AddPageHeader("Team", "List");
            var all = service.GetAll(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            return View(all);
        }

        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TeamDto model, int tenant)
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
            AddPageHeader("Team", "Add/Edit");
            TeamDto model = new TeamDto();
            model.GameId = tenant;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }

            model.RoleList = roleService.GetAll(CurrentUser.CompanyId).Select(c => new SelectedItemDto() { Name = c.Name, Id = c.Id.Value.ToString() }).ToList();

            model.UserList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            {
                Name = $"{c.Fname} {c.Lname}",
                Id = c.Id.ToString(),
                Additional = c.PlayerType.ToString()
            }).ToList();
            //model.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            //{
            //    Name = c.Name,
            //    Id = c.Id.ToString()
            //}).ToList();

            model.AllForChart = await gameService.GetAllForChart(CurrentUser.CompanyId);
            model.AllRecord = service.GetAllActive(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }

                
        private async Task<IActionResult> AddEdit(TeamDto model,int tenant)
        {
            model.CompanyId = CurrentUser.CompanyId;
            model.CreatedBy = CurrentUser.UserId;
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.TeamPlayer.Any(a => a.IsAdded) && model.TeamPlayer.Any(a => a.IsAdded && !a.RoleId.HasValue))
            {
                ModelState.AddModelError("", $"Please assign role to user this game");
                return CreateModelStateErrors();
            }

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            model.TeamPlayer = model.TeamPlayer.Where(x => x.IsAdded).ToList();

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
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Team/index", IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id,int tenant)
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
        public async Task<IActionResult> Delete(int id,int tenant, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Team has been updated successfully.", false);
                // return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("AddEdit"), IsSuccess = true });
                return Redirect($"/{tenant}/Team/index");

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", IsSuccess = false });
            }
        }

        public async Task<IActionResult> GetTeamList()
        {
            var TeamList = service.GetAllDrop(null, CurrentUser.CompanyId);
            return Json(TeamList);
        }


        [HttpPost]
        public async Task<IActionResult> ShareTeamExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var teamModel = service.GetAll(id);
                string FileName = string.Empty;
                var newFile = ShareTeamExportExcel(teamModel, out FileName);
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

        private FileInfo ShareTeamExportExcel(List<TeamDto> teamModel, out string FileName)
        {
            string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Team List -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Team List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
            using (ExcelPackage pack = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet;
                string sheetName = "Team List";
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
                worksheet.Cells[rowNo, column].Value = "Weight Age";
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


                foreach (var row in teamModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Name;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.Weightage;

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
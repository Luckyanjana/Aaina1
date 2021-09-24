using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using Wkhtmltopdf.NetCore;

namespace Aaina.Web.Controllers
{
    public class CommonController : BaseCommonController
    {
        private readonly IGeneratePdf generatePdf;
        private readonly IGameService gameService;
        private readonly IStatusService statusService;
        private readonly IUserLoginService userService;
        private readonly ITeamService teamService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDropBoxService dropBoxService;
        public CommonController(IGeneratePdf _generatePdf, IStatusService statusService, IGameService gameService, IUserLoginService _userService, ITeamService teamService, IHostingEnvironment hostingEnvironment, IDropBoxService dropBoxService)
        {
            generatePdf = _generatePdf;
            this.gameService = gameService;
            this.statusService = statusService;
            this.userService = _userService;
            this.teamService = teamService;
            this._hostingEnvironment = hostingEnvironment;
            this.dropBoxService = dropBoxService;
        }

        [HttpPost]
        public async Task<ActionResult> ExporttoPDF(string htmlContent)
        {

            var result = this.generatePdf.GetPDF(htmlContent);
            return this.File(result, "application/pdf", DateTime.Now.Ticks + ".pdf");

        }

        [HttpPost]
        public async Task<ActionResult> ExporttoExcel(string htmlContent)
        {
            var result = this.generatePdf.GetPDF(htmlContent);
            return this.File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.Ticks + ".xlsx");

        }

        [HttpGet]
        public async Task<IActionResult> StatusFeedback(int? id)
        {
            StatusFeedbackDto model = new StatusFeedbackDto();
            if (id.HasValue)
            {
                model = await statusService.GetFeedbackDetailsById(id.Value);
                model.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId);
                model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectedItemDto()
                {
                    Name = c.GetEnumDescription(),
                    Id = ((int)c).ToString()
                }).ToList();
            }
            else
            {
                model.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId);
                model.StatusList = Enum.GetValues(typeof(StatusType)).Cast<StatusType>().Select(c => new SelectedItemDto()
                {
                    Name = c.GetEnumDescription(),
                    Id = ((int)c).ToString()
                }).ToList();
                model.TeamList = teamService.GetAllDrop(null, CurrentUser.CompanyId);

                model.UserList = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectedItemDto()
                {
                    Name = $"{c.Fname} {c.Lname}",
                    Id = c.Id.ToString()
                }).ToList();
                model.ModeList = Enum.GetValues(typeof(StatusModeType)).Cast<StatusModeType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            }
            return PartialView("_feedback", model);
        }

        [HttpPost]
        public async Task<IActionResult> StatusFeedback(StatusFeedbackPostDto model)
        {
            model.UserId = CurrentUser.UserId;

            if (!model.Id.HasValue && model.Nonstatus != null && !string.IsNullOrEmpty(model.Nonstatus.Name))
            {
                model.Nonstatus.CompanyId = CurrentUser.CompanyId;
                model.Nonstatus.CreatedBy = CurrentUser.UserId;
                model.Id = await statusService.AddNonStatusAsync(model.Nonstatus);
            }

            model.UserId = CurrentUser.UserId;
            await statusService.AddFeedbackAsync(model);
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index"), IsSuccess = true });
        }


        [HttpGet]
        public async Task<IActionResult> ShareUsers(string popupurl)
        {
            var details = userService.GetByCompanyyId(CurrentUser.CompanyId).Select(c => new SelectedItemDto()
            {
                Name = $"{c.Fname} {c.Lname}",
                Id = c.Id.ToString(),
                Additional = c.Email.ToString(),
            }).ToList();
            ViewBag.PopupUrl = popupurl.Replace("$", "&");
            return PartialView("_ShareUsers", details);
        }

        public IActionResult GetsubGameByGameId(int id)
        {
            var subGameList = gameService.GetAllDropByParent(id);
            return Json(subGameList);
        }

        [HttpPost]
        public async Task<IActionResult> SharePDF(List<string> users, string htmlContent)
        {
            try
            {
                string folder = $"{_hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
                string allusersEmails = string.Join(";", users);
                var result = this.generatePdf.GetPDF(htmlContent);
                string FileName = $"{CurrentUser.Name} -{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf";
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();

                attachments.Add(FileName, result);
                Common.Common.SendMailWithAttachment(allusersEmails, "Share", html, attachments);
                ShowSuccessMessage("Success!", $"Share successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = "", Message = "Share successfully.", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }

        }

        public async Task<IActionResult> GetPagePermission(string controllerName, string actionName)
        {
            var pagePermission = gameService.GetPagePermission(CurrentUser.UserId, CurrentUser.RoleId, controllerName, actionName);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = JsonConvert.SerializeObject(pagePermission), IsSuccess = true }); ;
        }

        public async Task<IActionResult> DownloadFileByBropbox(string id, string path)
        {
            var stream = dropBoxService.Download(path, id);
            return File(stream, GetContentType(id), id);
        }
        public async Task<IActionResult> DownloadBropbox(string fileName, string path)
        {
            var stream = dropBoxService.Download(path);
            return File(stream, GetContentType(fileName), fileName);
        }

        public async Task<IActionResult> ShareBropbox(string id, string path)
        {
            string shareUrl = dropBoxService.Share(path + "/" + id);
            return PartialView("_shareDropboxUrl", new Modal
            {
                Message = shareUrl,
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Public URL" },
                Footer = new ModalFooter { CancelButtonText = "Close", OnlyCancelButton = true }
            });

        }

        public async Task<IActionResult> ShareBropboxFile(string path)
        {
            string shareUrl = dropBoxService.Share(path);
            return Json(shareUrl);

        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}
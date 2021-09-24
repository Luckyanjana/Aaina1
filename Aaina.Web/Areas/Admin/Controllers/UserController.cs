using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code;
using Aaina.Web.Code.LIBS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aaina.Web.Areas.Admin.Controllers
{

    public class UserController : BaseController
    {
        private readonly IUserLoginService service;
        private readonly IHostingEnvironment env;
        private readonly IGameService gameService;
        private readonly IRoleService roleService;
        public UserController(IUserLoginService service, IHostingEnvironment env, IGameService gameService, IRoleService roleService)
        {
            this.service = service;
            this.env = env;
            this.gameService = gameService;
            this.roleService = roleService;
        }
        public IActionResult Index()
        {
            AddPageHeader("User", "List");
            List<UserLoginDto> list = service.GetAllByCompanyyId(CurrentUser.CompanyId);
            return View(list);
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.PlayerTypeList = Enum.GetValues(typeof(PlayersType)).Cast<PlayersType>().Select(c => new SelectedItemDto() { Name = c.ToString(), Id = ((int)c).ToString() }).ToList();
            return PartialView("_Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var userNameValid = await service.UserNameExist(dto.UserName, null);
            if (userNameValid)
            {
                string registerName = "User name";
                ModelState.AddModelError("", $"this {registerName} already register please try with another {registerName}");
                return CreateModelStateErrors();
            }

            var emailValid = await service.EmailExist(dto.Email, null);
            if (emailValid)
            {
                string registerName = "email";
                ModelState.AddModelError("", $"this {registerName} already register please try with another {registerName}");
                return CreateModelStateErrors();
            }
            string salt = PasswordHasher.GenerateSalt();
            dto.Password = PasswordHasher.GeneratePassword("123456", salt);
            dto.UserType = (int)UserType.User;
            int companyId = CurrentUser.CompanyId;
            int userId = await service.Register(dto, salt, companyId);
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = Url.Action("index"), IsSuccess = true, Data = new { name = $"{dto.Fname} {dto.Lname}", id = userId, playersTypeId = dto.PlayerType, playersType = ((PlayersType)dto.PlayerType).ToString() } });

        }


        public async Task<IActionResult> AddEdit(int? id)
        {
            UserProfileDto model = new UserProfileDto();

            if (id.HasValue)
            {
                model = service.GetByUserProfileId(id.Value);
            }
            model.AllGame = gameService.GetAllByParentId(null, CurrentUser.CompanyId);
            model.AllForChart = await gameService.GetAllForChart(CurrentUser.CompanyId);
            model.RoleList = roleService.GetAll(CurrentUser.CompanyId).Select(c => new SelectedItemDto() { Name = c.Name, Id = c.Id.Value.ToString() }).ToList();
            model.GameRoleList = id.HasValue ? await gameService.GetPlayerGamleRole(CurrentUser.CompanyId, id.Value) : new List<UserGameRole>();
            model.PlayerTypeList = Enum.GetValues(typeof(PlayersType)).Cast<PlayersType>().Select(c => new SelectedItemDto() { Name = c.ToString(), Id = ((int)c).ToString() }).ToList();
            return View("_AddEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(UserProfileDto dto)
        {

            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            var userNameValid = await service.UserNameExist(dto.UserName, dto.Id);
            if (userNameValid)
            {
                string registerName = "User name";
                ModelState.AddModelError("", $"this {registerName} already register please try with another {registerName}");
                return CreateModelStateErrors();
            }

            var emailValid = await service.EmailExist(dto.Email, dto.Id);
            if (emailValid)
            {
                string registerName = "email";
                ModelState.AddModelError("", $"this {registerName} already register please try with another {registerName}");
                return CreateModelStateErrors();
            }

            if (dto.GameRoleList.Any(a => a.IsAdded) && dto.GameRoleList.Any(a => a.IsAdded && !a.RoleId.HasValue))
            {
                ModelState.AddModelError("", $"Please assign role to user");
                return CreateModelStateErrors();
            }

            dto.CompanyId = CurrentUser.CompanyId;

            var allFiles = this.Request.Form.Files;
            var idProffFileFile = allFiles["IdProffFileFile"];
            var eduCertFile = allFiles["EduCertFile"];
            var expCertFile = allFiles["ExpCertFile"];
            var policeVerificationFile = allFiles["PoliceVerificationFile"];
            var otherFile = allFiles["OtherFile"];
            var avatarUrlfile = allFiles["AvatarUrlfile"];

            if (avatarUrlfile != null)
            {
                //GoogleDriveAPIHelper.UplaodFileOnDrive(this.env,avatarUrlfile);

                var profileExten = new[] { ".jpg", ".png", ".jpeg" };
                var ext = Path.GetExtension(avatarUrlfile.FileName).ToLower();
                if (!profileExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Profile image not valid, Please choose jpg,png,jpeg format");
                    return CreateModelStateErrors();
                }

            }

            if (idProffFileFile != null)
            {
                var pdfExten = new[] { ".pdf" };
                var ext = Path.GetExtension(idProffFileFile.FileName).ToLower();
                if (!pdfExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Id Proof not valid file, Please choose pdf format");
                    return CreateModelStateErrors();
                }

            }


            if (eduCertFile != null)
            {
                var pdfExten = new[] { ".pdf" };
                var ext = Path.GetExtension(eduCertFile.FileName).ToLower();
                if (!pdfExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Education Certificates not valid file, Please choose pdf format");
                    return CreateModelStateErrors();
                }

            }

            if (expCertFile != null)
            {
                var pdfExten = new[] { ".pdf" };
                var ext = Path.GetExtension(expCertFile.FileName).ToLower();
                if (!pdfExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Experience Certificates not valid file, Please choose pdf format");
                    return CreateModelStateErrors();
                }

            }

            if (policeVerificationFile != null)
            {
                var pdfExten = new[] { ".pdf" };
                var ext = Path.GetExtension(policeVerificationFile.FileName).ToLower();
                if (!pdfExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Police Verification not valid file, Please choose pdf format");
                    return CreateModelStateErrors();
                }

            }

            if (otherFile != null)
            {
                var pdfExten = new[] { ".pdf" };
                var ext = Path.GetExtension(otherFile.FileName).ToLower();
                if (!pdfExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"Other not valid file, Please choose pdf format");
                    return CreateModelStateErrors();
                }

            }


            dto.AvatarUrl = avatarUrlfile != null ? await this.UploadProfile(env, CurrentUser.CompanyId, avatarUrlfile, dto.AvatarUrl, "EmployeeImages", new[] { ".jpg", ".png", ".jpg", ".jpeg" }) : dto.AvatarUrl;

            dto.IdProffFile = idProffFileFile != null ? await UploadProfile(env, CurrentUser.CompanyId, idProffFileFile, dto.IdProffFile, "Userdoc", new[] { ".pdf" }) : dto.IdProffFile;

            dto.EduCert = eduCertFile != null ? await UploadProfile(env, CurrentUser.CompanyId, eduCertFile, dto.EduCert, "Userdoc", new[] { ".pdf" }) : dto.EduCert;

            dto.ExpCert = expCertFile != null ? await UploadProfile(env, CurrentUser.CompanyId, expCertFile, dto.ExpCert, "Userdoc", new[] { ".pdf" }) : dto.ExpCert;

            dto.PoliceVerification = policeVerificationFile != null ? await UploadProfile(env, CurrentUser.CompanyId, policeVerificationFile, dto.PoliceVerification, "Userdoc", new[] { ".pdf" }) : dto.PoliceVerification;

            dto.Other = otherFile != null ? await UploadProfile(env, CurrentUser.CompanyId, otherFile, dto.Other, "Userdoc", new[] { ".pdf" }) : dto.Other;

            if (dto.Password != "**********")
            {
                string salt = PasswordHasher.GenerateSalt();
                dto.Password = PasswordHasher.GeneratePassword(dto.Password, salt);
                dto.UserType = (int)UserType.User;
                dto.SaltKey = salt;
            }

            int userId = await service.AddUpdateUserProfileAsync(dto);
            var allGameRole = dto.GameRoleList.Any() ? dto.GameRoleList.Where(x => x.IsAdded).ToList() : new List<UserGameRole>();
            await gameService.AddUpdatePlayerGamleRole(CurrentUser.CompanyId, userId, allGameRole);
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = Url.Action("index"), IsSuccess = true, Data = new { name = $"{dto.Fname} {dto.Lname}", id = userId } });

        }


        public async Task<IActionResult> GetUserList()
        {
            var TeamList = service.GetAllDrop(CurrentUser.CompanyId, null);
            return Json(TeamList);
        }

        [HttpPost]
        public async Task<IActionResult> ShareExcel(List<string> users, int id)
        {
            try
            {
                string allusersEmails = string.Join(";", users);
                string html = "meeting link";
                var attachments = new Dictionary<string, byte[]>();
                var usersModel = service.GetAllByCompanyyId(id);
                string FileName = string.Empty;
                var newFile = UsersExportExcel(usersModel, out FileName);
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

        private FileInfo UsersExportExcel(List<UserLoginDto> usersModel, out string FileName)
        {

            string folder = $"{env.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
            string filePath = "";
            FileName = $"Players List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            int rowNo = 1;
            var newFile = FileOutputUtil.CreateFile($"Players List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx");
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
                worksheet.Cells[rowNo, column].Value = "Type";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 3;
                worksheet.Cells[rowNo, column].Value = "User Name";
                worksheet.Cells[rowNo, column].AutoFitColumns(60);
                worksheet.Cells[rowNo, column].Style.Font.Bold = true;
                worksheet.Cells[rowNo, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNo, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNo, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                column = 4;
                worksheet.Cells[rowNo, column].Value = "Doj";
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


                rowNo++;


                foreach (var row in usersModel)
                {
                    column = 1;
                    worksheet.Cells[rowNo, column].Value = row.Fname + " " + row.Lname;

                    column = 2;
                    worksheet.Cells[rowNo, column].Value = row.UserType;

                    column = 3;
                    worksheet.Cells[rowNo, column].Value = row.UserName;

                    column = 4;
                    worksheet.Cells[rowNo, column].Value = row.Doj;
                    column = 5;
                    worksheet.Cells[rowNo, column].Value = row.IsActive == true ? "Active" : "InActive";

                    rowNo++;
                }
                pack.Save();
            }
            return newFile;
        }

        //[HttpPost]
        //public async Task<IActionResult> SharePDF(List<string> users, int id, int? lookId, int? presetId, int? atterbuteId, int? filterId)
        //{
        //    try
        //    {
        //        string allusersEmails = string.Join(";", users);
        //        string html = "meeting link";
        //        var attachments = new Dictionary<string, byte[]>();
        //        var feeedbackModel = await GetFeedbackData(id, lookId, presetId, atterbuteId, filterId);
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

    }
}
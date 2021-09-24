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

namespace Aaina.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserLoginService service;
        private readonly IHostingEnvironment env;
        private readonly IGameService gameService;
        private readonly IRoleService roleService;
        private readonly IDropBoxService dropBoxService;
        public UserController(IUserLoginService service, IHostingEnvironment env, IGameService gameService, IRoleService roleService, IDropBoxService dropBoxService)
        {
            this.service = service;
            this.env = env;
            this.gameService = gameService;
            this.roleService = roleService;
            this.dropBoxService = dropBoxService;
        }
        public IActionResult Index(int tenant)
        {
            AddPageHeader("User", "List");
            List<UserLoginDto> list = service.GetAllByCompanyyId(CurrentUser.CompanyId, tenant);
            return View(list);
        }

        public async Task<IActionResult> Register(int tenant)
        {
            ViewBag.PlayerTypeList = Enum.GetValues(typeof(PlayersType)).Cast<PlayersType>().Select(c => new SelectedItemDto() { Name = c.ToString(), Id = ((int)c).ToString() }).ToList();
            return PartialView("_Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto, int tenant)
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
            dropBoxService.CreateFolder($"/{CurrentUser.CompanyId}/{userId}");
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = Url.Action("index"), IsSuccess = true, Data = new { name = $"{dto.Fname} {dto.Lname}", id = userId, playersTypeId = dto.PlayerType, playersType = ((PlayersType)dto.PlayerType).ToString() } });

        }

        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit((int?)null, tenant);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserProfileDto model, int tenant)
        {
            return await AddEdit(model, tenant);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int tenant)
        {
            return await AddEdit(id, tenant);
        }
        private async Task<IActionResult> AddEdit(int? id, int tenant)
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
        private async Task<IActionResult> AddEdit(UserProfileDto dto, int tenant)
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

            if (!dto.Id.HasValue)
            {
                string salt = PasswordHasher.GenerateSalt();
                RegisterDto registerDto = new RegisterDto()
                {
                    Password = dto.Password,
                    Email = dto.Email,
                    Fname = dto.Fname,
                    Lname = dto.Lname,
                    Mname = dto.Mname,
                    UserName = dto.UserName,
                    PlayerType = dto.PlayerType,
                    UserType = dto.UserType
                };

                registerDto.Password = PasswordHasher.GeneratePassword(dto.Password, salt);
                registerDto.UserType = (int)UserType.User;
                int userId1 = await service.Register(registerDto, salt, CurrentUser.CompanyId);
                dto.Id = userId1;
                dropBoxService.CreateFolder($"/{CurrentUser.CompanyId}/{userId1}");
            }

            var allFiles = this.Request.Form.Files;
            var idProffFileFile = allFiles["IdProffFileFile"];
            var eduCertFile = allFiles["EduCertFile"];
            var expCertFile = allFiles["ExpCertFile"];
            var policeVerificationFile = allFiles["PoliceVerificationFile"];
            var otherFile = allFiles["OtherFile"];
            var avatarUrlfile = allFiles["AvatarUrlfile"];

            if (avatarUrlfile != null)
            {

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

            if (idProffFileFile != null)
            {
                if (!string.IsNullOrEmpty(dto.IdProffFile))
                {
                    dropBoxService.Delete($"/{CurrentUser.CompanyId}/{dto.Id}/{dto.IdProffFile}");
                }
                dto.IdProffFile = idProffFileFile.FileName;
                dropBoxService.Upload(idProffFileFile, $"/{CurrentUser.CompanyId}/{dto.Id}");
            }

            if (eduCertFile != null)
            {
                if (!string.IsNullOrEmpty(dto.EduCert))
                {
                    dropBoxService.Delete($"/{CurrentUser.CompanyId}/{dto.Id}/{dto.EduCert}");
                }

                dto.EduCert = eduCertFile.FileName;
                dropBoxService.Upload(eduCertFile, $"/{CurrentUser.CompanyId}/{dto.Id}");
            }

            if (expCertFile != null)
            {
                if (!string.IsNullOrEmpty(dto.ExpCert))
                {
                    dropBoxService.Delete($"/{CurrentUser.CompanyId}/{dto.Id}/{dto.ExpCert}");
                }

                dto.ExpCert = expCertFile.FileName;
                dropBoxService.Upload(eduCertFile, $"/{CurrentUser.CompanyId}/{dto.Id}");
            }

            if (policeVerificationFile != null)
            {
                if (!string.IsNullOrEmpty(dto.PoliceVerification))
                {
                    dropBoxService.Delete($"/{CurrentUser.CompanyId}/{dto.Id}/{dto.PoliceVerification}");
                }

                dto.PoliceVerification = policeVerificationFile.FileName;
                dropBoxService.Upload(policeVerificationFile, $"/{CurrentUser.CompanyId}/{dto.Id}");
            }



            if (otherFile != null)
            {
                if (!string.IsNullOrEmpty(dto.Other))
                {
                    dropBoxService.Delete($"/{CurrentUser.CompanyId}/{dto.Id}/{dto.Other}");
                }

                dto.Other = otherFile.FileName;
                dropBoxService.Upload(otherFile, $"/{CurrentUser.CompanyId}/{dto.Id}");
            }

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
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = $"/{tenant}/user/index", IsSuccess = true, Data = new { name = $"{dto.Fname} {dto.Lname}", id = userId } });

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
    }
}
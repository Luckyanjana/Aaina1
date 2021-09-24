using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code.LIBS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Aaina.Web.Controllers
{
    public class AccountController : BaseCommonController
    {
        // GET: /<controller>/
        private readonly IUserLoginService usersService;
        private readonly ICompanyService companyService;
        private readonly IHostingEnvironment env;
        private readonly IGameService gameService;
        private readonly IWeightageService weightageService;


        public AccountController(IUserLoginService _usersService, ICompanyService companyService, IHostingEnvironment _env, IGameService gameService, IWeightageService weightageService)
        {
            this.usersService = _usersService;
            this.companyService = companyService;
            this.env = _env;
            this.gameService = gameService;
            this.weightageService = weightageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto requestDto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (string.IsNullOrEmpty(returnUrl) || returnUrl.ToLower().Contains("logoff"))
            {
                returnUrl = "/home";
            }
            ViewData["ReturnUrl"] = returnUrl;

            var user = await usersService.GetByUserNameOrEmail(requestDto.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User does not exist");
                return CreateModelStateErrors();
            }

            //if (!user.IsEmailVerify)
            //{
            //    ModelState.AddModelError("", "Your account is not verified. Please use below link to resend account verification email.");
            //    return CreateModelStateErrors();
            //}

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Your account has been deactivated, please contact to admin.");
                return CreateModelStateErrors();
            }

            //DateTime dateTime = DateTime.Now;
            //DateTime lockDatetime = dateTime.AddMinutes(-15);
            //if (user.IsLocked && user.LockedDateTime >= lockDatetime)
            //{
            //    ModelState.AddModelError("", "Your account is currently locked, please contact your administrator or try again later");
            //    return CreateModelStateErrors();
            //}

            bool isValid = PasswordHasher.VerifyHashedPassword(user.Password, requestDto.Password, user.SaltKey);
            if (!isValid)
            {

                //user.InvalidAttempt = (user.InvalidAttempt.HasValue ? user.InvalidAttempt.Value : 0) + 1;

                //if (user.InvalidAttempt >= 3)
                //{
                //    user.IsLocked = true;
                //    user.LockedDateTime = DateTime.Now;
                //}
                //await service.UpdateAsync(user);
                //if (user.InvalidAttempt >= 3)
                //{
                //    ModelState.AddModelError("", "Your account is currently locked, please contact your administrator or try again later");
                //}
                //else
                //{
                ModelState.AddModelError("", "Username or password invalid ");
                // }

                return CreateModelStateErrors();
            }
            //else
            //{
            //    user.InvalidAttempt = 0;
            //    user.IsLocked = false;
            //    user.LockedDateTime = null;
            //    await service.UpdateAsync(user);
            //}


            //if (!user.IsEmailVerify)
            //{
            //    ModelState.AddModelError("", "Your email is not verify yet, Please verify and login again");
            //    return CreateModelStateErrors();
            //}

            if (string.IsNullOrEmpty(user.Email))
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", "home"), IsSuccess = true });
            }
            //string profilePicture = !string.IsNullOrEmpty(user.AvatarUrl) ? $"/{user.CompanyId}/EmployeeImages/{user.AvatarUrl}" : $"/img/avatar.png";
            await CreateAuthenticationTicket(user);
            PermissionHelper.RemovePermission(user.Id);
            if (user.UserType == (int)UserType.SuperAdmin)
            {
                return Json(new RequestOutcome<string> { RedirectUrl = Url.Action("index", "home", new { area = "SuperAdmin" }), IsSuccess = true });
            }
            else if (user.UserType == (int)UserType.Admin)
            {
                
                var defaultGame = gameService.GetFirstGame(user.CompanyId);

                LeftMenuDto leftMenu = new LeftMenuDto();
                var gamemenu = await gameService.GetMenuNLevel(user.CompanyId);
                leftMenu.LeftMenu = gamemenu;
                leftMenu.LeftMenuStatic = await gameService.GetMenuStatic(user.Id, defaultGame.Id.Value);
                leftMenu.LeftUserMenuStatic = await gameService.GetUserMenuStatic(user.Id);
                leftMenu.EmojiList = weightageService.GetAllActive(user.CompanyId);
                PermissionHelper.SetPermission(JsonConvert.SerializeObject(leftMenu), user.Id);

                return Json(new RequestOutcome<string> { RedirectUrl = $"/{defaultGame.Id}/Dashboard/Index", IsSuccess = true });

            }
            else
            {
                var defaultGame = gameService.GetFirstGame(user.CompanyId);
                return Json(new RequestOutcome<string> { RedirectUrl = $"/{defaultGame.Id}/project/gamefeebback", IsSuccess = true });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            RegisterDto registerDto = new RegisterDto();
            return View(registerDto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {


            if (ModelState.IsValid)
            {
                var userNameValid = await usersService.UserNameExist(registerDto.UserName, null);
                if (userNameValid)
                {

                    ModelState.AddModelError("", $"this username already register please try with another username");
                    return CreateModelStateErrors();
                }

                var emailValid = await usersService.EmailExist(registerDto.Email, null);
                if (emailValid)
                {
                    ModelState.AddModelError("", $"this email already register please try with another email");
                    return CreateModelStateErrors();
                }

                string salt = PasswordHasher.GenerateSalt();
                registerDto.Password = PasswordHasher.GeneratePassword(registerDto.Password, salt);
                registerDto.UserType = (int)UserType.User;
                int companyId = (await companyService.FirstOrDefault()).Id;
                int userId = await usersService.Register(registerDto, salt, companyId);

                //string html = System.IO.File.ReadAllText($"{env.WebRootPath}/EmailTemplate/VerifyEmail.html", Encoding.UTF8);

                //string siteUrl = $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.Value}/";
                //string activeLink = $"{siteUrl}Email/VerifyEmail/{userId}";

                //html = html.Replace("##name##", registerDto.Name).Replace("##sitename##", EmailConfigurationCore.SenderName)
                //    .Replace("##link##", activeLink).Replace("##sitenurl##", siteUrl);

                //await Common.SendWelComeMail(registerDto.EmailAddress, $"Verify your email address for {EmailConfigurationCore.SenderName}", html);
                TempData["message"] = "Register";
                TempData["isValidRequest"] = true;
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Register"), IsSuccess = true });

            }

            return CreateModelStateErrors();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Thanks()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(IFormCollection fc)
        {
            try
            {
                string userName = fc["UserName"];

                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("", $"User name/Email required");
                    return CreateModelStateErrors();
                }

                var userobj = await usersService.GetByUserNameOrEmail(userName);

                if (userobj == null)
                {
                    ModelState.AddModelError("", "Invalid email/user name");
                    return CreateModelStateErrors();
                }
                //else if (!userobj.IsEmailVerify)
                //{

                //    ModelState.AddModelError("", $"Inactive account");
                //    return CreateModelStateErrors();
                //}

                var passwordResetLink = Guid.NewGuid().ToString();

                usersService.InsertPasswordReset(userobj.Id, passwordResetLink);

                TempData["message"] = "Reset link has been sent to you on your register email, please check your inbox.";

                string html = System.IO.File.ReadAllText($"{env.WebRootPath}/EmailTemplate/ForgotPasswordEmail.html", Encoding.UTF8);

                string siteUrl = $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.Value}/";
                string activeLink = $"{siteUrl}Account/SetupPassword/{passwordResetLink}";

                html = html.Replace("##name##", $"{userobj.Fname} {userobj.Lname}").Replace("##sitename##", "Aaina")
                    .Replace("##link##", activeLink).Replace("##siteurl##", siteUrl);

                Common.Common.SendWelComeMail(userobj.Email, "Reset your password", html);

                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("ForgotPassword"), IsSuccess = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                return CreateModelStateErrors();
            }


        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult ReSendVerification()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReSendVerification(IFormCollection fc)
        {
            try
            {
                string userName = fc["UserName"];

                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("", $"User name/Email required");
                    return CreateModelStateErrors();
                }

                var userobj = await usersService.GetByUserNameOrEmail(userName);

                if (userobj == null)
                {
                    ModelState.AddModelError("", "Invalid email/user name");
                    return CreateModelStateErrors();
                }

                string html = System.IO.File.ReadAllText($"{env.WebRootPath}/EmailTemplate/VerifyEmail.html", Encoding.UTF8);

                string siteUrl = $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.Value}/";
                string activeLink = $"{siteUrl}Email/VerifyEmail/{userobj.Id}";

                html = html.Replace("##name##", userobj.Fname).Replace("##sitename##", "Aaina")
                    .Replace("##link##", activeLink).Replace("##sitenurl##", siteUrl);

                 Common.Common.SendWelComeMail(userobj.Email, $"Verify your email address for Aaina", html);
                TempData["message"] = "Account active  link has been sent to you on your register email, please check your inbox.";
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("resendverification"), IsSuccess = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Internal error occured.");
                return CreateModelStateErrors();
            }


        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SetupPassword(string id)
        {
            PasswordResetDto model = new PasswordResetDto();

            var pObj = await usersService.GetPasswordResetByLink(id);

            if (pObj == null)
            {
                ViewBag.Invalid = "Invalid Link";
                return View();
            }
            else if (pObj.IsForgotVerified)
            {
                ViewBag.Invalid = "Link already verified.";
                return View(model);
            }
            else if (pObj.LinkExpiredDate < DateTime.UtcNow)
            {
                ViewBag.Invalid = "Link expired.";
                return View(model);
            }

            if (pObj != null && pObj.IsForgotVerified == false && pObj.LinkExpiredDate >= DateTime.UtcNow)
            {

                model.UserId = pObj.Id;
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SetupPassword(PasswordResetDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var profile = await usersService.GetById(model.UserId);
                    if (profile == null)
                    {
                        ModelState.AddModelError("", "Invalid user");
                        return CreateModelStateErrors();
                    }
                    string salt = PasswordHasher.GenerateSalt();
                    profile.SaltKey = salt;
                    profile.Password = PasswordHasher.GeneratePassword(model.NewPassword, salt);
                    profile.IsForgotVerified = true;
                    //profile.IsLocked = false;
                    //profile.LockedDateTime = null;
                    //profile.InvalidAttempt = 0;
                    await usersService.UpdateAsync(profile);
                    TempData["message"] = "Reset password";
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("setuppassword"), IsSuccess = true });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Internal error occured.");
                    return CreateModelStateErrors();
                }
            }

            ModelState.AddModelError("", "Internal error occured.");
            return CreateModelStateErrors();
        }


        public IActionResult ChangePassword()
        {
            AddPageHeader("Change Password", "");
            ChangePasswordDto model = new ChangePasswordDto();
            return View("ChangePassword", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var profile = await usersService.GetById(CurrentUser.UserId);
                bool isValid = PasswordHasher.VerifyHashedPassword(profile.Password, model.CurrentPassword, profile.SaltKey);
                if (profile != null && isValid)
                {
                    string salt = PasswordHasher.GenerateSalt();
                    profile.Password = PasswordHasher.GeneratePassword(model.NewPassword, salt);
                    profile.SaltKey = salt;
                    //profile.IsLocked = false;
                    //profile.InvalidAttempt = 0;
                    //profile.LockedDateTime = null;
                    await usersService.UpdateAsync(profile);
                    ShowSuccessMessage("Success", "Password has been changed successfully.", false);
                    return NewtonSoftJsonResult(new RequestOutcome<string>
                    {
                        RedirectUrl = Url.Action("index", "home"),
                        IsSuccess = true
                    });
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Current password doesn't match.");
                    return CreateModelStateErrors();
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid requiest");
                return CreateModelStateErrors();
            }
        }

        public async Task<IActionResult> Profile()
        {
            AddPageHeader("Profile", "");

            var user = usersService.GetByUserId(CurrentUser.UserId);
            ProfileDto model = new ProfileDto()
            {
                Lname = user.Lname,
                AvatarUrl = user.AvatarUrl,
                Mname = user.Mname,
                Email = user.Email,
                Fname = user.Fname,
                CompanyId = user.CompanyId,
                MobileNo = user.MobileNo,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                Id = user.Id,
                State = user.State
            };
            return View("_Profile", model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto model)
        {
            if (ModelState.IsValid)
            {

                if (Request.Form != null && Request.Form.Files != null && Request.Form.Files.Any())
                {
                    model.AvatarUrl = await UploadProfile(CurrentUser.CompanyId, Request.Form.Files[0], model.AvatarUrl);
                }


                var profile = await usersService.GetById(CurrentUser.UserId);

                profile.Fname = model.Fname;
                profile.Lname = model.Lname;
                profile.Mname = model.Mname;
                profile.AvatarUrl = model.AvatarUrl;
                profile.Address = model.Address;
                profile.City = profile.City;
                profile.Dob = model.Dob;
                profile.Gender = model.Gender;
                profile.MobileNo = model.MobileNo;
                profile.State = model.State;
                profile.ModifiedDate = DateTime.Now;
                await usersService.UpdateAsync(profile);
                ShowSuccessMessage("Success", "Profile has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string>
                {
                    RedirectUrl = Url.Action("index", "home"),
                    IsSuccess = true,
                    Message = "Profile has been updated successfully."
                });

            }
            else
            {
                ModelState.AddModelError("", "Invalid requiest");
                return CreateModelStateErrors();
            }
        }

        public async Task<IActionResult> SignOut()
        {
            PermissionHelper.RemovePermission(CurrentUser.UserId);
            await RemoveAuthentication();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied()
        {
            if (CurrentUser.RoleId == (int)UserType.SuperAdmin)
            {
                return Redirect(Url.Action("index", "home", new { area = "SuperAdmin" }));
            }
            else if (CurrentUser.RoleId == (int)UserType.Admin)
            {
                return Redirect(Url.Action("index", "home", new { area = "Admin" }));
            }
            else
            {
                return Redirect(Url.Action("index", "home"));
            }
        }
        private async Task<string> UploadProfile(int companyId, IFormFile imageFile, string existingFileName)
        {
            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
            string myfile = "";
            string filePath = "";
            string ext = "";
            string webRoot = "";
            string Directirypath = "";
            string DeleteFilePath = "";

            bool isAnyfile = false;
            if (imageFile != null)
            {
                isAnyfile = true;
            }

            if (isAnyfile == true)
            {
                try
                {
                    webRoot = env.WebRootPath;
                    if (!Directory.Exists(webRoot + "/DYF/" + companyId + "/EmployeeImages/"))
                    {
                        Directory.CreateDirectory(webRoot + "/DYF/" + companyId + "/EmployeeImages/");
                    }
                    Directirypath = webRoot + "/DYF/" + companyId + "/EmployeeImages/";

                    if (imageFile != null)
                    {
                        ext = Path.GetExtension(imageFile.FileName).ToLower();
                        if (allowedExtensions.Contains(ext))
                        {
                            myfile = CurrentUser.UserId + ext;
                            filePath = Path.Combine(Directirypath, myfile);
                            if (!string.IsNullOrEmpty(existingFileName))
                            {
                                //To Delete existing image
                                DeleteFilePath = Path.Combine(Directirypath, existingFileName);
                                FileInfo file = new FileInfo(DeleteFilePath);
                                if (file.Exists)//check file exsit or not
                                {
                                    file.Delete();
                                }
                            }
                            using (var streams = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(streams);

                            }

                        }
                    }
                }
                catch
                {
                    myfile = "";
                }
            }

            return myfile;
        }



    }
}

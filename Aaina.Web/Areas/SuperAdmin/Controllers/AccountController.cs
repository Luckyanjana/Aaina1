using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code.LIBS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Areas.SuperAdmin.Controllers
{
    public class AccountController : BaseController
    {
        // GET: /<controller>/
        private readonly IUserLoginService usersService;
        private readonly IHostingEnvironment env;


        public AccountController(IUserLoginService _usersService, IHostingEnvironment _env)
        {
            this.usersService = _usersService;
            this.env = _env;
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
                        RedirectUrl = Url.Action("index", "home", new { area = "admin" }),
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
            await RemoveAuthentication();
            return RedirectToAction("Login","Account", new { area = "" });
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
                    if (!Directory.Exists(webRoot + "/" + companyId + "/EmployeeImages/"))
                    {
                        Directory.CreateDirectory(webRoot + "/" + companyId + "/EmployeeImages/");
                    }
                    Directirypath = webRoot + "/" + companyId + "/EmployeeImages/";

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
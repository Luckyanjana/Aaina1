using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Dto;
using Aaina.Web.Models.Others;
using Aaina.Web.Models.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
//using Newtonsoft.Json;


namespace Aaina.Web.Controllers
{

    //[Authorize(Roles = "User")]
    public class BaseController : Controller
    {

        public CustomPrincipal CurrentUser => new CustomPrincipal(HttpContext.User);
        public CustomMenuPermission CurrentMenuPermission => new CustomMenuPermission(HttpContext.Items);
        public async Task CreateAuthenticationTicket(UserLogin user)
        {

            if (user != null)
            {
                var claims = new List<Claim>{
                        new Claim(ClaimTypes.Email, user.Email??""),
                        new Claim(ClaimTypes.Name,$"{user.Fname} {user.Lname}"??""),
                        new Claim(ClaimTypes.PrimarySid,Convert.ToString(user.Id)),
                        new Claim(ClaimTypes.Sid,Convert.ToString(user.CompanyId)),
                        new Claim(ClaimTypes.Role,((UserType)user.UserType).ToString()),
                        new Claim("roleId",Convert.ToString(user.UserType)),
                        new Claim("avatar",!string.IsNullOrEmpty(user.AvatarUrl)?$"/DYF/{user.CompanyId}/EmployeeImages/{user.AvatarUrl}":"/img/Default_avatar.jpg")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {

                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

            }


        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login"
                }));
            }

            if (CurrentUser.IsAuthenticated && CurrentUser.RoleId == (int)UserType.User)
            {
                bool CheckValue = !IsAjax(filterContext);
                string actionName = Convert.ToString(filterContext.RouteData.Values["action"]).ToLower();
                string controllerName = Convert.ToString(filterContext.RouteData.Values["Controller"]).ToLower();
                if (actionName == "delete" || actionName == "add" || actionName == "edit")
                {
                    CheckValue = true;
                }

                if (CheckValue && filterContext.HttpContext.Request.Method != "POST")
                {
                    filterContext.HttpContext.Items.Remove("IsList");
                    filterContext.HttpContext.Items.Remove("IsView");
                    filterContext.HttpContext.Items.Remove("IsAdd");
                    filterContext.HttpContext.Items.Remove("IsEdit");
                    filterContext.HttpContext.Items.Remove("IsDelete");

                    var menuDetails = PermissionHelper.GetPermission(CurrentUser.UserId);
                    LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menuDetails);
                    List<MenuPermissionListDto> permissionMenu = new List<MenuPermissionListDto>();
                    if (leftMenu != null && leftMenu.LeftMenuStatic != null && leftMenu.LeftMenuStatic.Any())
                    {
                        foreach (var item in leftMenu.LeftMenuStatic)
                        {
                            permissionMenu.AddRange(item.ChildMenu);
                        }
                    }

                    if (permissionMenu.Any(x => x.Controller.ToLower() == controllerName))
                    {
                        var permision =permissionMenu.FirstOrDefault(x => x.IsActive && x.Controller.ToLower() == controllerName);
                        if (permision != null)
                        {
                            filterContext.HttpContext.Items.Add("IsList", permision.IsList);
                            filterContext.HttpContext.Items.Add("IsView", permision.IsView);
                            filterContext.HttpContext.Items.Add("IsAdd", permision.IsAdd);
                            filterContext.HttpContext.Items.Add("IsEdit", permision.IsEdit);
                            filterContext.HttpContext.Items.Add("IsDelete", permision.IsDelete);
                        }
                        else
                        {
                            filterContext.HttpContext.Items.Add("IsList", false);
                            filterContext.HttpContext.Items.Add("IsView", false);
                            filterContext.HttpContext.Items.Add("IsAdd", false);
                            filterContext.HttpContext.Items.Add("IsEdit", false);
                            filterContext.HttpContext.Items.Add("IsDelete", false);
                        }
                        bool isValidRequest = false;
                        if (permissionMenu != null)
                        {
                            switch (actionName)
                            {
                                case "index":
                                    isValidRequest = permissionMenu.Any(x => x.Controller.ToLower() == controllerName && x.IsList);
                                    break;
                                case "view":
                                    isValidRequest = permissionMenu.Any(x => x.Controller.ToLower() == controllerName && x.IsView);
                                    break;
                                case "add":
                                    isValidRequest = permissionMenu.Any(x => x.Controller.ToLower() == controllerName && x.IsAdd);
                                    break;
                                case "edit":
                                    isValidRequest = permissionMenu.Any(x => x.Controller.ToLower() == controllerName && x.IsEdit);
                                    break;
                                case "delete":
                                    isValidRequest = permissionMenu.Any(x => x.Controller.ToLower() == controllerName && x.IsDelete);
                                    break;
                                default:
                                    isValidRequest = true;
                                    break;
                            }
                        }
                        
                        if (!isValidRequest)
                        {
                            filterContext.Result = new RedirectToRouteResult("gameRoute",new RouteValueDictionary(new
                            {
                                controller = "error",
                                action = "accessdenied",
                                tenant = filterContext.RouteData.Values["tenant"]

                            }));
                        }
                    }
                    else
                    {
                        filterContext.HttpContext.Items.Add("IsList", true);
                        filterContext.HttpContext.Items.Add("IsView", true);
                        filterContext.HttpContext.Items.Add("IsAdd", true);
                        filterContext.HttpContext.Items.Add("IsEdit", true);
                        filterContext.HttpContext.Items.Add("IsDelete", true);
                    }

                }
            }
            else
            {
                filterContext.HttpContext.Items.Remove("IsList");
                filterContext.HttpContext.Items.Remove("IsView");
                filterContext.HttpContext.Items.Remove("IsAdd");
                filterContext.HttpContext.Items.Remove("IsEdit");
                filterContext.HttpContext.Items.Remove("IsDelete");

                filterContext.HttpContext.Items.Add("IsList", true);
                filterContext.HttpContext.Items.Add("IsView", true);
                filterContext.HttpContext.Items.Add("IsAdd", true);
                filterContext.HttpContext.Items.Add("IsEdit", true);
                filterContext.HttpContext.Items.Add("IsDelete", true);
            }
        }

        protected void ShowSuccessMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Success, isCurrentView);
        }

        protected void ShowErrorMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Danger, isCurrentView);
        }

        private void ShowMessages(string title, string message, MessageType messageType, bool isCurrentView)
        {
            Notification model = new Notification
            {
                Heading = title,
                Message = message,
                Type = messageType
            };

            if (isCurrentView)
                this.ViewData.AddOrReplace("NotificationModel", model);
            else
            {
                this.TempData["NotificationModel"] = JsonConvert.SerializeObject(model);
                TempData.Keep("NotificationModel");
            }
        }

        internal void AddPageHeader(string pageHeader = "", string pageDescription = "", string belowHeader = "")
        {
            ViewBag.PageHeader = Tuple.Create(pageHeader, pageDescription, belowHeader);
        }

        public PartialViewResult CreateModelStateErrors()
        {
            return PartialView("_ValidationSummary", ModelState.Values.SelectMany(x => x.Errors));
        }
        #region "Serialization"

        public IActionResult NewtonSoftJsonResult(object data)
        {
            return Json(data);
        }

        #endregion

        public async Task RemoveAuthentication()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        protected async Task<string> UploadProfile(IHostingEnvironment env, int companyId, IFormFile imageFile, string existingFileName, string folderName, string[] allowedExtensions)
        {

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
                    webRoot = env.WebRootPath + "/DYF";
                    if (!Directory.Exists($"{webRoot}/{companyId}/{folderName}/"))
                    {
                        Directory.CreateDirectory($"{webRoot}/{companyId}/{folderName}/");
                    }
                    Directirypath = $"{webRoot}/{companyId}/{folderName}/";

                    if (imageFile != null)
                    {
                        ext = Path.GetExtension(imageFile.FileName).ToLower();
                        if (allowedExtensions.Contains(ext))
                        {
                            myfile = Guid.NewGuid().ToString() + ext;
                            filePath = Path.Combine(Directirypath, myfile);
                            if (!string.IsNullOrEmpty(existingFileName))
                            {
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

        protected async Task<string> UploadProfile(IHostingEnvironment env, int companyId, IFormFile imageFile, string existingFileName, string folderName, string[] allowedExtensions, string fileName = "")
        {

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
                    webRoot = env.WebRootPath + "/DYF";
                    if (!Directory.Exists($"{webRoot}/{companyId}/{folderName}/"))
                    {
                        Directory.CreateDirectory($"{webRoot}/{companyId}/{folderName}/");
                    }
                    Directirypath = $"{webRoot}/{companyId}/{folderName}/";

                    if (imageFile != null)
                    {
                        ext = Path.GetExtension(imageFile.FileName).ToLower();
                        if (allowedExtensions.Contains(ext))
                        {
                            myfile = (string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString() : fileName) + ext;
                            filePath = Path.Combine(Directirypath, myfile);
                            if (!string.IsNullOrEmpty(existingFileName))
                            {
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

                            if (fileName.Contains("mini"))
                            {
                                Image img = Image.FromFile(filePath);
                                Image thumbnail = img.GetThumbnailImage(21, 20, null, IntPtr.Zero);
                                img.Dispose();
                                thumbnail.Save(filePath);
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
        protected string GetEmojiName(List<WeightageDto> emojiList, double rating)
        {
            string emoji = "1.png";
            emoji = emojiList.Any(a => a.Rating == rating) ? emojiList.FirstOrDefault(a => a.Rating == rating).Emoji : emoji;
            return emoji;
        }
        protected string GetEmojiNameMini(List<WeightageDto> emojiList, double rating)
        {
            string emoji = "1-mini.png";
            if (emojiList.Any(a => a.Rating == rating))
            {
               var emoji1 = emojiList.FirstOrDefault(a => a.Rating == rating).Emoji.Split(".");
                emoji = $"{emoji1[0].Replace("-mini",string.Empty)}-mini.{emoji1[1]}";
            }

            return emoji;
        }
        protected string GetEmojiNameMiniFromName(string emoji)
        {
            emoji = $"{emoji.Split('.')[0]}-mini.{emoji.Split('.')[1]}";
            return emoji;
        }

        private bool IsAjax(ActionExecutingContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}

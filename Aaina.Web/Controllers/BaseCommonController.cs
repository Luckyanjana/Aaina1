using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Web.Models.Others;
using Aaina.Web.Models.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Aaina.Web.Controllers
{
    public class BaseCommonController : Controller
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
                        new Claim("playerType",Convert.ToString(user.PlayerType)),
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

        public async Task RemoveAuthentication()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
    }
}
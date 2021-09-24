using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Aaina.Web.Areas.SuperAdmin.Controllers
{
    
    [Authorize(Roles = "SuperAdmin")]
    [Area("SuperAdmin")]
    public class BaseController : Controller
    {
        public CustomPrincipal CurrentUser => new CustomPrincipal(HttpContext.User);
        public ClaimsPrincipal LoggedinUser => HttpContext.User;

       

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

        internal void AddPageHeader(string pageHeader = "", string pageDescription = "")
        {
            ViewBag.PageHeader = Tuple.Create(pageHeader, pageDescription,"");
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
    }
}
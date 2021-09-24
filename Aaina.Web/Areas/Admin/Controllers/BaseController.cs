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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Aaina.Web.Areas.Admin.Controllers
{

    [Authorize(Roles = "Admin")]
    [Area("Admin")]
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
                emoji = emojiList.FirstOrDefault(a => a.Rating == rating).Emoji;
                emoji = $"{emoji.Split('.')[0]}-mini.{emoji.Split('.')[1]}";
            }

            return emoji;
        }
        protected string GetEmojiNameMiniFromName(string emoji)
        {
            emoji = $"{emoji.Split('.')[0]}-mini.{emoji.Split('.')[1]}";
            return emoji;
        }
    }
}
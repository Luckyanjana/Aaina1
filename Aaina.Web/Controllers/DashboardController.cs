using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aaina.Web.Models;
using Aaina.Service;
using Aaina.Dto;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Aaina.Common;
using Dropbox.Api;
using Newtonsoft.Json;

namespace Aaina.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IPushNotificationService pushNotificationService;
        private readonly IGameService gameService;
        private readonly INotificationService notificationService;
        private readonly ISessionService sessionService;
        private readonly IGoogleDriveService googleDriveService;
        private readonly IDropBoxService dropBoxService;
        private readonly IWeightageService weightageService;
        public DashboardController(IPushNotificationService pushNotificationService,
            IGameService gameService, INotificationService notificationService,
            ISessionService sessionService, IGoogleDriveService googleDriveService, IDropBoxService dropBoxService,
            IWeightageService weightageService)
        {
            this.pushNotificationService = pushNotificationService;
            this.gameService = gameService;
            this.notificationService = notificationService;
            this.sessionService = sessionService;
            this.googleDriveService = googleDriveService;
            this.dropBoxService = dropBoxService;
            this.weightageService = weightageService;
        }


        public async Task<IActionResult> Index(int tenant)
        {
            var menu = PermissionHelper.GetPermission(CurrentUser.UserId);
            LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);
            if (string.IsNullOrEmpty(menu) || leftMenu == null || leftMenu.LeftMenu == null || !leftMenu.LeftMenu.Any())
            {
                leftMenu = new LeftMenuDto();
                var gamemenu = await gameService.GetMenuNLevel(CurrentUser.CompanyId);
                leftMenu.LeftMenu = gamemenu;
                leftMenu.EmojiList = weightageService.GetAllActive(CurrentUser.CompanyId);
            }
            leftMenu.LeftMenuStatic = new List<MenuPermissionListDto>();
            PermissionHelper.SetPermission(JsonConvert.SerializeObject(leftMenu), CurrentUser.UserId);

            var model = await gameService.GetById(tenant);
            ViewBag.Name = model.Name;
            return View();
        }





    }
}

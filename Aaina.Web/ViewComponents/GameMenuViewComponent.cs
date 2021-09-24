using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Aaina.Web.ViewComponents
{
    public class GameMenuViewComponent : ViewComponent
    {
        //private readonly IGameService service;
        //private readonly IWeightageService weightageService;
        //public GameMenuViewComponent(IGameService _service, IWeightageService weightageService)
        //{
        //    this.service = _service;
        //    this.weightageService = weightageService;
        //}

        public async Task<IViewComponentResult> InvokeAsync(int companyId,int userId, int gameId,int userType)
        {
            var items = await GetItemsAsync(companyId, userId, gameId, userType);
            return View(items);
        }
        private async Task<LeftMenuDto> GetItemsAsync(int companyId, int userId, int gameId,int userType)
        {   
            var menu = PermissionHelper.GetPermission(userId);
            LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);

            //var gamemenu = await service.GetMenu(companyId);
            //leftMenu.LeftMenu = gamemenu;
            //if (userType == (int)UserType.User)
            //{
            //    leftMenu.LeftMenuStatic = await service.GetMenuStatic(userId, gameId);
            //}
            //leftMenu.EmojiList = weightageService.GetAllActive(companyId);
            return leftMenu;
        }
    }
}

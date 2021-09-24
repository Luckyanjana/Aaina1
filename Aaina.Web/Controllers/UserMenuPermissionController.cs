using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Controllers
{
    public class UserMenuPermissionController : BaseController
    {

        private readonly IUserMenuPermissionService _service;
        private readonly IUserLoginService _roleService;
        private readonly IMenuService _menuService;
        public UserMenuPermissionController(IUserMenuPermissionService service, IUserLoginService roleService, IMenuService menuService)
        {
            this._service = service;
            this._roleService = roleService;
            this._menuService = menuService;
        }

        public IActionResult Index(int? userId)
        {
            ViewBag.roleId = userId;
            List<MenuPermissionDto> model = new List<MenuPermissionDto>();
            ViewBag.RolesList = _roleService.GetAllDrop(CurrentUser.CompanyId,null);
            if (userId.HasValue)
            {
                model = _service.GetByUserIdAll(userId.Value);
            }

            return View(model);
        }

        [HttpPost]
        public void Save(List<UserMenuPermissionDto> model)
        {
            int userId = model.FirstOrDefault().UserId;
            model = model.Where(a => a.IsAdd || a.IsList || a.IsView || a.IsEdit || a.IsDelete || a.IsMain).ToList();
            ShowSuccessMessage("Success!", "Saved Successfully", false);
            _service.Save(model,CurrentUser.UserId, userId);

        }
    }
}

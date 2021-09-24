using Aaina.Dto;

using Aaina.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class RoleMenuPermissionController : BaseController
    {

        private readonly IRoleMenuPermissionService _roleMenuPermissionService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        public RoleMenuPermissionController(IRoleMenuPermissionService roleMenuPermissionService, IRoleService roleService, IMenuService menuService)
        {
            this._roleMenuPermissionService = roleMenuPermissionService;
            this._roleService = roleService;
            this._menuService = menuService;
        }

        public IActionResult Index(int? roleId)
        {
            ViewBag.roleId = roleId;
            List<MenuPermissionDto> model = new List<MenuPermissionDto>();
            ViewBag.RolesList = _roleService.GetAll(CurrentUser.CompanyId);
            if (roleId.HasValue)
            {
                model = _roleMenuPermissionService.GetByRoleIdAll(roleId.Value);
            }

            return View(model);
        }

        [HttpPost]
        public void Save(List<RoleMenuPermissionDto> model)
        {
            int roleId = model.FirstOrDefault().RoleId;
            model = model.Where(a => a.IsAdd || a.IsList || a.IsView || a.IsEdit || a.IsDelete || a.IsMain).ToList();
            ShowSuccessMessage("Success!", "Saved Successfully", false);
            _roleMenuPermissionService.Save(model,CurrentUser.UserId, roleId);

        }
    }
}

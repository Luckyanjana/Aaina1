using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class MenuDto
    {
        public MenuDto()
        {
            this.ChildMenu = new List<MenuDto>();
            this.ParentMenuList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
       
        public string ParentName { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsMain { get; set; }
        public int? ParentId { get; set; }
        public int? Order { get; set; }
        public bool IsActive { get; set; }       
        public List<MenuDto> ChildMenu { get; set; }
        public List<SelectedItemDto> ParentMenuList { get; set; }
    }

    public class MenuPermissionDto
    {
        public MenuPermissionDto()
        {
            this.ChildMenu = new List<MenuPermissionDto>();
        }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public int? ParentId { get; set; }
        public int? Order { get; set; }
        public bool IsView { get; set; }
        public bool IsList { get; set; }
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
        public bool IsEdit { get; set; }
        public List<MenuPermissionDto> ChildMenu { get; set; }
    }
    public class MenuPermissionListDto
    {
        public MenuPermissionListDto()
        {
            this.ChildMenu = new List<MenuPermissionListDto>();
        }
       
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public int? Order { get; set; }
        public bool IsView { get; set; }
        public bool IsList { get; set; }
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
        public bool IsEdit { get; set; }
        public List<MenuPermissionListDto> ChildMenu { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class RoleMenuPermissionDto
    {
        public int MenuId { get; set; }
        public int RoleId { get; set; }        
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsList { get; set; }
        public bool IsView { get; set; }
        public bool IsMain { get; set; }
    }

    public class UserMenuPermissionDto
    {
        public int MenuId { get; set; }
        public int UserId { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsList { get; set; }
        public bool IsView { get; set; }
        public bool IsMain { get; set; }
        public int CreatedBy { get; set; }
    }
}

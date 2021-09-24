using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto.UserMenuPermission
{
    public class UserMenuPermissionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public bool CanbeAdd { get; set; }
        public bool CanbeEdit { get; set; }
        public bool CanbeDelete { get; set; }
        public bool CanbeView { get; set; }
        public bool CanbeGive { get; set; }

    }
}

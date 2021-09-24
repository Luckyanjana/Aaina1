using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class UserMenuPermission
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? MenuId { get; set; }
        public bool IsView { get; set; }
        public bool IsList { get; set; }
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
        public bool IsEdit { get; set; }
        public int CreatedBy { get; set; }
    }
}

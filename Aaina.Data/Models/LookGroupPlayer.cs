using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class LookGroupPlayer
    {
        public int Id { get; set; }
        public int LookGroupId { get; set; }
        public int UserId { get; set; }

        public virtual LookGroup LookGroup { get; set; }
        public virtual UserLogin User { get; set; }
    }
}

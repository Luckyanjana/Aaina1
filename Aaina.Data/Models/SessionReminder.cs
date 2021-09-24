using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class SessionReminder
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int TypeId { get; set; }
        public int Every { get; set; }
        public int Unit { get; set; }

        public virtual Session Session { get; set; }
    }
}

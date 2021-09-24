using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class NotificationReminder
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public string Reason { get; set; }
        public int? SendBy { get; set; }
        public int SendTo { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }

        public virtual UserLogin SendByNavigation { get; set; }
        public virtual UserLogin SendToNavigation { get; set; }
    }
}

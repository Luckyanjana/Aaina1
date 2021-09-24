using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportGiveDetails
    {
        public ReportGiveDetails()
        {
            ReportGiveAttributeValue = new HashSet<ReportGiveAttributeValue>();
        }

        public int Id { get; set; }
        public int ReportGiveId { get; set; }
        public int? EntityId { get; set; }
        public string Remark { get; set; }
        public int? RemarkedBy { get; set; }

        public virtual UserLogin RemarkedByNavigation { get; set; }
        public virtual ReportGive ReportGive { get; set; }
        public virtual ICollection<ReportGiveAttributeValue> ReportGiveAttributeValue { get; set; }
    }
}

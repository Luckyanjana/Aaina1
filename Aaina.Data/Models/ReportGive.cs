using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportGive
    {
        public ReportGive()
        {
            ReportGiveDetails = new HashSet<ReportGiveDetails>();
        }

        public int Id { get; set; }
        public int ReportId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Remark { get; set; }
        public int? RemarkedBy { get; set; }
        public int? AssignToType { get; set; }

        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual UserLogin RemarkedByNavigation { get; set; }
        public virtual ReportTemplate Report { get; set; }
        public virtual ICollection<ReportGiveDetails> ReportGiveDetails { get; set; }
    }
}

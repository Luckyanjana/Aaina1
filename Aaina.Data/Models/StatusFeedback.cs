using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class StatusFeedback
    {
        public StatusFeedback()
        {
            StatusFeedbackDetail = new HashSet<StatusFeedbackDetail>();
        }

        public int Id { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ActualTime { get; set; }

        public virtual Status Status { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual ICollection<StatusFeedbackDetail> StatusFeedbackDetail { get; set; }
    }
}

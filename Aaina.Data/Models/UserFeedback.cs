using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class UserFeedback
    {
        public UserFeedback()
        {
            UserFeedbackDetails = new HashSet<UserFeedbackDetails>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int LookId { get; set; }
        public int UserId { get; set; }
        public bool IsDraft { get; set; }
        public DateTime FeedbackDate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual Look Look { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual ICollection<UserFeedbackDetails> UserFeedbackDetails { get; set; }
    }
}

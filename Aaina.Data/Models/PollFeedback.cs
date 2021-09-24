using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PollFeedback
    {
        public PollFeedback()
        {
            PollQuestionFeedback = new HashSet<PollQuestionFeedback>();
        }

        public int Id { get; set; }
        public int PollId { get; set; }
        public int UserId { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Poll Poll { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual ICollection<PollQuestionFeedback> PollQuestionFeedback { get; set; }
    }
}

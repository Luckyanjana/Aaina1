using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Poll
    {
        public Poll()
        {
            PollFeedback = new HashSet<PollFeedback>();
            PollParticipants = new HashSet<PollParticipants>();
            PollQuestion = new HashSet<PollQuestion>();
            PollReminder = new HashSet<PollReminder>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual Game Game { get; set; }
        public virtual Game SubGame { get; set; }
        public virtual PollScheduler PollScheduler { get; set; }
        public virtual ICollection<PollFeedback> PollFeedback { get; set; }
        public virtual ICollection<PollParticipants> PollParticipants { get; set; }
        public virtual ICollection<PollQuestion> PollQuestion { get; set; }
        public virtual ICollection<PollReminder> PollReminder { get; set; }
    }
}

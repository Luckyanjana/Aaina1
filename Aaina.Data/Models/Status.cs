using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Status
    {
        public Status()
        {
            StatusFeedback = new HashSet<StatusFeedback>();
            StatusGameBy = new HashSet<StatusGameBy>();
            StatusReminder = new HashSet<StatusReminder>();
            StatusSnooze = new HashSet<StatusSnooze>();
            StatusTeamBy = new HashSet<StatusTeamBy>();
            StatusTeamFor = new HashSet<StatusTeamFor>();
            StatusUserBy = new HashSet<StatusUserBy>();
            StatusUserFor = new HashSet<StatusUserFor>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? EstimatedTime { get; set; }

        public virtual Company Company { get; set; }
        public virtual Game Game { get; set; }
        public virtual StatusScheduler StatusScheduler { get; set; }
        public virtual ICollection<StatusFeedback> StatusFeedback { get; set; }
        public virtual ICollection<StatusGameBy> StatusGameBy { get; set; }
        public virtual ICollection<StatusReminder> StatusReminder { get; set; }
        public virtual ICollection<StatusSnooze> StatusSnooze { get; set; }
        public virtual ICollection<StatusTeamBy> StatusTeamBy { get; set; }
        public virtual ICollection<StatusTeamFor> StatusTeamFor { get; set; }
        public virtual ICollection<StatusUserBy> StatusUserBy { get; set; }
        public virtual ICollection<StatusUserFor> StatusUserFor { get; set; }
    }
}

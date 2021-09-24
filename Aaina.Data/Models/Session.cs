using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Session
    {
        public Session()
        {
            PostSession = new HashSet<PostSession>();
            PreSession = new HashSet<PreSession>();
            PreSessionDelegate = new HashSet<PreSessionDelegate>();
            PreSessionGroup = new HashSet<PreSessionGroup>();
            PreSessionStatus = new HashSet<PreSessionStatus>();
            SessionParticipant = new HashSet<SessionParticipant>();
            SessionReminder = new HashSet<SessionReminder>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int TypeId { get; set; }
        public int ModeId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public int? Deadline { get; set; }

        public virtual Company Company { get; set; }
        public virtual Game Game { get; set; }
        public virtual SessionScheduler SessionScheduler { get; set; }
        public virtual ICollection<PostSession> PostSession { get; set; }
        public virtual ICollection<PreSession> PreSession { get; set; }
        public virtual ICollection<PreSessionDelegate> PreSessionDelegate { get; set; }
        public virtual ICollection<PreSessionGroup> PreSessionGroup { get; set; }
        public virtual ICollection<PreSessionStatus> PreSessionStatus { get; set; }
        public virtual ICollection<SessionParticipant> SessionParticipant { get; set; }
        public virtual ICollection<SessionReminder> SessionReminder { get; set; }
    }
}

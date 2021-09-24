using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Team
    {
        public Team()
        {
            LookTeam = new HashSet<LookTeam>();
            StatusTeamBy = new HashSet<StatusTeamBy>();
            StatusTeamFor = new HashSet<StatusTeamFor>();
            TeamFeedbackDetails = new HashSet<TeamFeedbackDetails>();
            TeamPlayer = new HashSet<TeamPlayer>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public double Weightage { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual Game Game { get; set; }
        public virtual ICollection<LookTeam> LookTeam { get; set; }
        public virtual ICollection<StatusTeamBy> StatusTeamBy { get; set; }
        public virtual ICollection<StatusTeamFor> StatusTeamFor { get; set; }
        public virtual ICollection<TeamFeedbackDetails> TeamFeedbackDetails { get; set; }
        public virtual ICollection<TeamPlayer> TeamPlayer { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Look
    {
        public Look()
        {
            GameFeedback = new HashSet<GameFeedback>();
            LookAttribute = new HashSet<LookAttribute>();
            LookGame = new HashSet<LookGame>();
            LookGroup = new HashSet<LookGroup>();
            LookPlayers = new HashSet<LookPlayers>();
            LookSubAttribute = new HashSet<LookSubAttribute>();
            LookTeam = new HashSet<LookTeam>();
            LookUser = new HashSet<LookUser>();
            TeamFeedback = new HashSet<TeamFeedback>();
            UserFeedback = new HashSet<UserFeedback>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CalculationType { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsSchedule { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual Game Game { get; set; }
        public virtual LookScheduler LookScheduler { get; set; }
        public virtual ICollection<GameFeedback> GameFeedback { get; set; }
        public virtual ICollection<LookAttribute> LookAttribute { get; set; }
        public virtual ICollection<LookGame> LookGame { get; set; }
        public virtual ICollection<LookGroup> LookGroup { get; set; }
        public virtual ICollection<LookPlayers> LookPlayers { get; set; }
        public virtual ICollection<LookSubAttribute> LookSubAttribute { get; set; }
        public virtual ICollection<LookTeam> LookTeam { get; set; }
        public virtual ICollection<LookUser> LookUser { get; set; }
        public virtual ICollection<TeamFeedback> TeamFeedback { get; set; }
        public virtual ICollection<UserFeedback> UserFeedback { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Company
    {
        public Company()
        {
            Attribute = new HashSet<Attribute>();
            Filter = new HashSet<Filter>();
            FormBuilder = new HashSet<FormBuilder>();
            Game = new HashSet<Game>();
            GameFeedback = new HashSet<GameFeedback>();
            GamePlayer = new HashSet<GamePlayer>();
            Look = new HashSet<Look>();
            Play = new HashSet<Play>();
            Poll = new HashSet<Poll>();
            ReportTemplate = new HashSet<ReportTemplate>();
            Role = new HashSet<Role>();
            Session = new HashSet<Session>();
            Status = new HashSet<Status>();
            Team = new HashSet<Team>();
            TeamFeedback = new HashSet<TeamFeedback>();
            TeamPlayer = new HashSet<TeamPlayer>();
            UserFeedback = new HashSet<UserFeedback>();
            UserLogin = new HashSet<UserLogin>();
            Weightage = new HashSet<Weightage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string DriveId { get; set; }

        public virtual ICollection<Attribute> Attribute { get; set; }
        public virtual ICollection<Filter> Filter { get; set; }
        public virtual ICollection<FormBuilder> FormBuilder { get; set; }
        public virtual ICollection<Game> Game { get; set; }
        public virtual ICollection<GameFeedback> GameFeedback { get; set; }
        public virtual ICollection<GamePlayer> GamePlayer { get; set; }
        public virtual ICollection<Look> Look { get; set; }
        public virtual ICollection<Play> Play { get; set; }
        public virtual ICollection<Poll> Poll { get; set; }
        public virtual ICollection<ReportTemplate> ReportTemplate { get; set; }
        public virtual ICollection<Role> Role { get; set; }
        public virtual ICollection<Session> Session { get; set; }
        public virtual ICollection<Status> Status { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<TeamFeedback> TeamFeedback { get; set; }
        public virtual ICollection<TeamPlayer> TeamPlayer { get; set; }
        public virtual ICollection<UserFeedback> UserFeedback { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
        public virtual ICollection<Weightage> Weightage { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Game
    {
        public Game()
        {
            Attribute = new HashSet<Attribute>();
            Filter = new HashSet<Filter>();
            GameFeedbackDetails = new HashSet<GameFeedbackDetails>();
            GameLocation = new HashSet<GameLocation>();
            GamePlayer = new HashSet<GamePlayer>();
            InverseParent = new HashSet<Game>();
            Look = new HashSet<Look>();
            LookGame = new HashSet<LookGame>();
            PlayGame = new HashSet<Play>();
            PlaySubGame = new HashSet<Play>();
            PollGame = new HashSet<Poll>();
            PollSubGame = new HashSet<Poll>();
            ReportTemplate = new HashSet<ReportTemplate>();
            Session = new HashSet<Session>();
            Status = new HashSet<Status>();
            StatusFeedbackDetailGame = new HashSet<StatusFeedbackDetail>();
            StatusFeedbackDetailSubGame = new HashSet<StatusFeedbackDetail>();
            StatusGameBy = new HashSet<StatusGameBy>();
            Team = new HashSet<Team>();
            WeightagePreset = new HashSet<WeightagePreset>();
            WeightagePresetDetails = new HashSet<WeightagePresetDetails>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public double Weightage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }
        public string ClientName { get; set; }
        public bool ApplyForChild { get; set; }
        public string Location { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual Game Parent { get; set; }
        public virtual ICollection<Attribute> Attribute { get; set; }
        public virtual ICollection<Filter> Filter { get; set; }
        public virtual ICollection<GameFeedbackDetails> GameFeedbackDetails { get; set; }
        public virtual ICollection<GameLocation> GameLocation { get; set; }
        public virtual ICollection<GamePlayer> GamePlayer { get; set; }
        public virtual ICollection<Game> InverseParent { get; set; }
        public virtual ICollection<Look> Look { get; set; }
        public virtual ICollection<LookGame> LookGame { get; set; }
        public virtual ICollection<Play> PlayGame { get; set; }
        public virtual ICollection<Play> PlaySubGame { get; set; }
        public virtual ICollection<Poll> PollGame { get; set; }
        public virtual ICollection<Poll> PollSubGame { get; set; }
        public virtual ICollection<ReportTemplate> ReportTemplate { get; set; }
        public virtual ICollection<Session> Session { get; set; }
        public virtual ICollection<Status> Status { get; set; }
        public virtual ICollection<StatusFeedbackDetail> StatusFeedbackDetailGame { get; set; }
        public virtual ICollection<StatusFeedbackDetail> StatusFeedbackDetailSubGame { get; set; }
        public virtual ICollection<StatusGameBy> StatusGameBy { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<WeightagePreset> WeightagePreset { get; set; }
        public virtual ICollection<WeightagePresetDetails> WeightagePresetDetails { get; set; }
    }
}

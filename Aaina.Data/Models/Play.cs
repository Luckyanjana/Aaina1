using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Play
    {
        public Play()
        {
            InverseParent = new HashSet<Play>();
            PlayDelegate = new HashSet<PlayDelegate>();
            PlayFeedback = new HashSet<PlayFeedback>();
            PlayPersonInvolved = new HashSet<PlayPersonInvolved>();
            PostSessionAgenda = new HashSet<PostSessionAgenda>();
            PreSessionAgenda = new HashSet<PreSessionAgenda>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int Type { get; set; }
        public int GameId { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccountableId { get; set; }
        public int? DependancyId { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public string Emotion { get; set; }
        public string Phoemotion { get; set; }
        public int? FeedbackType { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        public bool IsToday { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? ParentId { get; set; }
        public string CoordinateEmotion { get; set; }
        public string DecisionMakerEmotion { get; set; }
        public int GameType { get; set; }
        public bool IsRequested { get; set; }

        public virtual UserLogin Accountable { get; set; }
        public virtual Company Company { get; set; }
        public virtual UserLogin Dependancy { get; set; }
        public virtual Game Game { get; set; }
        public virtual Play Parent { get; set; }
        public virtual Game SubGame { get; set; }
        public virtual ICollection<Play> InverseParent { get; set; }
        public virtual ICollection<PlayDelegate> PlayDelegate { get; set; }
        public virtual ICollection<PlayFeedback> PlayFeedback { get; set; }
        public virtual ICollection<PlayPersonInvolved> PlayPersonInvolved { get; set; }
        public virtual ICollection<PostSessionAgenda> PostSessionAgenda { get; set; }
        public virtual ICollection<PreSessionAgenda> PreSessionAgenda { get; set; }
    }
}

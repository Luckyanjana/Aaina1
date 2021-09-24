using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Attribute
    {
        public Attribute()
        {
            GameFeedbackDetails = new HashSet<GameFeedbackDetails>();
            LookAttribute = new HashSet<LookAttribute>();
            SubAttribute = new HashSet<SubAttribute>();
            TeamFeedbackDetails = new HashSet<TeamFeedbackDetails>();
            UserFeedbackDetails = new HashSet<UserFeedbackDetails>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual Game Game { get; set; }
        public virtual ICollection<GameFeedbackDetails> GameFeedbackDetails { get; set; }
        public virtual ICollection<LookAttribute> LookAttribute { get; set; }
        public virtual ICollection<SubAttribute> SubAttribute { get; set; }
        public virtual ICollection<TeamFeedbackDetails> TeamFeedbackDetails { get; set; }
        public virtual ICollection<UserFeedbackDetails> UserFeedbackDetails { get; set; }
    }
}

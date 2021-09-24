using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class SubAttribute
    {
        public SubAttribute()
        {
            GameFeedbackDetails = new HashSet<GameFeedbackDetails>();
            LookSubAttribute = new HashSet<LookSubAttribute>();
            TeamFeedbackDetails = new HashSet<TeamFeedbackDetails>();
            UserFeedbackDetails = new HashSet<UserFeedbackDetails>();
        }

        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Name { get; set; }
        public double Weightage { get; set; }
        public string Desciption { get; set; }
        public bool IsQuantity { get; set; }
        public int? UnitId { get; set; }

        public virtual Attribute Attribute { get; set; }
        public virtual ICollection<GameFeedbackDetails> GameFeedbackDetails { get; set; }
        public virtual ICollection<LookSubAttribute> LookSubAttribute { get; set; }
        public virtual ICollection<TeamFeedbackDetails> TeamFeedbackDetails { get; set; }
        public virtual ICollection<UserFeedbackDetails> UserFeedbackDetails { get; set; }
    }
}

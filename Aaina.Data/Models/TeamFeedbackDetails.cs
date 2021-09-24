using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class TeamFeedbackDetails
    {
        public int Id { get; set; }
        public int TeamFeedbackId { get; set; }
        public int TeamId { get; set; }
        public int AttributeId { get; set; }
        public int SubAttributeId { get; set; }
        public string Feedback { get; set; }
        public double? Quantity { get; set; }
        public double? Percentage { get; set; }

        public virtual Attribute Attribute { get; set; }
        public virtual SubAttribute SubAttribute { get; set; }
        public virtual Team Team { get; set; }
        public virtual TeamFeedback TeamFeedback { get; set; }
    }
}

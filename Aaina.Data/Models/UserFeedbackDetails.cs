using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class UserFeedbackDetails
    {
        public int Id { get; set; }
        public int UserFeedbackId { get; set; }
        public int UserId { get; set; }
        public int AttributeId { get; set; }
        public int SubAttributeId { get; set; }
        public string Feedback { get; set; }
        public double? Quantity { get; set; }
        public double? Percentage { get; set; }

        public virtual Attribute Attribute { get; set; }
        public virtual SubAttribute SubAttribute { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual UserFeedback UserFeedback { get; set; }
    }
}

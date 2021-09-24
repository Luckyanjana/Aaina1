using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PlayFeedback
    {
        public long PlayFeedbackId { get; set; }
        public int? Emations { get; set; }
        public double? Complition { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public string Comment { get; set; }
        public int? PlayId { get; set; }
        public DateTime? Created { get; set; }
        public int? CretedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual Play Play { get; set; }
    }
}

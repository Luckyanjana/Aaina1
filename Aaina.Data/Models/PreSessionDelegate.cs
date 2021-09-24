using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PreSessionDelegate
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int DecisionMakerId { get; set; }
        public int DelegateId { get; set; }
        public string Description { get; set; }
        public DateTime DelegateDate { get; set; }

        public virtual UserLogin DecisionMaker { get; set; }
        public virtual UserLogin Delegate { get; set; }
        public virtual Session Session { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PreSessionGroup
    {
        public PreSessionGroup()
        {
            PreSessionGroupDetails = new HashSet<PreSessionGroupDetails>();
        }

        public int Id { get; set; }
        public int SessionId { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsComplete { get; set; }

        public virtual Session Session { get; set; }
        public virtual ICollection<PreSessionGroupDetails> PreSessionGroupDetails { get; set; }
    }
}

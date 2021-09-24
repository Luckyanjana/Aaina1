using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PreSession
    {
        public PreSession()
        {
            PreSessionAgenda = new HashSet<PreSessionAgenda>();
            PreSessionParticipant = new HashSet<PreSessionParticipant>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Session Session { get; set; }
        public virtual ICollection<PreSessionAgenda> PreSessionAgenda { get; set; }
        public virtual ICollection<PreSessionParticipant> PreSessionParticipant { get; set; }
    }
}

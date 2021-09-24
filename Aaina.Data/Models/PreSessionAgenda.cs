using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PreSessionAgenda
    {
        public PreSessionAgenda()
        {
            PreSessionAgendaDoc = new HashSet<PreSessionAgendaDoc>();
        }

        public int Id { get; set; }
        public int PreSessionId { get; set; }
        public int PlayId { get; set; }
        public bool IsApproved { get; set; }

        public virtual Play Play { get; set; }
        public virtual PreSession PreSession { get; set; }
        public virtual ICollection<PreSessionAgendaDoc> PreSessionAgendaDoc { get; set; }
    }
}

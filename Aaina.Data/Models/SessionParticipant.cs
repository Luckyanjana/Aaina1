using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class SessionParticipant
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int? UserId { get; set; }
        public int TypeId { get; set; }
        public int ParticipantTyprId { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }

        public virtual Session Session { get; set; }
        public virtual UserLogin User { get; set; }
    }
}

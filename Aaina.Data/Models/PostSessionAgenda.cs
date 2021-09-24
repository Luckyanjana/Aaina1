using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PostSessionAgenda
    {
        public int Id { get; set; }
        public int PostSessionId { get; set; }
        public int PlayId { get; set; }
        public string Remarks { get; set; }

        public virtual Play Play { get; set; }
        public virtual PostSession PostSession { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PlayDelegate
    {
        public int Id { get; set; }
        public int PlayId { get; set; }
        public int AccountableId { get; set; }
        public int DelegateId { get; set; }
        public string Description { get; set; }
        public DateTime DelegateDate { get; set; }

        public virtual UserLogin Accountable { get; set; }
        public virtual UserLogin Delegate { get; set; }
        public virtual Play Play { get; set; }
    }
}

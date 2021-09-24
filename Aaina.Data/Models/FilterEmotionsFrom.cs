using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class FilterEmotionsFrom
    {
        public int Id { get; set; }
        public int FilterId { get; set; }
        public int FromId { get; set; }

        public virtual Filter Filter { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportTemplateGame
    {
        public int Id { get; set; }
        public int ReportTemplateId { get; set; }
        public int GameId { get; set; }

        public virtual ReportTemplate ReportTemplate { get; set; }
    }
}

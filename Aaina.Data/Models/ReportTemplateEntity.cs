using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportTemplateEntity
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int EntityId { get; set; }

        public virtual ReportTemplate Report { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportGiveAttributeValue
    {
        public int Id { get; set; }
        public int ReportGiveDetailId { get; set; }
        public int FormBuilderAttributeId { get; set; }
        public string AttributeValue { get; set; }
        public int? LookUpId { get; set; }

        public virtual FormBuilderAttribute FormBuilderAttribute { get; set; }
        public virtual FormBuilderAttributeLookUp LookUp { get; set; }
        public virtual ReportGiveDetails ReportGiveDetail { get; set; }
    }
}

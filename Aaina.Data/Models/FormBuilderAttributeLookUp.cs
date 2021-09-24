using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class FormBuilderAttributeLookUp
    {
        public FormBuilderAttributeLookUp()
        {
            ReportGiveAttributeValue = new HashSet<ReportGiveAttributeValue>();
        }

        public int Id { get; set; }
        public int FormBuilderAttributeId { get; set; }
        public string OptionName { get; set; }

        public virtual FormBuilderAttribute FormBuilderAttribute { get; set; }
        public virtual ICollection<ReportGiveAttributeValue> ReportGiveAttributeValue { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class FormBuilderAttribute
    {
        public FormBuilderAttribute()
        {
            FormBuilderAttributeLookUp = new HashSet<FormBuilderAttributeLookUp>();
            ReportGiveAttributeValue = new HashSet<ReportGiveAttributeValue>();
        }

        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string AttributeName { get; set; }
        public int DataType { get; set; }
        public bool IsRequired { get; set; }
        public int OrderNo { get; set; }
        public string DbcolumnName { get; set; }

        public virtual FormBuilder FormBuilder { get; set; }
        public virtual ICollection<FormBuilderAttributeLookUp> FormBuilderAttributeLookUp { get; set; }
        public virtual ICollection<ReportGiveAttributeValue> ReportGiveAttributeValue { get; set; }
    }
}

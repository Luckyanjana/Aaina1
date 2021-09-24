using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class FormBuilder
    {
        public FormBuilder()
        {
            FormBuilderAttribute = new HashSet<FormBuilderAttribute>();
            ReportTemplate = new HashSet<ReportTemplate>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual ICollection<FormBuilderAttribute> FormBuilderAttribute { get; set; }
        public virtual ICollection<ReportTemplate> ReportTemplate { get; set; }
    }
}

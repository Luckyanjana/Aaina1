using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ReportTemplate
    {
        public ReportTemplate()
        {
            ReportGive = new HashSet<ReportGive>();
            ReportTemplateEntity = new HashSet<ReportTemplateEntity>();
            ReportTemplateGame = new HashSet<ReportTemplateGame>();
            ReportTemplateReminder = new HashSet<ReportTemplateReminder>();
            ReportTemplateUser = new HashSet<ReportTemplateUser>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int FormBuilderId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int TypeId { get; set; }
        public int? AccountAbilityId { get; set; }
        public int? EntityType { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual FormBuilder FormBuilder { get; set; }
        public virtual Game Game { get; set; }
        public virtual ReportTemplateScheduler ReportTemplateScheduler { get; set; }
        public virtual ICollection<ReportGive> ReportGive { get; set; }
        public virtual ICollection<ReportTemplateEntity> ReportTemplateEntity { get; set; }
        public virtual ICollection<ReportTemplateGame> ReportTemplateGame { get; set; }
        public virtual ICollection<ReportTemplateReminder> ReportTemplateReminder { get; set; }
        public virtual ICollection<ReportTemplateUser> ReportTemplateUser { get; set; }
    }
}

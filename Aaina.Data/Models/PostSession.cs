using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PostSession
    {
        public PostSession()
        {
            PostSessionAgenda = new HashSet<PostSessionAgenda>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Session Session { get; set; }
        public virtual ICollection<PostSessionAgenda> PostSessionAgenda { get; set; }
    }
}

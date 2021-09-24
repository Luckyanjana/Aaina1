using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ChatGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CretedBy { get; set; }
        public DateTime? Created { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}

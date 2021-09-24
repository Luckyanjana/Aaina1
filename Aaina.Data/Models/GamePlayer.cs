using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class GamePlayer
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int RoleId { get; set; }
        public int TypeId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual Game Game { get; set; }
        public virtual Role Role { get; set; }
        public virtual UserLogin User { get; set; }
    }
}

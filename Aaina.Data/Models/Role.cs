using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Role
    {
        public Role()
        {
            GamePlayer = new HashSet<GamePlayer>();
            TeamPlayer = new HashSet<TeamPlayer>();
            WeightagePresetDetails = new HashSet<WeightagePresetDetails>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public double Weightage { get; set; }
        public string Desciption { get; set; }
        public string ColorCode { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int PlayerType { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<GamePlayer> GamePlayer { get; set; }
        public virtual ICollection<TeamPlayer> TeamPlayer { get; set; }
        public virtual ICollection<WeightagePresetDetails> WeightagePresetDetails { get; set; }
    }
}

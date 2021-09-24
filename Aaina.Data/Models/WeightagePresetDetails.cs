using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class WeightagePresetDetails
    {
        public int Id { get; set; }
        public int WeightagePresetId { get; set; }
        public int GameId { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public double Weightage { get; set; }

        public virtual Game Game { get; set; }
        public virtual Role Role { get; set; }
        public virtual UserLogin User { get; set; }
        public virtual WeightagePreset WeightagePreset { get; set; }
    }
}

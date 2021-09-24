using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class WeightagePreset
    {
        public WeightagePreset()
        {
            WeightagePresetDetails = new HashSet<WeightagePresetDetails>();
        }

        public int Id { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public DateTime AddedDate { get; set; }

        public virtual Game Game { get; set; }
        public virtual ICollection<WeightagePresetDetails> WeightagePresetDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class Filter
    {
        public Filter()
        {
            FilterAttributes = new HashSet<FilterAttributes>();
            FilterEmotionsFor = new HashSet<FilterEmotionsFor>();
            FilterEmotionsFrom = new HashSet<FilterEmotionsFrom>();
            FilterEmotionsFromP = new HashSet<FilterEmotionsFromP>();
            FilterPlayers = new HashSet<FilterPlayers>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int EmotionsFor { get; set; }
        public int EmotionsFrom { get; set; }
        public int CalculationType { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int EmotionsFromP { get; set; }
        public bool IsSelf { get; set; }
        public int? CreatedBy { get; set; }
        public int GameId { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserLogin CreatedByNavigation { get; set; }
        public virtual Game Game { get; set; }
        public virtual ICollection<FilterAttributes> FilterAttributes { get; set; }
        public virtual ICollection<FilterEmotionsFor> FilterEmotionsFor { get; set; }
        public virtual ICollection<FilterEmotionsFrom> FilterEmotionsFrom { get; set; }
        public virtual ICollection<FilterEmotionsFromP> FilterEmotionsFromP { get; set; }
        public virtual ICollection<FilterPlayers> FilterPlayers { get; set; }
    }
}

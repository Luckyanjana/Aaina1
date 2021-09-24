using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class GameLocation
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Location { get; set; }

        public virtual Game Game { get; set; }
    }
}

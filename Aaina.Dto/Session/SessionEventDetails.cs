using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class SessionEventDetails
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string GMeetUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Confirmed { get; set; }

        public int Rejected { get; set; }

        public int Pending { get; set; }

        public string Coordinator { get; set; }

        public bool IsCoordinator { get; set; }
        public string Manager { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Aaina.Dto
{
    public class SessionScheduleEventDto
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string Type { get; set; }
        public int Id { get; set; }
    }
}

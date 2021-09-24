using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class PreSessionGroupDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int GameId { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsComplete { get; set; }
    }

    public class PreSessionGroupDetailDto
    {
        public int Id { get; set; }
        public int PreSessionGroupId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class ChatUserListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string ProfileImage { get; set; }
        public string LastMessage { get; set; }
        public int Type { get; set; }
        public int UnreadMessage { get; set; }
        public string SenderName { get; set; }
        public DateTime? SendDate { get; set; }
        public bool IsOnline { get; set; }
        public string UserIds { get; set; }
        public int UserType { get; set; }
        public string Admin { get; set; }
    }
}

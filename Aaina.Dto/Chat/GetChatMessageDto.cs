using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class GetChatMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ProfileImage { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
    }

    public class ChatExportMessageDto
    {
        public ChatExportMessageDto()
        {
            this.ChatList = new List<GetChatMessageDto>();
        }
        public List<GetChatMessageDto> ChatList { get; set; }
        public string ReceiverName { get; set; }
        public string ProfileImage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

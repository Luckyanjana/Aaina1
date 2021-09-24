using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class ChatMessageDto
    {
        public long Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public byte ReceiverType { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
    }
}

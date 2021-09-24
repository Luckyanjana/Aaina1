using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ChatMessage
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

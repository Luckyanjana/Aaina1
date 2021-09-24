using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
   public  class ChatViewModel
    {
        public ChatViewModel()
        {
            users = new List<ChatUserListDto>();
        }

        public List<ChatUserListDto> users { get; set; }
        public string ConnectionId { get; set; }
    }
}

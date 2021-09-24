using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class ChatGroupDto
    {
        public ChatGroupDto()
        {
            this.UserList = new List<int>();
        }        
        public string Name { get; set; }
        public int CretedBy { get; set; }
        public DateTime? Created { get; set; }
        public List<int> UserList { get; set; }
    }
}

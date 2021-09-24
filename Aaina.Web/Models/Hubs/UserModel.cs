using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Models
{
    public class UserModel
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string LoginTime { get; set; }
    }
}

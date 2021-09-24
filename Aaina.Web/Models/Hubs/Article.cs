using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Models.Hubs
{
    public class Article
    {
        public string articleHeading { get; set; }
        public string articleContent { get; set; }
        public string userId { get; set; }
        public string[] userIds { get; set; }
    }
}

using System;

namespace Aaina.Web.Models.Hubs
{
    public class NotificationModel
    {
       
        public string[] TeamIds { get; set; }
        public string[] PlayerIds { get; set; }
        public string Region { get; set; }
        public string Message { get; set; }
        public int? SentBy { get; set; }
        public int SendTo { get; set; }
        public DateTime SendDate { get; set; }
        public int NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }

        public string[] ConnectedPlayerIds { get; set; }
        public string[] NotConnectedPlayerIds { get; set; }
    }
}

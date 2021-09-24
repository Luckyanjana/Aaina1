using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
   public class NotificationsDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string click_action { get; set; }
        public string icon { get; set; }
        public string image { get; set; }
        public Dictionary<string, string> custom_data { get; set; }
    }
    public class PushNotificationItemDto
    {


        public string Text { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string click_action { get; set; }
        public string icon { get; set; }
        public Dictionary<string, string> custom_data { get; set; }
    }
    public class PushNotificationRequestDto
    {

        public NotificationsDto Notification { get; set; }
        public string To { get; set; }
        public string priority { get; set; }
    }

    public class NotificationsUserDetailDto
    {
        public string TokenId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
    }
}

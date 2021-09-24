using System;

namespace Aaina.Dto
{
    public class NotificationReminderDto
    {
        public long PlayerId { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
        public int? SendBy { get; set; }
        public int SendTo { get; set; }
        public DateTime SendDate { get; set; }
        public int NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }


    }

    public class PendingNotificationDto
    {
        public long Id { get; set; }
        public string SenderName { get; set; }
        public string SendDate { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }

        public int SessionId { get; set; }
        public int UserId { get; set; }


    }
}

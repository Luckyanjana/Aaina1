using Aaina.Data.Models;
using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface INotificationService
    {
        Task<bool> AddReminderNotification(List<NotificationReminderDto> rNModelList);
        int UnreadMessageCount(int playerId);
        List<PendingNotificationDto> GetPlayerPendingMessage(int playerId,int page);
        List<PendingNotificationDto> GetPendingMessage(int userId, int notificationtype);
        int UnreadMessageTotalCount(int userId, int notificationType);
        NotificationReminder GetNotificationById(int id);
        void UpdateNotification(NotificationReminder entity);
        //Task<bool> UpdateReminderNotificationAsync(List<PendingNotificationDto> vModel);
    }
}

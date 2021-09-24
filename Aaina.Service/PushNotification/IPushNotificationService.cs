using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface IPushNotificationService : IDisposable
    {
        Task<bool> PushNotifications(List<PushNotificationItemDto> notificationModel);
        Task<bool> PushNotifications(PushNotificationItemDto item);
        bool Add(int userId, string token);
        bool Delete(int userId, string token);
        List<NotificationsUserDetailDto> GetUserTokenByUserIds(List<int> UserId);

        List<NotificationsUserDetailDto> GetTokenByUserId(int userId);

        List<NotificationsUserDetailDto> GetUserToken(int compantId);
    }
}

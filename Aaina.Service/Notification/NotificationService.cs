using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<NotificationReminder, int> _repoNotificationReminder;
        public NotificationService(IRepository<NotificationReminder, int> repoNotificationReminder)
        {
            _repoNotificationReminder = repoNotificationReminder;
        }
        public async Task<bool> AddReminderNotification(List<NotificationReminderDto> rNModelList)
        {
            List<NotificationReminder> NRList = rNModelList.Select(NR => new NotificationReminder()
            {
                SendBy = NR.SendBy,
                SendTo = NR.SendTo,
                SendDate = NR.SendDate,
                Message = NR.Message,
                Reason = NR.Reason,
                Type = NR.NotificationType,
                IsRead = NR.IsRead,
                ReadDate = NR.ReadDate
            }).ToList();

            await _repoNotificationReminder.InsertRangeAsyn(NRList);
            return true;
        }
        public int UnreadMessageCount(int playerId)
        {
            return _repoNotificationReminder.GetAll(x => x.SendTo == playerId && !x.IsRead).Count();
        }

        public int UnreadMessageTotalCount(int userId, int notificationType)
        {
            return _repoNotificationReminder.GetAll(x => x.SendTo == userId && x.Type == notificationType && !x.IsRead).Count();
        }
        public List<PendingNotificationDto> GetPlayerPendingMessage(int playerId,int page)
        {
            var notificationMessage = _repoNotificationReminder.GetAllIncluding(x => x.SendTo == playerId, x => x.Include(m => m.SendByNavigation)).OrderByDescending(o=>o.SendDate)
                .Skip((page-1)*20).Take(20).Select(x => new PendingNotificationDto()
            {
                Id = x.Id,
                SenderName = x.SendByNavigation.Fname + " " + x.SendByNavigation.Lname,
                SendDate = x.SendDate.ToShortDateString(),
                Reason = x.Reason,
                Message = x.Message
            }).ToList();

           // UpdateReminderNotification(notificationMessage);

            return notificationMessage;
        }

        private bool UpdateReminderNotification(List<PendingNotificationDto> vModel)
        {

            foreach (var item in vModel)
            {
                var record = _repoNotificationReminder.GetIncludingById(x => x.Id == item.Id);
                record.IsRead = true;
                record.ReadDate = DateTime.Now;
                _repoNotificationReminder.Update(record);
            }
            return true;
        }

        public List<PendingNotificationDto> GetPendingMessage(int userId, int notificationtype)
        {
            var notificationMessage = _repoNotificationReminder.GetAllIncluding(x => x.SendTo == userId && x.Type == notificationtype && !x.IsRead, x => x.Include(m => m.SendByNavigation)).Select(x => new PendingNotificationDto()
            {
                Id = x.Id,
                SenderName = x.SendByNavigation.Fname + " " + x.SendByNavigation.Lname,
                SendDate = x.SendDate.ToShortDateString(),
                Reason = x.Reason,
                Message = x.Message
            }).ToList();


            return notificationMessage;
        }
        public NotificationReminder GetNotificationById(int id)
        {
            return _repoNotificationReminder.GetAll(x => x.Id == id).FirstOrDefault();

        }

        public void UpdateNotification(NotificationReminder entity)
        {
            _repoNotificationReminder.Update(entity);

        }

    }
}

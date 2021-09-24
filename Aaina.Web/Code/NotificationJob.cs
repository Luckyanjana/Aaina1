using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models.Hubs;
using FluentValidation.Results;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Aaina.Web.Code
{
    [DisallowConcurrentExecution]
    public class NotificationJob : IJob
    {
        private readonly ISessionService _sessionService;
        private readonly IUserLoginService _userLoginService;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly ILookService _lookService;
        private readonly IStatusService _statusService;
        private readonly IReportService _reportService;
        private readonly IPollService _pollService;
        public NotificationJob(ISessionService sessionService, IUserConnectionManager userConnectionManager,
            INotificationService notificationService, IHubContext<NotificationUserHub> notificationUserHubContext,
            ILookService lookService, IUserLoginService userLoginService, IStatusService statusService, IReportService reportService, IPollService pollService)
        {
            this._sessionService = sessionService;
            this._userConnectionManager = userConnectionManager;
            this._notificationService = notificationService;
            this._notificationUserHubContext = notificationUserHubContext;
            this._lookService = lookService;
            this._userLoginService = userLoginService;
            this._statusService = statusService;
            this._reportService = reportService;
            this._pollService = pollService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var currentDateTime = DateTime.Now;
            if (currentDateTime.Second == 59)
            {
                currentDateTime = currentDateTime.AddSeconds(1);
                await LookNotification(currentDateTime);

                await SessionReminserNotification(currentDateTime);
                SessionReminserEmail(currentDateTime);

                await StatusReminserNotification(currentDateTime);
                StatusReminderMailNotification(currentDateTime);

                await ReportTemplateReminderNotification(currentDateTime);
                ReportTemplateReminderMailNotification(currentDateTime);

                await PollReminderNotification(currentDateTime);
                PollReminderMailNotification(currentDateTime);
            }
        }

        private void StatusReminderMailNotification(DateTime currentDateTime)
        {
            List<StatusFeedbackReminderDto> list = _statusService.GetStatusParticipantReminderEventEmail(currentDateTime, (int)NotificationsType.Email);

            foreach (var item in list)
            {
                var allPlayerEmails = string.Join(";", list.Select(s => s.Email).ToList());
                string html = $"Statusfor {item.Name} <br/> mail from {SiteKeys.Domain} at {DateTime.Now}";
                Common.Common.SendWelComeMail(allPlayerEmails, $"Status for {item.Name}", html);
            }
        }

        private void ReportTemplateReminderMailNotification(DateTime currentDateTime)
        {
            var list = _reportService.GetReportTemplateReminderMailNotification(currentDateTime, (int)NotificationsType.Email);
            var sessionList = list.GroupBy(x => x.Id).ToList();
            foreach (var item in sessionList)
            {
                var allPlayerEmails = string.Join(";", item.Select(s => s.Email).ToList());
                string html = $"Report Template for {item.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain}";
                Common.Common.SendWelComeMail(allPlayerEmails, $"Report Template for {item.FirstOrDefault().Name}", html);
            }
        }



        private void SessionReminserEmail(DateTime currentDateTime)
        {
            var list = _sessionService.GetSessionParticipantReminderEvent(currentDateTime, (int)NotificationsType.Email);
            var sessionList = list.GroupBy(x => x.Id).ToList();
            foreach (var item in sessionList)
            {
                var allPlayerEmails = string.Join(";", item.Select(s => s.Email).ToList());
                string html = $"Session event for {item.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain} Please Click Here to Join Via Meeting Url: <a href='{item.FirstOrDefault().MeetingUrl}'>{list.FirstOrDefault().MeetingUrl}</a> at {DateTime.Now}";
                Common.Common.SendWelComeMail(allPlayerEmails, $"Meeting Invite for session {item.FirstOrDefault().Name}", html);
            }
        }

        private async Task SessionReminserNotification(DateTime currentDateTime)
        {
            var lists = _sessionService.GetSessionParticipantReminderEvent(currentDateTime, (int)NotificationsType.Email);
            var sessionList = lists.GroupBy(x => x.Id).ToList();
            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            foreach (var list in sessionList)
            {
                var title = $"Meeting Invite for session {list.FirstOrDefault().Name}";
                var description = $"Session event for {list.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain} Please Click Here to Join Via Meeting Url: ${list.FirstOrDefault().MeetingUrl} at {DateTime.Now}";
                var allPlayerIds = list.Select(x => x.UserId.ToString()).Distinct().ToList().ToArray();
                var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
                if (connections != null && connections.Count > 0)
                {
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", description, title);
                }

                var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
                var connectedUserIds = userIds.Item1.ToArray();
                var notConnectedUserIds = userIds.Item2.ToArray();

                if (connectedUserIds != null && connectedUserIds.Count() > 0)
                {
                    foreach (var connItem in connectedUserIds)
                    {
                        NotificationReminderDto NCModel = new NotificationReminderDto();
                        NCModel.SendTo = Convert.ToInt32(connItem);
                        NCModel.SendDate = DateTime.Now;
                        NCModel.Reason = title;
                        NCModel.Message = description;
                        NCModel.NotificationType = (int)NotificationType.Reminder;
                        NCModel.IsRead = true;
                        NCModel.ReadDate = DateTime.Now;
                        notifyModel.Add(NCModel);
                    }
                }

                if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                {
                    foreach (var connItem in notConnectedUserIds)
                    {
                        NotificationReminderDto NNCModel = new NotificationReminderDto();
                        NNCModel.SendTo = Convert.ToInt32(connItem);
                        NNCModel.SendDate = DateTime.Now;
                        NNCModel.Reason = title;
                        NNCModel.Message = description;
                        NNCModel.NotificationType = (int)NotificationType.Reminder;
                        NNCModel.IsRead = false;
                        notifyModel.Add(NNCModel);
                    }
                }

            }

            if (notifyModel.Any())
                await _notificationService.AddReminderNotification(notifyModel);
        }
        private async Task StatusReminserNotification(DateTime currentDateTime)
        {
            List<StatusFeedbackReminderDto> lists = _statusService.GetStatusParticipantReminderEventEmail(currentDateTime, (int)NotificationsType.Notification);
            var statusList = lists.GroupBy(x => x.Id).ToList();

            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            foreach (var status in statusList)
            {
                var title = $"Meeting Invite for status {status.FirstOrDefault().Name}";
                var description = $"Status event for {status.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain}  at {DateTime.Now}";
                var allPlayerIds = status.Select(x => x.UserId.ToString()).Distinct().ToList().ToArray();
                var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
                if (connections != null && connections.Count > 0)
                {
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", description, title);
                }

                var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
                var connectedUserIds = userIds.Item1.ToArray();
                var notConnectedUserIds = userIds.Item2.ToArray();

                if (connectedUserIds != null && connectedUserIds.Count() > 0)
                {
                    foreach (var connItem in connectedUserIds)
                    {
                        NotificationReminderDto NCModel = new NotificationReminderDto();
                        NCModel.SendTo = Convert.ToInt32(connItem);
                        NCModel.SendDate = DateTime.Now;
                        NCModel.Reason = title;
                        NCModel.Message = description;
                        NCModel.NotificationType = (int)NotificationType.StatusNotifiction;
                        NCModel.IsRead = true;
                        NCModel.ReadDate = DateTime.Now;
                        notifyModel.Add(NCModel);
                    }
                }

                if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                {
                    foreach (var connItem in notConnectedUserIds)
                    {
                        NotificationReminderDto NNCModel = new NotificationReminderDto();
                        NNCModel.SendTo = Convert.ToInt32(connItem);
                        NNCModel.SendDate = DateTime.Now;
                        NNCModel.Reason = title;
                        NNCModel.Message = description;
                        NNCModel.NotificationType = (int)NotificationType.StatusNotifiction;
                        NNCModel.IsRead = false;
                        notifyModel.Add(NNCModel);
                    }
                }

            }

            if (notifyModel.Any())
                await _notificationService.AddReminderNotification(notifyModel);
        }

        private async Task ReportTemplateReminderNotification(DateTime currentDateTime)
        {
            var list = _reportService.GetReportTemplateReminderMailNotification(currentDateTime, (int)NotificationsType.Notification);
            var reportList = list.GroupBy(x => x.Id).ToList();
            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            foreach (var status in reportList)
            {
                var title = $"Meeting Invite for report template {status.FirstOrDefault().Name}";
                var description = $"Report event for {status.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain}  at {DateTime.Now}";
                var allPlayerIds = status.Select(x => x.UserId.ToString()).Distinct().ToList().ToArray();
                var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
                if (connections != null && connections.Count > 0)
                {
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", description, title);
                }

                var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
                var connectedUserIds = userIds.Item1.ToArray();
                var notConnectedUserIds = userIds.Item2.ToArray();

                if (connectedUserIds != null && connectedUserIds.Count() > 0)
                {
                    foreach (var connItem in connectedUserIds)
                    {
                        NotificationReminderDto NCModel = new NotificationReminderDto();
                        NCModel.SendTo = Convert.ToInt32(connItem);
                        NCModel.SendDate = DateTime.Now;
                        NCModel.Reason = title;
                        NCModel.Message = description;
                        NCModel.NotificationType = (int)NotificationType.ReportNotifiction;
                        NCModel.IsRead = true;
                        NCModel.ReadDate = DateTime.Now;
                        notifyModel.Add(NCModel);
                    }
                }

                if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                {
                    foreach (var connItem in notConnectedUserIds)
                    {
                        NotificationReminderDto NNCModel = new NotificationReminderDto();
                        NNCModel.SendTo = Convert.ToInt32(connItem);
                        NNCModel.SendDate = DateTime.Now;
                        NNCModel.Reason = title;
                        NNCModel.Message = description;
                        NNCModel.NotificationType = (int)NotificationType.ReportNotifiction;
                        NNCModel.IsRead = false;
                        notifyModel.Add(NNCModel);
                    }
                }

            }

            if (notifyModel.Any())
                await _notificationService.AddReminderNotification(notifyModel);
        }

        private async Task PollReminderNotification(DateTime currentDateTime)
        {
            var list = _pollService.GetPollReminderMailNotification(currentDateTime, (int)NotificationsType.Notification);
            var reportList = list.GroupBy(x => x.Id).ToList();
            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            foreach (var status in reportList)
            {
                var title = $"Meeting Invite for Poll {status.FirstOrDefault().Name}";
                var description = $"Poll event for {status.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain}  at {DateTime.Now}";
                var allPlayerIds = status.Select(x => x.UserId.ToString()).Distinct().ToList().ToArray();
                var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
                if (connections != null && connections.Count > 0)
                {
                    await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", description, title);
                }

                var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
                var connectedUserIds = userIds.Item1.ToArray();
                var notConnectedUserIds = userIds.Item2.ToArray();

                if (connectedUserIds != null && connectedUserIds.Count() > 0)
                {
                    foreach (var connItem in connectedUserIds)
                    {
                        NotificationReminderDto NCModel = new NotificationReminderDto();
                        NCModel.SendTo = Convert.ToInt32(connItem);
                        NCModel.SendDate = DateTime.Now;
                        NCModel.Reason = title;
                        NCModel.Message = description;
                        NCModel.NotificationType = (int)NotificationType.PollNotifiction;
                        NCModel.IsRead = true;
                        NCModel.ReadDate = DateTime.Now;
                        notifyModel.Add(NCModel);
                    }
                }

                if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
                {
                    foreach (var connItem in notConnectedUserIds)
                    {
                        NotificationReminderDto NNCModel = new NotificationReminderDto();
                        NNCModel.SendTo = Convert.ToInt32(connItem);
                        NNCModel.SendDate = DateTime.Now;
                        NNCModel.Reason = title;
                        NNCModel.Message = description;
                        NNCModel.NotificationType = (int)NotificationType.PollNotifiction;
                        NNCModel.IsRead = false;
                        notifyModel.Add(NNCModel);
                    }
                }

            }

            if (notifyModel.Any())
                await _notificationService.AddReminderNotification(notifyModel);
        }

        private void PollReminderMailNotification(DateTime currentDateTime)
        {
            var list = _pollService.GetPollReminderMailNotification(currentDateTime, (int)NotificationsType.Email);
            var sessionList = list.GroupBy(x => x.Id).ToList();
            foreach (var item in sessionList)
            {
                var allPlayerEmails = string.Join(";", item.Select(s => s.Email).ToList());
                string html = $"Poll for {item.FirstOrDefault().Name} <br/> mail from {SiteKeys.Domain}";
                Common.Common.SendWelComeMail(allPlayerEmails, $"Poll for {item.FirstOrDefault().Name}", html);
            }
        }

        private async Task LookNotification(DateTime currentDateTime)
        {
            var list = _lookService.GetCompanyLookParticipantNotification(currentDateTime);
            var allPlayerIds = list.Select(x => x.UserId.ToString()).Distinct().ToList().ToArray();
            var connections = _userConnectionManager.GetUserAllConnections(allPlayerIds);
            if (connections != null && connections.Count > 0)
            {
                await _notificationUserHubContext.Clients.Clients(connections).SendAsync("sendToUser", $"look at {DateTime.Now}", "Look Notification");
            }

            var userIds = _userConnectionManager.GetHubUsers(allPlayerIds);
            var connectedUserIds = userIds.Item1.ToArray();
            var notConnectedUserIds = userIds.Item2.ToArray();

            List<NotificationReminderDto> notifyModel = new List<NotificationReminderDto>();
            if (connectedUserIds != null && connectedUserIds.Count() > 0)
            {
                foreach (var connItem in connectedUserIds)
                {
                    NotificationReminderDto NCModel = new NotificationReminderDto();
                    NCModel.SendTo = Convert.ToInt32(connItem);
                    NCModel.SendDate = DateTime.Now;
                    NCModel.Reason = "/admin/Look/AddEdit/29";
                    NCModel.Message = "Look Notification";
                    NCModel.NotificationType = (int)NotificationType.LookNotifiction;
                    NCModel.IsRead = true;
                    NCModel.ReadDate = DateTime.Now;
                    notifyModel.Add(NCModel);
                }
            }

            if (notConnectedUserIds != null && notConnectedUserIds.Count() > 0)
            {
                foreach (var connItem in notConnectedUserIds)
                {
                    NotificationReminderDto NNCModel = new NotificationReminderDto();
                    NNCModel.SendTo = Convert.ToInt32(connItem);
                    NNCModel.SendDate = DateTime.Now;
                    NNCModel.Reason = "/admin/Look/AddEdit/29";
                    NNCModel.Message = "Look Notification";
                    NNCModel.NotificationType = (int)NotificationType.LookNotifiction;
                    NNCModel.IsRead = false;
                    notifyModel.Add(NNCModel);
                }
            }
            foreach (var connItem in allPlayerIds)
            {
                var userobj = _userLoginService.GetByUserId(Convert.ToInt32(connItem));
                string html = "meeting link";

                Common.Common.SendWelComeMail(userobj.Email, "Meeting Invite", html);
            }
            await _notificationService.AddReminderNotification(notifyModel);

        }
    }
}

using Aaina.Service;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Models.Hubs
{
    public class NotificationUserHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IPreSessionGroupService _preSessionGroupService;
        private readonly IUserLoginService _userService;
        private readonly ISessionService _sessionService;
        public NotificationUserHub(IUserConnectionManager userConnectionManager, IPreSessionGroupService preSessionGroupService, IUserLoginService userService,
            ISessionService sessionService)
        {
            _userConnectionManager = userConnectionManager;
            _preSessionGroupService = preSessionGroupService;
            _userService = userService;
            _sessionService = sessionService;
        }
        public string GetConnectionId()
        {
            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];            
            var contextId = Context.ConnectionId;
            _userConnectionManager.KeepUserConnection(userId, contextId);
            return contextId;
        }
        //public async override Task OnConnectedAsync()
        //{
        //    //var connected = base.OnConnectedAsync();
        //    //var userName = Context.User.Identity.Name;
        //    if (this.Context.GetHttpContext().Request.Query["userId"] != "2")

        //        await this.Clients.Client(Context.ConnectionId).SendAsync("sendToUser", "The message","ekdam sahi hai");
        //    // await SendPendingMessages(Context.ConnectionId);
        //    //if (!string.IsNullOrWhiteSpace(userName))
        //    //{
        //    //    var connId = Context.ConnectionId;
        //    //    SendPendingMessages(connId);
        //    //}
        //    // return   connected;
        //}
        //public async Task SendPendingMessages(string connectionId)
        //{
        //    // Get pending messages for user and send it
        //    //var meesages = DataAccess.GetPendingMessages(userName);
        //    await Clients.Client(connectionId).SendAsync("SendPendingMessages", Context.User.Identity.Name, DateTimeOffset.Now, "Hello");
        //}
        //public void SendPendingMessages(string connectionId)
        //{
        //    var userName = Context.User.Identity.Name;
        //    // Get pending messages for user and send it
        //    //var meesages = DataAccess.GetPendingMessages(userName);

        //    Clients.Client(connectionId).SendAsync("sendPendingMessages", "Hello Pending");
        //    //Clients.User(userName).SendAsync("Hello Pending");
        //}
        //Called when a connection with the hub is terminated.

        public Task Joinroup(string group)
        {

            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task RemoveGroup(string group)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        public Task EndGroupChat(string presessionId)
        {
            var pressionSession = _preSessionGroupService.CompleteAndGet(int.Parse(presessionId));
            var attUserIds = _sessionService.GetAllDecisionParticipant(pressionSession.SessionId).Select(x=>x.UserId.ToString()).ToArray();
            var connectionIds = _userConnectionManager.GetConnectedUser(attUserIds);
            Clients.All.SendAsync("sendToRemoveGroup", pressionSession.GroupName);
            return Task.CompletedTask;
        }

        public Task CheckGroupAvail(string userId)
        {
            var contextId = Context.ConnectionId;
            var connectionId = _userConnectionManager.GetUserConnections(userId);
            bool isAdd = connectionId.Any() && connectionId.Contains(contextId);
            
            if (isAdd)
            {
                var preSessionGroup = _preSessionGroupService.GetPresessionProupId(int.Parse(userId));
                if (int.Parse(userId) > 0 && preSessionGroup.Any())
                {
                    
                   Clients.All.SendAsync("sendToJoinGroup", string.Join(",", preSessionGroup.Select(x => x.Id)),
                        string.Join(",", preSessionGroup.Select(x => x.Name)),_sessionService.IsCoordinator(int.Parse(userId),int.Parse(preSessionGroup.FirstOrDefault().Additional)));
                }
            }
            return Task.CompletedTask;
            
        }
               
        public async Task SendMessage(string sender, string message)
        {
            var connectionId = _userConnectionManager.GetUserConnections(sender);
            var user =await _userService.GetById(int.Parse(sender));
            await Clients.All.SendAsync("SendMessage", connectionId, message, $"{user.Fname} {user.Lname}",
                !string.IsNullOrEmpty(user.AvatarUrl)?$"/DYF/{user.CompanyId}/EmployeeImages/{user.AvatarUrl}": "/img/Default_avatar.jpg", DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
        }

        public Task SendMessageToGroup( string groupname, string sender, string message, string presessionId)
        {
            _preSessionGroupService.AddGroupChatAsync(new Dto.PreSessionGroupDetailDto()
            {
                PreSessionGroupId =int.Parse(presessionId),
                SendDate = DateTime.Now,
                Message = message,
                UserId =int.Parse(sender)
            });

            var user = _userService.GetByIdAsync(int.Parse(sender));

            return Clients.Group(groupname).SendAsync("SendMessage", sender, message, $"{user.Fname} {user.Lname}",
                !string.IsNullOrEmpty(user.AvatarUrl) ? $"/DYF/{user.CompanyId}/EmployeeImages/{user.AvatarUrl}" : "/img/Default_avatar.jpg",
                DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            await _userConnectionManager.RemoveUserConnection(connectionId);
            //var value = await Task.FromResult(0);
        }
    }
}

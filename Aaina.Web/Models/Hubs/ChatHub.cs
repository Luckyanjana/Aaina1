using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR.Client;

namespace Aaina.Web.Models.Hubs
{
    //[HubName("ChatHub")]
    public class ChatHub : Hub
    {
        // static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
        // static List<UserModel> ConnectedUsers = new List<UserModel>();
        // static List<MessageModel> CurrentMessage = new List<MessageModel>();
        private readonly IChatService chatService;
        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            var contextId = Context.ConnectionId;
            var connectedUsers = chatService.KeepUserConnection(userId, contextId);
            await Clients.All.SendAsync("UserConnected", connectedUsers.Keys);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var contextId = Context.ConnectionId;
            var connectedUsers = chatService.RemoveUserConnection(contextId);
            await Clients.All.SendAsync("UserDisconnected", connectedUsers.Keys);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task CreateGroup(string senderId, string groupName, string gId, string userIds)
        {
            ChatGroupDto cgDTO = new ChatGroupDto();
            cgDTO.Name = groupName;
            cgDTO.CretedBy = int.Parse(senderId);
            cgDTO.UserList = userIds.Split(',').Select(a => int.Parse(a)).ToList();
            cgDTO.UserList.Add(int.Parse(senderId));
            int groupId = 0;
            var users = new List<string>();
            if (!string.IsNullOrEmpty(gId))
            {
                groupId = int.Parse(gId);
                var updateResponse = await chatService.UpdateGroup(cgDTO, groupId);
                users = updateResponse.Item1.Select(s => s.ToString()).ToList();
                var deletedIds = chatService.GetConnectedUserValue(updateResponse.Item2.Select(s => s.ToString()).ToArray());

                var editedIds = chatService.GetConnectedUserValue(updateResponse.Item3.Select(s => s.ToString()).ToArray());

                foreach (var item in deletedIds)
                {
                    await Groups.RemoveFromGroupAsync(item, groupId.ToString());
                }
                await Clients.Clients(deletedIds).SendAsync("UserRemoveFromGroup", groupId);

                
                await Clients.Clients(editedIds).SendAsync("UpdateGroupName", groupId,groupName,string.Join(",", cgDTO.UserList));
            }
            else
            {
                groupId = await chatService.AddGroup(cgDTO);
                users = cgDTO.UserList.Select(s => s.ToString()).ToList();
            }

            var connectionIds = chatService.GetConnectedUserValue(users.ToArray());
            
            
            foreach (var connectionId in connectionIds)
            {
                await Groups.AddToGroupAsync(connectionId, groupId.ToString());
            }

            await Clients.Clients(connectionIds).SendAsync("UserAddedToGroup", senderId, groupName, groupId.ToString(), userIds, "/img/Group.png");
        }
        public Task Joinroup(string group)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task RemoveGroup(string group)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        public async Task SendMessageToGroup(string groupname, string senderid, string sendername, string senderprofile, string message)
        {
            var user = await chatService.AddMessage(new Dto.ChatMessageDto()
            {
                Message = message,
                ReceiverId = int.Parse(groupname),
                SendDate = DateTime.Now,
                ReceiverType = 2,
                SenderId = int.Parse(senderid)
            });

            await Clients.Group(groupname).SendAsync("ReceiveMessageToGroup", senderid, sendername, senderprofile, groupname, message, DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
        }

        public async Task SendMessageToUser(string senderid, string sendername, string senderprofile, string receiverid, string receivertype, string message)
        {

            var user = await chatService.AddMessage(new Dto.ChatMessageDto()
            {
                Message = message,
                ReceiverId = int.Parse(receiverid),
                SendDate = DateTime.Now,
                ReceiverType = byte.Parse(receivertype),
                SenderId = int.Parse(senderid)
            });
            var connectionIds = chatService.GetUserConnections(receiverid);
            if (connectionIds.Any())
            {
                await Clients.Clients(connectionIds).SendAsync("ReceiveMessageToUser", senderid, sendername, message, receivertype,
                    senderprofile, DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
            }
        }

        public List<ChatUserListDto> GetUserList(string companyId, string userId)
        {
            List<string> connectedUsers = chatService.GetHubConnectedUsers();
            var userList = chatService.GetUserList(int.Parse(companyId), int.Parse(userId));
            userList.ForEach(x =>
            {
                x.IsOnline = connectedUsers.Any(a => int.Parse(a) == x.Id);
            });
            return userList;
        }

        public List<GetChatMessageDto> GetChatHistory(string senderId, string receiverId, string type)
        {
            var userList = chatService.GetChatMessage(int.Parse(senderId), int.Parse(receiverId), int.Parse(type));
            return userList;
        }

        public async Task DeleteUser(string groupId, string userId)
        {
            List<string> connectedUsers = chatService.GetUserConnections(userId);
            await chatService.DeleteUser(int.Parse(groupId), int.Parse(userId));
            foreach (var item in connectedUsers)
            {
                await Groups.RemoveFromGroupAsync(item, groupId);
            }
            await Clients.Clients(connectedUsers).SendAsync("UserRemoveFromGroup", groupId);
        }

        public async Task DeleteGroup(string groupId)
        {
            var allUserList = chatService.GroupUserIds(int.Parse(groupId));
            List<string> connectedUsers = chatService.GetUserAllConnections(allUserList.Select(x => x.ToString()).ToArray());

            await chatService.DeleteGroup(int.Parse(groupId));


            foreach (var item in connectedUsers)
            {
                await Groups.RemoveFromGroupAsync(item, groupId);
            }
            await Clients.Clients(connectedUsers).SendAsync("UserRemoveFromGroup", groupId);
        }
    }
}

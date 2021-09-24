using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class ChatService : IChatService
    {
        private IRepository<ChatMessage, long> repoMessage;
        private IRepository<ChatGroup, int> repoGroup;
        private IRepository<ChatGroupUser, int> repoGroupUser;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
        private static string userConnectionMapLocker = string.Empty;
        public ChatService(IConfiguration configuration, IRepository<ChatMessage, long> repoMessage, IRepository<ChatGroup, int> repoGroup, IRepository<ChatGroupUser, int> repoGroupUser)
        {
            this.repoMessage = repoMessage;
            this.repoGroup = repoGroup;
            this.repoGroupUser = repoGroupUser;
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
        }

        public Dictionary<string, List<string>> KeepUserConnection(string userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);

            }
            return userConnectionMap;
        }
        public Dictionary<string, List<string>> RemoveUserConnection(string connectionId)
        {
            //This method will remove the connectionId of user
            // bool isRemove = false;
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            userConnectionMap.Remove(userId);
                            //isRemove = userConnectionMap[userId].Remove(connectionId);
                            //break;
                        }
                    }
                }
            }

            return userConnectionMap;
            //return await Task.Run(() => isRemove);

        }

        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
                if (userConnectionMap.ContainsKey(userId))
            {
                {
                    conn = userConnectionMap[userId];

                }
            }
            return conn;
        }
        public List<string> GetConnectedUser(string[] userIds)
        {
            var conndUser = new List<string>();
            lock (userConnectionMapLocker)
            {
                conndUser = userConnectionMap.Where(x => userIds.Contains(x.Key)).Select(x => x.Key).ToList();
            }
            return conndUser;
        }
        public List<string> GetConnectedUserValue(string[] userIds)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userIds)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        var connectionId = userConnectionMap[userId][0];
                        conn.Add(connectionId);
                    }
                }
            }
            return conn;
        }
        //public List<List<string>> GetConnectedUserValue(string[] userIds)
        //{
        //    var conndUser = new List<List<string>>();
        //    lock (userConnectionMapLocker)
        //    {
        //          conndUser = userConnectionMap.Where(x => userIds.Contains(x.Key)).Select(x => x.Value).ToList();
        //    }
        //    return conndUser;
        //}
        public Tuple<List<string>, List<string>> GetHubUsers(string[] userIds)
        {
            List<string> conndUser = new List<string>();
            List<string> notConndUser = new List<string>();
            foreach (var id in userIds)
            {
                if (!userConnectionMap.ContainsKey(id))
                {
                    notConndUser.Add(id);
                }
                else
                {
                    conndUser.Add(id);
                }
            }

            return Tuple.Create(conndUser, notConndUser);
        }

        public List<string> GetHubConnectedUsers()
        {
            List<string> conndUser = userConnectionMap.Select(x => x.Key).ToList();
            return conndUser;
        }
        public List<string> GetUserAllConnections(string[] userIds)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                conn = userConnectionMap.Where(x => userIds.Contains(x.Key)).SelectMany(x => x.Value).ToList();
            }
            return conn;
        }
        public async Task<int> AddGroup(ChatGroupDto dto)
        {
            var groupModel = new ChatGroup()
            {
                Name = dto.Name,
                Created = DateTime.Now,
                CretedBy = dto.CretedBy,
                IsDeleted = false
            };
            groupModel = repoGroup.Insert(groupModel);
            await repoGroupUser.InsertRangeAsyn(dto.UserList.Select(x => new ChatGroupUser()
            {
                GroupId = groupModel.Id,
                IsDeleted = false,
                JoinDate = DateTime.Now,
                UserId = x
            }).ToList());
            return groupModel.Id;
        }

        public async Task<Tuple<List<int>, List<int>, List<int>>> UpdateGroup(ChatGroupDto dto, int id)
        {
            var groupModel = repoGroup.Get(id);
            groupModel.Name = dto.Name;
            repoGroup.Update(groupModel);
            var userList = repoGroupUser.GetAllList(x => x.GroupId == id).ToList();
            var existingPlayer = userList.Where(x => dto.UserList.Any(scdet => scdet == x.UserId)).ToList();
            var deletedPlayer = userList.Where(x => !dto.UserList.Any(scdet => scdet == x.UserId)).ToList();
            var insertedPlayer = dto.UserList.Where(x => !userList.Any(m => m.UserId == x)).ToList();
            List<int> deletedUserId = deletedPlayer.Select(s => s.UserId).ToList();
            List<int> addedUserId = insertedPlayer.Select(s => s).ToList();
            List<int> editedUserId = existingPlayer.Select(s => s.UserId).ToList();

            if (deletedPlayer.Any())
            {               
                this.repoGroupUser.DeleteRange(deletedPlayer);
            }


            if (insertedPlayer.Any())
            {
                List<ChatGroupUser> addrecords = insertedPlayer.Select(a => new ChatGroupUser()
                {
                    GroupId = groupModel.Id,
                    IsDeleted = false,
                    JoinDate = DateTime.Now,
                    UserId = a
                }).ToList();

                await repoGroupUser.InsertRangeAsyn(addrecords);
            }


            return new Tuple<List<int>, List<int>, List<int>>(addedUserId, deletedUserId, editedUserId);
        }

        public async Task<long> AddMessage(ChatMessageDto dto)
        {
            var messageModel = new ChatMessage()
            {
                IsRead = dto.IsRead,
                Message = dto.Message,
                ReceiverId = dto.ReceiverId,
                ReceiverType = dto.ReceiverType,
                SendDate = DateTime.Now,
                SenderId = dto.SenderId
            };
            await repoMessage.InsertAsync(messageModel);
            return messageModel.Id;
        }

        public async Task DeleteUser(int groupId, int userId)
        {
            var groupUser = repoGroupUser.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId && !x.IsDeleted);
            groupUser.IsDeleted = true;
            groupUser.DeletedDate = DateTime.Now;
            await repoGroupUser.UpdateAsync(groupUser);
        }

        public async Task DeleteGroup(int groupId)
        {
            var groupUser = repoGroup.Get(groupId);
            groupUser.IsDeleted = true;
            groupUser.DeletedDate = DateTime.Now;
            await repoGroup.UpdateAsync(groupUser);
        }

        public List<int> GroupUserIds(int groupId)
        {
            return repoGroupUser.GetAll(x => x.GroupId == groupId && !x.IsDeleted).Select(x => x.UserId).ToList();
        }

        public List<ChatUserListDto> GetUserList(int companyId, int userId)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[Chat_GetUserList]",
                          new SqlParameter("@companyId", companyId),
                          new SqlParameter("@userId", userId));

            List<ChatUserListDto> respData = new List<ChatUserListDto>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                respData = SqlHelper.ConvertDataTable<ChatUserListDto>(dt);
            }

            return respData;
        }

        public List<GetChatMessageDto> GetChatMessage(int senderId, int receiverId, int receiveType)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[Chat_GetChatMessage]",
                          new SqlParameter("@senderId", senderId),
                          new SqlParameter("@receiverId", receiverId),
                          new SqlParameter("@receiveType", receiveType));

            List<GetChatMessageDto> respData = new List<GetChatMessageDto>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                respData = SqlHelper.ConvertDataTable<GetChatMessageDto>(dt);
                respData = respData.OrderBy(o => o.SendDate).ToList();
            }

            return respData;
        }

        public List<GetChatMessageDto> GetChatMessage(int senderId, int receiverId, int receiveType, DateTime fromDate, DateTime toDate)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[Chat_GetChatMessageByDate]",
                          new SqlParameter("@senderId", senderId),
                          new SqlParameter("@receiverId", receiverId),
                          new SqlParameter("@receiveType", receiveType),
                          new SqlParameter("@fromDate", fromDate),
                          new SqlParameter("@toDate", toDate));

            List<GetChatMessageDto> respData = new List<GetChatMessageDto>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                respData = SqlHelper.ConvertDataTable<GetChatMessageDto>(dt);
                respData = respData.OrderBy(o => o.SendDate).ToList();
            }

            return respData;
        }
    }
}

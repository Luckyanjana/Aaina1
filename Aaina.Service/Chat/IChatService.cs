using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IChatService
    {
        Dictionary<string, List<string>> KeepUserConnection(string userId, string connectionId);
        Dictionary<string, List<string>> RemoveUserConnection(string connectionId);
        
        List<string> GetUserConnections(string userId);
        List<string> GetConnectedUser(string[] userIds);
        List<string> GetConnectedUserValue(string[] userIds);
        Tuple<List<string>, List<string>> GetHubUsers(string[] userIds);
        List<string> GetUserAllConnections(string[] userIds);
        List<string> GetHubConnectedUsers();

        Task<int> AddGroup(ChatGroupDto dto);

        Task<Tuple<List<int>, List<int>, List<int>>> UpdateGroup(ChatGroupDto dto, int id);
        Task<long> AddMessage(ChatMessageDto dto);

        Task DeleteUser(int groupId, int userId);

        Task DeleteGroup(int groupId);

        List<int> GroupUserIds(int groupId);

        List<ChatUserListDto> GetUserList(int companyId, int userId);

        List<GetChatMessageDto> GetChatMessage(int senderId, int receiverId, int receiveType);

        List<GetChatMessageDto> GetChatMessage(int senderId, int receiverId, int receiveType, DateTime fromDate, DateTime toDate);

    }
}

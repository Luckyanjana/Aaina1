using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(string userId, string connectionId);
        // void RemoveUserConnection(string connectionId);
        Task<bool> RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
        List<string> GetConnectedUser(string[] userIds);
        Tuple<List<string>, List<string>> GetHubUsers(string[] userIds);
        //List<string> GetNotConnectedUser(string[] userIds);
        List<string> GetUserAllConnections(string[] userIds);
    }
}

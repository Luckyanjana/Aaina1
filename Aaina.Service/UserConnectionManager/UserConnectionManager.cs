using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public class UserConnectionManager : IUserConnectionManager
    {

        private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
        private static string userConnectionMapLocker = string.Empty;
        public void KeepUserConnection(string userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);
                
            }
        }
        public async Task<bool> RemoveUserConnection(string connectionId)
        {
            //This method will remove the connectionId of user
            bool isRemove = false;
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            isRemove = userConnectionMap.Remove(userId);
                            //isRemove = userConnectionMap[userId].Remove(connectionId);
                            //break;
                        }
                    }
                }
            }

            return await Task.Run(() => isRemove);

        }
        //public List<string> GetUserConnections(string userId)
        //{
        //    var conn = new List<string>();
        //    lock (userConnectionMapLocker)
        //    {
        //        conn = userConnectionMap[userId];
        //    }
        //    return conn;
        //}

        public List<string> GetUserConnections(string userId)
        {
           var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                if (userConnectionMap.ContainsKey(userId))
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
        public List<string> GetUserAllConnections(string[] userIds)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                conn = userConnectionMap.Where(x => userIds.Contains(x.Key)).SelectMany(x => x.Value).ToList();
            }
            return conn;
        }
    }
}

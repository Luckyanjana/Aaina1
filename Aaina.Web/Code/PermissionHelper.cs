using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Web
{
    public static class PermissionHelper
    {
        private static string permissionStorage = "PermissionFile/";
        public static string ExceptionMessage { get; set; }

        public static bool SetPermission(string permission, int userId)
        {
            try
            {
                string path = Path.Combine(permissionStorage);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pathPermission = Path.Combine(permissionStorage) + userId + ".json";

                //// Delete the file if it exists.
                if (File.Exists(pathPermission))
                {
                    File.Delete(pathPermission);
                }

                //// Create the file.
                using (FileStream fs = File.Create(pathPermission))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(permission);
                    //// Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                return true;
            }
            catch (IOException ex)
            {
                ExceptionMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// return Permission json string
        /// </returns>
        public static string GetPermission(int userId)
        {
            try
            {
                string pathPermission = Path.Combine(permissionStorage) + userId + ".json";
                string textPermission = File.ReadAllText(pathPermission);
                return textPermission;
            }
            catch (IOException ex)
            {
                ExceptionMessage = ex.Message;
                return string.Empty;
            }
        }

        public static void RemovePermission(int userId)
        {
            try
            {
                string pathPermission = Path.Combine(permissionStorage) + userId + ".json";
                if (File.Exists(pathPermission))
                {
                    File.Delete(pathPermission);
                }
            }
            catch (IOException ex)
            {

            }
        }
    }
}

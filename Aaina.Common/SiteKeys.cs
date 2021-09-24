using Microsoft.Extensions.Configuration;
using System;

namespace Aaina.Common
{
    public class SiteKeys
    {
        private static IConfigurationSection _configuration;
        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }
        public static string MailServer => _configuration["MailServer"];

        public static string AdminSenderEmail => _configuration["AdminSenderEmail"];

        public static string MailServerPassword => _configuration["MailServerPassword"];

        public static int MailServerPort => int.Parse(_configuration["MailServerPort"]);

        public static bool EnableSsl => bool.Parse(_configuration["EnableSsl"]);
        public static string MailServerUsername => _configuration["MailServerUsername"];

        public static string SenderEmail => _configuration["SenderEmail"];
        public static string SenderName => _configuration["SenderName"];

        public static string Firebase_Public_Key => _configuration["Firebase_Public_Key"];
        public static string Firebase_Server_Key => _configuration["Firebase_Server_Key"];
        public static string FireBaseApiUrl => _configuration["FireBaseApiUrl"];
        public static string ImageUrlDomain => _configuration["ImageUrlDomain"];

        public static string Domain => _configuration["Domain"];

        public static bool IsDriveImplement => bool.Parse(_configuration["IsDriveImplement"]);
        public static string Dropbox_APIKey => _configuration["Dropbox_APIKey"];
        public static string Dropbox_ApiSecret => _configuration["Dropbox_ApiSecret"];
        public static string Dropbox_App => _configuration["Dropbox_App"];
        public static string Dropbox_RedirectUri => _configuration["Dropbox_RedirectUri"];
        public static string Dropbox_AccessTocken => _configuration["Dropbox_AccessTocken"];

        


    }
}

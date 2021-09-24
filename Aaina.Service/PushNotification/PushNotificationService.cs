using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IRepository<PushNotificationToken, int> repo;

        public PushNotificationService(IRepository<PushNotificationToken, int> repo)
        {
            this.repo = repo;
        }

        public async Task<bool> PushNotifications(List<PushNotificationItemDto> notificationModel)
        {
            string fireBaseApiUrl = SiteKeys.FireBaseApiUrl;
            var result = string.Empty;
            using (var httpClient = new HttpClient())
            {
                string serverKey = SiteKeys.Firebase_Server_Key;
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                foreach (var item in notificationModel)
                {
                    PushNotificationRequestDto requestModel = new PushNotificationRequestDto()
                    {
                        Notification = new NotificationsDto()
                        {
                            Body = item.Description,
                            Title = item.Text,
                            click_action = item.click_action,
                            custom_data = item.custom_data,
                            icon = SiteKeys.ImageUrlDomain + "/img/" + "icon.png"
                        },
                        To = item.Token,
                        priority = "high"
                    };

                    var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore // ignore null values
                    });

                    StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore // ignore null values
                    }), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(fireBaseApiUrl, content))
                    {
                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(apiResponse))
                            {
                                result = apiResponse;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            };
            return true;
        }

        public async Task<bool> PushNotifications(PushNotificationItemDto item)
        {
            string fireBaseApiUrl = SiteKeys.FireBaseApiUrl;
            var result = string.Empty;
            using (var httpClient = new HttpClient())
            {
                string serverKey = SiteKeys.Firebase_Server_Key;
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                PushNotificationRequestDto requestModel = new PushNotificationRequestDto()
                {
                    Notification = new NotificationsDto()
                    {
                        Body = item.Description,
                        Title = item.Text,
                        click_action = item.click_action,
                        custom_data = item.custom_data,
                        icon = SiteKeys.ImageUrlDomain + "/img/" + "icon.png"
                    },
                    To = item.Token
                };

                var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore // ignore null values
                });

                StringContent content = new StringContent(JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore // ignore null values
                }), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(fireBaseApiUrl, content))
                {
                    try
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrWhiteSpace(apiResponse))
                        {
                            result = apiResponse;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

            };
            return true;
        }

        public bool Add(int userId, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                PushNotificationToken objToken = repo.FirstOrDefault(x => x.TokenId == token && x.UserId == userId);
                if (objToken == null)
                {
                    objToken = new PushNotificationToken();
                    objToken.UserId = userId;
                    objToken.TokenId = token;
                    repo.Insert(objToken);
                }
            }
            return true;
        }

        public bool Delete(int userId, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                PushNotificationToken objToken = repo.FirstOrDefault(x => x.TokenId == token && x.UserId == userId);
                if (objToken != null)
                {
                    repo.Delete(objToken);
                }
            }
            return true;
        }


        public List<NotificationsUserDetailDto> GetUserTokenByUserIds(List<int> UserId)
        {
            return repo.GetAllList(x => UserId.Contains(x.UserId.Value)).Select(x => new NotificationsUserDetailDto
            {
                TokenId = x.TokenId,
                UserId = x.UserId.Value,
                Name = x.User.Fname + " " + x.User.Lname
            }).ToList();
        }

        public List<NotificationsUserDetailDto> GetTokenByUserId(int userId)
        {
            return repo.GetAllList(x => x.UserId == userId).Select(x => new NotificationsUserDetailDto
            {
                TokenId = x.TokenId,
                UserId = x.UserId.Value,
                Name = x.User.Fname + " " + x.User.Lname
            }).ToList();
        }

        public List<NotificationsUserDetailDto> GetUserToken(int compantId)
        {
            return repo.GetAll().Where(x => x.User.CompanyId == compantId).Select(x => new NotificationsUserDetailDto
            {
                TokenId = x.TokenId,
                UserId = x.UserId.Value,
                Name = x.User.Fname + " " + x.User.Lname
            }).ToList();
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}

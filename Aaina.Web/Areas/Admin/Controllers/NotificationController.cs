using Aaina.Service;
using Aaina.Web.Models.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly IHubContext<NotificationUserHub> _notificationHubContext;
        public NotificationController(IHubContext<NotificationUserHub> notificationHubContext)
        {
            _notificationHubContext = notificationHubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Article model)
        {
            await _notificationHubContext.Clients.All.SendAsync("sendToUser", model.articleHeading, model.articleContent);
            return View();
        }
    }
}

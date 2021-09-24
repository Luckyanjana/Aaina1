using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Service;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Controllers
{
    public class MasterController : BaseController
    {
        private readonly IGameService service;

        public MasterController(IGameService service)
        {           
            this.service = service;
        }
        public IActionResult GameList()
        {
            AddPageHeader("Game", "List");
            var all = service.GetTopParent(CurrentUser.CompanyId);
            return View(all);
        }
    }
}
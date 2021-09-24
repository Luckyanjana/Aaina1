using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Controllers
{
    public class Chat1Controller : BaseCommonController
    {
        private readonly ICompanyService companyService;
        private readonly IChatService chatService;
        private readonly IHostingEnvironment env;
        public Chat1Controller(ICompanyService companyService, IChatService chatService, IHostingEnvironment _env)
        {
            this.chatService = chatService;
            this.companyService = companyService;
            this.env = _env;
           
        }
        public IActionResult Index()
        {           
            return View();
        }
        public IActionResult Index1()
        {
            return View();
        }
    }
}
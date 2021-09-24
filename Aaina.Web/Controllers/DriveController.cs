using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Controllers
{
    public class DriveController : Controller
    {
        private readonly ICompanyService companyService;
        private readonly IUserLoginService userService;
        private readonly IGoogleDriveService googleDriveService;
        private readonly IDropBoxService dropBoxService;

        public DriveController(ICompanyService companyService,
            IUserLoginService userService, IGoogleDriveService googleDriveService, IDropBoxService dropBoxService)
        {
            this.companyService = companyService;
            this.userService = userService;
            this.googleDriveService = googleDriveService;
            this.dropBoxService = dropBoxService;
        }
        public IActionResult Index()
        {
            var companyList = companyService.GetAll();
            foreach (var item in companyList)
            {
                 dropBoxService.CreateFolder($"/{item.Id}");
                
                var userList = userService.GetByCompanyyId(item.Id);
                foreach (var user in userList)
                {
                    dropBoxService.CreateFolder($"/{item.Id}/{user.Id}");
                    
                }
            }


            return View();

        }

    }
}
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class MenuController : BaseController
    {

        private readonly IMenuService service;
        public MenuController(IMenuService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            var allmenu = service.GetAll();
            return View(allmenu);
        }

        public IActionResult AddEdit(int? id, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }
            MenuDto model = new MenuDto();
            if (id.HasValue)
            {
                model = service.GetById(id.Value);
            }
            model.ParentMenuList = service.GetDropdown();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return PartialView("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(MenuDto model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", $"Invalid");
                return CreateModelStateErrors();
            }

            if(!model.IsMain && (string.IsNullOrEmpty(model.Controller) || string.IsNullOrEmpty(model.Action)))
            {
                ModelState.AddModelError("", $"Controller or action name required.");
            }

            if (await service.IsExist(model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.Id > 0)
            {
                service.AddUpdate(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                
                service.AddUpdate(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }

        public IActionResult Activate(int id)
        {
            service.ActivateDeactivate(id,true);
            return RedirectToAction("Index","Menu");
        }

        public IActionResult DeActivate(int id)
        {
            service.ActivateDeactivate(id, false);
            return RedirectToAction("Index", "Menu");
        }

    }
}

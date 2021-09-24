using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code.LIBS;
using Aaina.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Areas.SuperAdmin.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyService service;
        private readonly IUserLoginService userLoginService;
        private readonly IGameService gameService;
        private readonly IDropBoxService dropBoxService;

        public CompanyController(ICompanyService service, IUserLoginService userLoginService, IGameService gameService, IDropBoxService dropBoxService)
        {
            this.service = service;
            this.userLoginService = userLoginService;
            this.gameService = gameService;
            this.dropBoxService = dropBoxService;
        }
        public IActionResult Index()
        {
            AddPageHeader("Company", "List");
            var companies = service.GetAll();
            return View(companies);
        }

        public IActionResult Add()
        {

            return PartialView("_Add");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await service.GetById(id);
            return PartialView("_Edit", company);
        }

        public async Task<IActionResult> Register(CompanyRegisterDto model)
        {
            if (await service.IsExist(model.Name, 0) || await userLoginService.EmailExist(model.Email, 0))
            {
                ModelState.AddModelError("", $"Company name and email already register!");
                return CreateModelStateErrors();
            }


            if (await userLoginService.EmailExist(model.Email, null))
            {
                ModelState.AddModelError("", $"Email already register!");
                return CreateModelStateErrors();
            }

            if (await userLoginService.UserNameExist(model.UserName, null))
            {
                ModelState.AddModelError("", $"User name already register!");
                return CreateModelStateErrors();
            }

            int companyId = await service.Add(new CompanyDto()
            {
                Address = model.Address,
                Desciption = model.Desciption,
                IsActive = true,
                Location = model.Location,
                Name = model.Name
            });
            dropBoxService.CreateFolder($"/{companyId}");
            await gameService.AddUpdateAsync(new GameDto()
            {
                CompanyId = companyId,
                Name = model.Name,
                Weightage = 1
            });
            RegisterDto registerDto = new RegisterDto()
            {
                Fname = model.Fname,
                Lname = model.Lname,
                Email = model.Email,
                UserName = model.UserName,
                Password = "123456"
            };
            string salt = PasswordHasher.GenerateSalt();
            registerDto.Password = PasswordHasher.GeneratePassword(registerDto.Password, salt);
            registerDto.UserType = (int)UserType.Admin;
            int userId = await userLoginService.Register(registerDto, salt, companyId);
            dropBoxService.CreateFolder($"/{companyId}/{userId}");
            ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }

        public async Task<IActionResult> Update(CompanyDto model)
        {
            await service.Update(model);

            ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Company?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Company" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Company has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception ex)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError,
                        NewtonSoftJsonResult(new RequestOutcome<string> { Data = ex.Message }));
            }
        }
    }
}
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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aaina.Web.Areas.SuperAdmin.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ICompanyService service;
        private readonly IUserLoginService userLoginService;
        public AdminController(ICompanyService service, IUserLoginService userLoginService)
        {
            this.service = service;
            this.userLoginService = userLoginService;

        }
        public IActionResult Index()
        {
            AddPageHeader("Admin", "List");
            var allAdmin = userLoginService.GetAllAdminByCompanyyId();
            return View(allAdmin);
        }

        public IActionResult Add()
        {
            UserLoginDto model = new UserLoginDto();
            ViewBag.allCompany = service.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return PartialView("_Add", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            UserLoginDto company = userLoginService.GetByUserId(id);
            ViewBag.allCompany = service.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return PartialView("_Edit", company);
        }

        public async Task<IActionResult> Save(UserLoginDto model)
        {

            if (await userLoginService.EmailExist(model.Email, model.Id))
            {
                ModelState.AddModelError("", $"Email already register!");
                return CreateModelStateErrors();
            }

            if (await userLoginService.UserNameExist(model.UserName, model.Id))
            {
                ModelState.AddModelError("", $"User name already register!");
                return CreateModelStateErrors();
            }


            if (model.Id > 0)
            {
                var user = await userLoginService.GetById(model.Id);
                user.Fname = model.Fname;
                user.Mname = model.Mname;
                user.Lname = model.Lname;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.ModifiedDate = DateTime.Now;
                await userLoginService.UpdateAsync(user);

                ShowSuccessMessage("Success!", $"{model.Fname} has been updated successfully.", false);
            }
            else
            {
                RegisterDto registerDto = new RegisterDto()
                {
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Email = model.Email,
                    UserName=model.UserName,
                    Password = "123456",
                };
                string salt = PasswordHasher.GenerateSalt();
                registerDto.Password = PasswordHasher.GeneratePassword(registerDto.Password, salt);
                registerDto.UserType = (int)UserType.Admin;
                int userId = await userLoginService.Register(registerDto, salt, model.CompanyId);
                ShowSuccessMessage("Success!", $"{model.Fname} has been added successfully.", false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Admin?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Admin" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                userLoginService.DeleteByUserId(id);
                ShowSuccessMessage("Success!", $"Admin has been updated successfully.", false);
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
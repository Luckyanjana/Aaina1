using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Controllers
{
    public class FormBuilderController : BaseController
    {
        private readonly IFormBuilderService _formBuilderService;
        public FormBuilderController(IFormBuilderService formBuilderService)
        {
            _formBuilderService = formBuilderService;
        }
        public IActionResult Index()
        {
            AddPageHeader("Form Builder Template", "List");
            var all = _formBuilderService.GetAll(CurrentUser.CompanyId);
            return View(all);
        }

        public async Task<IActionResult> AddEdit(int? id, int? CopyId)
        {
            FormBuilderDto model = new FormBuilderDto();

            if (CopyId.HasValue)
            {
                model.CopyId = CopyId.Value;
                id = model.CopyId;
            }
            if (id.HasValue)
            {
                model = await _formBuilderService.GetById(id.Value);
            }
            model.DataTypeList = Enum.GetValues(typeof(OptionType)).Cast<OptionType>().Select(x => new SelectedItemDto() { Id = ((int)x).ToString(), Name = x.GetEnumDescription() }).ToList();
            model.DBColumnList.Add(new SelectedItemDto() { Id = "User Name", Name = "User Name" });
            model.DBColumnList.Add(new SelectedItemDto() { Id = "Game Name", Name = "Game Name" });
            model.DBColumnList.Add(new SelectedItemDto() { Id = "Team Name", Name = "Team Name" });
            return View(model);
        }

        public async Task<IActionResult> Copy(FormBuilderDto model)
        {
            model = await _formBuilderService.GetById(model.CopyId);

            return View("AddEdit", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddEdit(FormBuilderDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", $"Invalid");
                return CreateModelStateErrors();
            }
            if (model.Id > 0 && model.CopyId == 0)
            {
                await _formBuilderService.Update(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else if (model.Id > 0 && model.CopyId > 0)
            {
                model.Id = 0;
                model.CompanyId = CurrentUser.CompanyId;
                model.CreatedBy = CurrentUser.UserId;
                await _formBuilderService.Update(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                model.CreatedBy = CurrentUser.UserId;
                await _formBuilderService.Add(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            ///return RedirectToAction("Index", "FormBuilder");
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
        }


        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            FormBuilderDto model = await _formBuilderService.GetById(id);
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Form Template?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Form Template" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                _formBuilderService.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Form Template has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        [HttpPost]
        public IActionResult DeleteAttribute(int attributeId)
        {
            try
            {
                _formBuilderService.DeleteAtterbute(attributeId);
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false });

            }
            
        }

        [HttpPost]
        public IActionResult DeleteAttributeOptions(int attributeId)
        {
            try
            {
                _formBuilderService.DeleteAtterbuteLook(attributeId);
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false });

            }

        }

    }
}

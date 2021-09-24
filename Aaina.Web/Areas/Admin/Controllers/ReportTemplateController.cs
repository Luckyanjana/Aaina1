using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class ReportTemplateController : BaseController
    {
        private readonly IReportService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        public ReportTemplateController(IReportService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
        }
        public IActionResult Index()
        {
            var AllRecord = service.GetAllByCompanyId(CurrentUser.CompanyId,null,null);
            return View(AllRecord);
        }

        public async Task<IActionResult> AddEdit(int? id, int? parentId, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }
            ReportDto model = new ReportDto()
            {
                Scheduler = new ReportSchedulerDto()
                {
                    Type = (int)ScheduleType.OneTime
                }
            };
            string gameName = string.Empty;
            if (!parentId.HasValue)
            {
                var game = gameService.GetFirstGame(CurrentUser.CompanyId);
                parentId = game.Id;
                gameName = game.Name;
            }
            else
            {
                var game = await gameService.GetDetailsId(parentId.Value);
                gameName = game.Name;
            }

            model.GameId = parentId;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
            }
            //model.Game = gameName;
            model.TypeList = Enum.GetValues(typeof(EmotionsFor)).Cast<EmotionsFor>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.AccountAbilityList = Enum.GetValues(typeof(AccountAbilityType)).Cast<AccountAbilityType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ParticipantTypeList = Enum.GetValues(typeof(ParticipantType)).Cast<ParticipantType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.Scheduler != null && !string.IsNullOrEmpty(model.Scheduler.DaysOfWeek))
            {
                model.Scheduler.DaysOfWeekList = model.Scheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await gameService.GetGamlePlayer(CurrentUser.CompanyId)).Select(s => new GameUser()
            {
                Name = s.Name,
                UserId = s.UserId,
                UserTypeId = s.UserTypeId
            }).ToList();

            model.GameList = parentId.HasValue ?
                gameService.GetAllByParentId(parentId.Value, CurrentUser.CompanyId).ToList() :
                gameService.GetAllByParentId(null, CurrentUser.CompanyId);


            model.AllRecord = model.GameId.HasValue ? service.GetAllByGameId(CurrentUser.CompanyId, model.GameId.Value) : new List<ReportDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(ReportDto model)
        {
            model.CompanyId = CurrentUser.CompanyId;
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.Scheduler == null || !model.Scheduler.StartDate.HasValue)
            {
                ModelState.AddModelError("", $"Report Scheduler Required!");
                return CreateModelStateErrors();
            }

            model.Reminder = model.Reminder.Where(x => x.Every.HasValue && x.TypeId.HasValue && x.Unit.HasValue).ToList();

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }



            if (model.Scheduler != null && model.Scheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.Scheduler.DaysOfWeek = string.Join(",", model.Scheduler.DaysOfWeekList);
            }

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);

                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("AddEdit", new { id = "", parentId = model.GameId }), IsSuccess = true });

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Report?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Report" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Look has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });

            }
            catch (Exception exception)
            {
                return Json(new RequestOutcome<string> { IsSuccess = false, Message = exception?.Message, Data = exception?.StackTrace });
            }
        }
    }
}
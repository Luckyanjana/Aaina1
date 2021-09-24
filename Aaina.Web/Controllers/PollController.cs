using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Controllers
{
    public class PollController : BaseController
    {
        private readonly IPollService service;
        private readonly IGameService gameService;
        private readonly IHostingEnvironment env;

        public PollController(IPollService service, IGameService gameService, IHostingEnvironment env)
        {
            this.service = service;
            this.gameService = gameService;
            this.env = env;


        }
        public IActionResult Index(int tenant)
        {
            var AllRecord = service.GetAllByCompanyId(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant);
            return View(AllRecord);
        }


        public async Task<IActionResult> Add(int tenant)
        {
            return await AddEdit(null, tenant, null);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int tenant, int? copyId)
        {
            return await AddEdit(id, tenant, copyId);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PollDto model, int tenant)
        {
            return await AddEdit(model, tenant);
        }
        private async Task<IActionResult> AddEdit(int? id, int tenant, int? copyId)
        {
            if (copyId.HasValue)
            {
                id = copyId;
            }
            PollDto model = new PollDto()
            {
                PollScheduler = new PollSchedulerDto()
                {
                    Type = (int)ScheduleType.OneTime
                },
                GameId=tenant
            };
            string gameName = string.Empty;
            if (tenant==0)
            {
                var game = gameService.GetFirstGame(CurrentUser.CompanyId);
                tenant = game.Id.Value;
                gameName = game.Name;
            }
            else
            {
                var game = await gameService.GetDetailsId(tenant);
                gameName = game.Name;
            }

            model.GameId = tenant;
            if (id.HasValue)
            {
                model = await service.GetById(id.Value);
                
            }
            model.SubGameList = gameService.GetAllDropByParent(model.GameId.Value);
            model.PollTypeList = Enum.GetValues(typeof(PollType)).Cast<PollType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();


            model.NotificationsList = Enum.GetValues(typeof(NotificationsType)).Cast<NotificationsType>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();
            model.NotificationsUnitList = Enum.GetValues(typeof(NotificationsUnit)).Cast<NotificationsUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.ScheduleFrequencyList = Enum.GetValues(typeof(ScheduleFrequency)).Cast<ScheduleFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.WeekDayList = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.DailyFrequencyList = Enum.GetValues(typeof(ScheduleDailyFrequency)).Cast<ScheduleDailyFrequency>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.MonthlyOccurrenceList = Enum.GetValues(typeof(ScheduleMonthlyOccurrence)).Cast<ScheduleMonthlyOccurrence>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            model.OccursEveryTimeUnitList = Enum.GetValues(typeof(ScheduleTimeUnit)).Cast<ScheduleTimeUnit>().Select(c => new SelectedItemDto() { Name = c.GetEnumDescription(), Id = ((int)c).ToString() }).ToList();

            if (model.PollScheduler != null && !string.IsNullOrEmpty(model.PollScheduler.DaysOfWeek))
            {
                model.PollScheduler.DaysOfWeekList = model.PollScheduler.DaysOfWeek.Split(',').Select(s => int.Parse(s)).ToList();
            }
            model.PlayerList = (await gameService.GetGamlePlayer(CurrentUser.CompanyId));
            

            model.AllRecord = model.GameId.HasValue ? service.GetAllByCompanyId(CurrentUser.CompanyId,CurrentUser.UserId, model.GameId.Value) : new List<PollDto>();
            if (copyId.HasValue)
            {
                model.Id = null;
            }
            return View("_AddEdit", model);
        }
        
        private async Task<IActionResult> AddEdit(PollDto model, int tenant)
        {
            if (!ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.PollScheduler == null || !model.PollScheduler.StartDate.HasValue)
            {
                ModelState.AddModelError("", $"Poll Scheduler Required!");
                return CreateModelStateErrors();
            }

            model.PollQuestion = model.PollQuestion.Where(x => x.QuestionTypeId > 0 && !string.IsNullOrEmpty(x.Name)).ToList();

            if (!model.PollQuestion.Any())
            {
                ModelState.AddModelError("", $"Poll Question Required!");
                return CreateModelStateErrors();
            }

            for (int i = 0; i < model.PollQuestion.Count; i++)
            {
                var preAgenda = model.PollQuestion[i];
                var preAgendaOptionAdd = new List<PollOptionDto>();
                for (int j = 0; j < preAgenda.PollQuestionOption.Count; j++)
                {
                    var preAgendaOption = preAgenda.PollQuestionOption[j];

                    var files = Request.Form.Files.Where(x => x.Name == $"PollQuestion[{i}].PollQuestionOption[{j}].optionFile").ToList();
                    if (files != null && files.Any())
                    {

                        foreach (IFormFile item in files)
                        {
                            string fileName = await this.UploadProfile(env, CurrentUser.CompanyId, item, null, $"Poll/{CurrentUser.UserId}", new[] { ".png", ".jpeg", ".jpg" });
                            preAgendaOptionAdd.Add(new PollOptionDto()
                            {
                                FilePath = fileName,
                                Id = preAgendaOption.Id,
                                Name = preAgendaOption.Name
                            });
                        }
                    }
                    else
                    {
                        preAgendaOptionAdd.Add(preAgendaOption);
                    }
                }
                preAgenda.PollQuestionOption = preAgendaOptionAdd;


            }


            model.PollReminder = model.PollReminder.Where(x => x.Every.HasValue && x.TypeId.HasValue && x.Unit.HasValue).ToList();

            if (await service.IsExist(CurrentUser.CompanyId, model.Name, model.Id))
            {
                ModelState.AddModelError("", $"Name already register!");
                return CreateModelStateErrors();
            }

            if (model.PollScheduler.Type == (int)ScheduleType.Recurring && !model.PollScheduler.RecurseEvery.HasValue)
            {
                ModelState.AddModelError("", $"Recurse Every is required");
                return CreateModelStateErrors();
            }

            if (model.PollScheduler.DailyFrequency == (int)ScheduleDailyFrequency.Every)
            {
                bool isValid = true;

                if (!model.PollScheduler.OccursEveryValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Value is required");
                }

                if (!model.PollScheduler.OccursEveryTimeUnit.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("", $"Occurs Every Time Unit is required");
                }

                if (!isValid)
                {
                    return CreateModelStateErrors();
                }
            }


            if (model.PollScheduler != null && model.PollScheduler.Frequency == (int)ScheduleFrequency.Weekly)
            {
                model.PollScheduler.DaysOfWeek = string.Join(",", model.PollScheduler.DaysOfWeekList);
            }

            if (model.Id > 0)
            {
                await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been updated successfully.", false);
            }
            else
            {
                model.CompanyId = CurrentUser.CompanyId;
                model.CreatedBy = CurrentUser.UserId;
                var sessionId = await service.AddUpdateAsync(model);
                ShowSuccessMessage("Success!", $"{model.Name} has been added successfully.", false);
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Poll/Index", IsSuccess = true });

        }

        [HttpGet]
        public async Task<IActionResult> Feedback(int id)
        {
            PollFeedbackDisplayDto poll = await service.GetFeedbackById(id);
            return PartialView("_Feedback", poll);
        }

        [HttpPost]
        public IActionResult Feedback(PollFeedbackAddDto dto)
        {
            if (dto.QuestionList.Any(a=>!a.AnswerId.HasValue))
            {
                ModelState.AddModelError("", $"Please give answer");
                return CreateModelStateErrors();
            }

            dto.UserId = CurrentUser.UserId;
            service.AddFeedback(dto);
            ShowSuccessMessage("Success!", $"Poll feedback has been added successfully.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
        }

        [HttpGet]
        public IActionResult Delete(int id,int tenant)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Poll?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Poll" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id,int tenant, IFormCollection FormCollection)
        {
            try
            {
                service.DeleteBy(id);
                ShowSuccessMessage("Success!", $"Poll has been updated successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = $"/{tenant}/Poll/index", IsSuccess = true });

            }
            catch (Exception exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "This Record used another place", Data = exception?.StackTrace, IsSuccess = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            PollFeedbackDisplayDto poll = new PollFeedbackDisplayDto();
            if (CurrentUser.RoleId == (int)UserType.User)
            {
                poll = await service.ViewResult(id, CurrentUser.UserId);
            }
            else
            {
                poll = await service.ViewResult(id, null);
                poll.ParticipantList = await service.GetParticipantByPollId(id);
            }
            
            return PartialView("_result", poll);
        }

        [HttpGet]
        public async Task<IActionResult> ViewResult(int id, int userId)
        {
            PollFeedbackDisplayDto poll = await service.ViewResult(id, userId);
            return Json(poll.QuestionList);
        }

    }
}
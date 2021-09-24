using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Controllers
{
    public class SessionScheduleController : BaseController
    {
        private readonly ISessionService service;
        private readonly IReportService reportService;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly IPreSessionService preSessionService;
        private readonly IHostingEnvironment env;
        private readonly IDropBoxService dropBoxService;
        public SessionScheduleController(ISessionService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService, IPreSessionService preSessionService, IHostingEnvironment env, IReportService reportService,
            IDropBoxService dropBoxService)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
            this.preSessionService = preSessionService;
            this.env = env;
            this.reportService = reportService;
            this.dropBoxService = dropBoxService;
        }
        public IActionResult Index(int tenant)
        {
            // ViewBag.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId);
            return View();
        }

        public IActionResult GetEvent(int tenant, DateTime start, DateTime end, bool isSession, bool isReport)
        {
            List<SessionScheduleEventDto> events = new List<SessionScheduleEventDto>();
            try
            {
                if (isSession)
                {
                    events = service.GetCompanySesstionEvent(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant, start, end);
                }

                if (isReport)
                {
                    var event1 = reportService.GetCompanyReportEvent(CurrentUser.CompanyId, CurrentUser.RoleId == (int)UserType.User ? CurrentUser.UserId : (int?)null, tenant, start, end);
                    if (event1.Any())
                    {
                        if (events.Any())
                            events.AddRange(event1);
                        else
                            events = event1;
                    }
                }
            }
            catch (Exception ex)
            {
                events = new List<SessionScheduleEventDto>();

            }

            return Json(events);
        }
        public IActionResult Details(int id, DateTime start, DateTime end)
        {
            SessionEventDetails events = service.GetSesstionEventDetails(id, start, end,CurrentUser.UserId);
            return PartialView("_EventDetails", events);
        }

        public async Task<IActionResult> Agenda(int sessionId, DateTime start, DateTime end)
        {
            PreSessionDto model = await preSessionService.GetById(sessionId, start, end, CurrentUser.UserId);

            ViewBag.sessionId = sessionId;
            ViewBag.start = start;
            ViewBag.end = end;
            ViewBag.isDecisionMaker = service.IsDecisionMaker(CurrentUser.UserId, sessionId);
            List<PreSessionAgendaListDto> agendaList = await preSessionService.GetList(sessionId, start, end, CurrentUser.UserId);
            ViewBag.agendaList = agendaList;
            ViewBag.isValid = preSessionService.PreSessionUpdateStatus(CurrentUser.CompanyId, sessionId, start, end);
            ViewBag.AccountableList = userService.GetAllDrop(CurrentUser.CompanyId, CurrentUser.UserId).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agenda(PreSessionDto model, int tenant)
        {
            var session = await this.service.GetById(model.SessionId);
            for (int i = 0; i < model.PreSessionAgenda.Count; i++)
            {
                var preAgenda = model.PreSessionAgenda[i];
                var files = Request.Form.Files.Where(x => x.Name == $"PreSessionAgenda[{i}].files").ToList();
                if (files != null && files.Any())
                {
                    preAgenda.PreSessionAgendaDoc = new List<PreSessionAgendaDocDto>();
                    foreach (IFormFile item in files)
                    {
                        dropBoxService.Upload(item, $"/{CurrentUser.CompanyId}/Game_{session.GameId}/Session_{model.SessionId}/Agenda");
                        preAgenda.PreSessionAgendaDoc.Add(new PreSessionAgendaDocDto()
                        {
                            FileName = item.FileName
                        });
                    }
                }
            }


            model.CompanyId = CurrentUser.CompanyId;
            model.CreatedBy = CurrentUser.UserId;
            await preSessionService.AddUpdateAsync(model);
            return Redirect($"/{tenant}/SessionSchedule/index");
        }

        public IActionResult GetPlayAction(int tenant, int sessionId, DateTime start, DateTime end)
        {
            var model = preSessionService.GetPlayAction(sessionId, start, end, CurrentUser.UserId, tenant);
            return Json(model);
        }
        public async Task<IActionResult> AgendaList(int sessionId, DateTime start, DateTime end)
        {
            ViewBag.sessionId = sessionId;
            ViewBag.start = start.ToString("dd/MM/yyyy HH:mm");
            ViewBag.end = end.ToString("dd/MM/yyyy HH:mm");
            ViewBag.isDecisionMaker = service.IsDecisionMaker(CurrentUser.UserId, sessionId);
            List<PreSessionAgendaListDto> model = await preSessionService.GetList(sessionId, start, end, CurrentUser.UserId);
            ViewBag.isValid = preSessionService.PreSessionUpdateStatus(CurrentUser.CompanyId, sessionId, start, end);
            return View(model);
        }
        public IActionResult AgendaApprove(int id)
        {
            bool model = preSessionService.Approve(id);
            return Json(new { isSucess = model });
        }

        public IActionResult AgendaDelete(int id)
        {
            bool model = preSessionService.Delete(id);
            return Json(new { isSucess = model });
        }
        public IActionResult AgendaDisApprove(int id)
        {
            bool model = preSessionService.DisApprove(id);
            return Json(new { isSucess = model });
        }

        public IActionResult Accept(int sessionId, DateTime start, DateTime end)
        {
            bool model = preSessionService.UpdateStatus(CurrentUser.CompanyId, sessionId, start, end, (int)PreSessionStatusType.Accept, CurrentUser.UserId, null);
            return Json(new { isSucess = model });
        }

        public IActionResult Reject(int sessionId, DateTime start, DateTime end)
        {
            bool model = preSessionService.UpdateStatus(CurrentUser.CompanyId, sessionId, start, end, (int)PreSessionStatusType.Reject, CurrentUser.UserId, null);
            return Json(new { isSucess = model });
        }

        public IActionResult ReSchedule(int sessionId, DateTime start, DateTime end, DateTime reSchedule)
        {
            bool model = preSessionService.UpdateStatus(CurrentUser.CompanyId, sessionId, start, end, (int)PreSessionStatusType.ReSchedule, CurrentUser.UserId, reSchedule);
            return Json(new { isSucess = model });
        }

        public IActionResult Delegate(int sessionId, int delegateId)
        {
            bool model = preSessionService.UpdateDelegate(sessionId, CurrentUser.UserId, delegateId);
            return Json(new { isSucess = model });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aaina.Web.Areas.Admin.Controllers
{
    public class SessionScheduleController : BaseController
    {
        private readonly ISessionService service;
        private readonly IGameService gameService;
        private readonly ITeamService teamService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userService;
        private readonly IAttributeService attributeService;
        private readonly IPreSessionService preSessionService;
        private readonly IHostingEnvironment env;
        public SessionScheduleController(ISessionService service, IGameService gameService, IRoleService roleService, IUserLoginService userService,
            IAttributeService attributeService, ITeamService teamService, IPreSessionService preSessionService, IHostingEnvironment env)
        {
            this.roleService = roleService;
            this.userService = userService;
            this.service = service;
            this.gameService = gameService;
            this.attributeService = attributeService;
            this.teamService = teamService;
            this.preSessionService = preSessionService;
            this.env = env;
        }
        public IActionResult Index()
        {
            ViewBag.GameList = gameService.GetAllDrop(null, CurrentUser.CompanyId);
            return View();
        }
        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult GetEvent(int? gameId,DateTime start, DateTime end)
        {
            List<SessionScheduleEventDto> events = new List<SessionScheduleEventDto>();
            try
            {
                events = service.GetCompanySesstionEvent(CurrentUser.CompanyId,gameId, start, end);
            }
            catch (Exception ex)
            {
                events = new List<SessionScheduleEventDto>();

            }

            return Json(events);
        }

        public IActionResult Details(int id, DateTime start, DateTime end)
        {
            SessionEventDetails events =  service.GetSesstionEventDetails( id, start, end,CurrentUser.UserId);
            return PartialView("_EventDetails", events);
        }

        public async Task<IActionResult> Agenda(int sessionId, DateTime start, DateTime end)
        {
            PreSessionDto model = await preSessionService.GetById(sessionId, start, end,CurrentUser.UserId);

            ViewBag.sessionId = sessionId;
            ViewBag.start = start.ToString("dd/MM/yyyy");
            ViewBag.end = end.ToString("dd/MM/yyyy");
            ViewBag.isDecisionMaker = service.IsDecisionMaker(CurrentUser.UserId, sessionId);
            List<PreSessionAgendaListDto> agendaList = await preSessionService.GetList(sessionId, start, end, CurrentUser.UserId);
            ViewBag.agendaList = agendaList;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agenda(PreSessionDto model)
        {
            for (int i = 0; i < model.PreSessionAgenda.Count; i++)
            {
                var preAgenda = model.PreSessionAgenda[i];
                var files = Request.Form.Files.Where(x => x.Name == $"PreSessionAgenda[{i}].files").ToList();
                if (files != null && files.Any())
                {
                    preAgenda.PreSessionAgendaDoc = new List<PreSessionAgendaDocDto>();
                    foreach (IFormFile item in files)
                    {
                        string fileName = await this.UploadProfile(env, CurrentUser.CompanyId, item, null, $"Agenda/{CurrentUser.UserId}", new[] { ".pdf" });
                        preAgenda.PreSessionAgendaDoc.Add(new PreSessionAgendaDocDto()
                        {
                            FileName = fileName
                        });
                    }
                }
            }

            model.CompanyId = CurrentUser.CompanyId;
            model.CreatedBy = CurrentUser.UserId;
            await preSessionService.AddUpdateAsync(model);
            return RedirectToAction("index");
        }

        public async Task<IActionResult> AgendaList(int sessionId, DateTime start, DateTime end)
        {
            List<PreSessionAgendaListDto> model = await preSessionService.GetList(sessionId, start, end,CurrentUser.UserId);
            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Aaina.Service
{
    public class PreSessionService : IPreSessionService
    {
        private IRepository<PreSession, int> repo;
        private IRepository<UserLogin, int> userLoginRepo;
        private IRepository<Play, int> repoPlay;
        private IRepository<Session, int> repoSession;
        private IRepository<SessionParticipant, int> repoSessionParticipant;
        private IRepository<PreSessionParticipant, int> repoParticipant;
        private IRepository<PreSessionAgenda, int> repoAgenda;
        private IRepository<PreSessionAgendaDoc, int> repoAgendaDoc;
        private IRepository<PreSessionStatus, int> repoPreSessionStatus;
        private IRepository<PreSessionDelegate, int> repoPreSessionDelegate;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        public PreSessionService(IConfiguration configuration, IRepository<PreSession, int> repo, IRepository<PreSessionParticipant, int> repoParticipant,
           IRepository<PreSessionAgenda, int> repoAgenda, IRepository<PreSessionAgendaDoc, int> repoAgendaDoc,
           IRepository<Play, int> repoPlay, IRepository<Session, int> repoSession, IRepository<UserLogin, int> userLoginRepo, IRepository<SessionParticipant, int> repoSessionParticipant, IRepository<PreSessionStatus, int> repoPreSessionStatus, IRepository<PreSessionDelegate, int> repoPreSessionDelegate)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoParticipant = repoParticipant;
            this.repoAgenda = repoAgenda;
            this.repoAgendaDoc = repoAgendaDoc;
            this.repoPlay = repoPlay;
            this.repoSession = repoSession;
            this.userLoginRepo = userLoginRepo;
            this.repoSessionParticipant = repoSessionParticipant;
            this.repoPreSessionStatus = repoPreSessionStatus;
            this.repoPreSessionDelegate = repoPreSessionDelegate;
        }

        public async Task<int> AddUpdateAsync(PreSessionDto dto)
        {
            if (dto.Id.HasValue)
            {
                var look = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.PreSessionParticipant)
             .Include(m => m.PreSessionAgenda).Include("PreSessionAgenda.PreSessionAgendaDoc"));

                if (look.PreSessionParticipant != null)
                {
                    var existingAttribute = look.PreSessionParticipant.Where(x => dto.PreSessionParticipant.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedAttribute = look.PreSessionParticipant.Where(x => !dto.PreSessionParticipant.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedAttribute = dto.PreSessionParticipant.Where(x => !look.PreSessionParticipant.Any(m => m.Id == x.Id)).ToList();


                    if (deletedAttribute.Any())
                    {
                        this.repoParticipant.DeleteRange(deletedAttribute);
                    }

                    if (existingAttribute.Any())
                    {
                        foreach (var e in existingAttribute)
                        {
                            var record = dto.PreSessionParticipant.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId;
                            e.TypeId = record.TypeId;
                            e.ParticipantTyprId = record.ParticipantTyprId;
                            e.Remarks = record.Remarks;
                            e.Status = record.Status;
                            repoParticipant.Update(e);
                        }
                    }
                    if (insertedAttribute.Any())
                    {
                        List<PreSessionParticipant> addrecords = insertedAttribute.Select(a => new PreSessionParticipant()
                        {
                            PreSessionId = dto.Id.Value,
                            ParticipantTyprId = a.ParticipantTyprId,
                            TypeId = a.TypeId,
                            UserId = a.UserId,
                            Remarks = a.Remarks,
                            Status = a.Status
                        }).ToList();

                        await repoParticipant.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.PreSessionAgenda != null)
                {
                    var existingGame = look.PreSessionAgenda.Where(x => dto.PreSessionAgenda.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = look.PreSessionAgenda.Where(x => !dto.PreSessionAgenda.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.PreSessionAgenda.Where(x => !look.PreSessionAgenda.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {
                        foreach (var item in deletedGame)
                        {
                            if (item.PreSessionAgendaDoc != null && item.PreSessionAgendaDoc.Any())
                            {
                                repoAgendaDoc.DeleteRange(item.PreSessionAgendaDoc.ToList());
                            }
                        }
                        this.repoAgenda.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {

                            if (e.PreSessionAgendaDoc != null && e.PreSessionAgendaDoc.Any())
                            {
                                repoAgendaDoc.DeleteRange(e.PreSessionAgendaDoc.ToList());
                            }

                            var record = dto.PreSessionAgenda.FirstOrDefault(a => a.Id == e.Id);
                            e.PlayId = record.PlayId;
                            e.PreSessionAgendaDoc = record.PreSessionAgendaDoc.Select(x => new PreSessionAgendaDoc()
                            {
                                FileName = x.FileName
                            }).ToList();
                            repoAgenda.Update(e);
                        }
                    }

                    if (insertedGame.Any())
                    {
                        List<PreSessionAgenda> addrecords = insertedGame.Select(a => new PreSessionAgenda()
                        {
                            PlayId = a.PlayId,
                            PreSessionId = dto.Id.Value,
                            PreSessionAgendaDoc = a.PreSessionAgendaDoc.Select(x => new PreSessionAgendaDoc()
                            {
                                FileName = x.FileName
                            }).ToList()
                        }).ToList();

                        await repoAgenda.InsertRangeAsyn(addrecords);
                    }
                }

                // 

                look.ModifiedDate = DateTime.Now;


                repo.Update(look);
                return dto.Id.Value;
            }
            else
            {
                var lookInfo = new PreSession()
                {
                    CompanyId = dto.CompanyId.Value,
                    CreatedBy = dto.CreatedBy,
                    SessionId = dto.SessionId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    PreSessionParticipant = dto.PreSessionParticipant.Select(a => new PreSessionParticipant()
                    {

                        ParticipantTyprId = a.ParticipantTyprId,
                        TypeId = a.TypeId,
                        UserId = a.UserId,
                        PreSessionId = a.PreSessionId,
                        Remarks = a.Remarks,
                        Status = a.Status
                    }).ToList(),
                    PreSessionAgenda = dto.PreSessionAgenda.Select(a => new PreSessionAgenda()
                    {
                        PlayId = a.PlayId,
                        PreSessionAgendaDoc = a.PreSessionAgendaDoc.Select(x => new PreSessionAgendaDoc()
                        {
                            FileName = x.FileName
                        }).ToList()
                    }).ToList()

                };

                await repo.InsertAsync(lookInfo);
                return lookInfo.Id;
            }

        }

        public async Task<PreSessionDto> GetById(int id, DateTime start, DateTime end, int userId)
        {
            PreSessionDto model = new PreSessionDto()
            {
                SessionId = id,
                StartDate = start,
                EndDate = end
            };
            var x = await this.repo.GetIncludingByIdAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end && x.CreatedBy == userId,
                x => x.Include(m => m.PreSessionParticipant)
            .Include(m => m.PreSessionAgenda).Include("PreSessionAgenda.Play").Include("PreSessionAgenda.PreSessionAgendaDoc")
            .Include("PreSessionAgenda.Play.Game").Include("PreSessionAgenda.Play.Dependancy").Include("PreSessionParticipant.User").Include(m => m.Session));
            if (x != null)
            {
                model = new PreSessionDto()
                {
                    SessionId = x.SessionId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CreatedBy = x.CreatedBy,
                    SessionName = x.Session.Name,
                    PreSessionParticipant = x.PreSessionParticipant.Select(s => new PreSessionParticipantDto()
                    {
                        Id = s.Id,
                        ParticipantTyprId = s.ParticipantTyprId,
                        ParticipantType = ((ParticipantType)s.ParticipantTyprId).GetEnumDescription(),
                        Type = ((PlayersType)s.TypeId).GetEnumDescription(),
                        StatusStr = ((ConfirmStatus)s.Status).GetEnumDescription(),
                        TypeId = s.TypeId,
                        UserId = s.UserId,
                        User = s.User.Fname + " " + s.User.Lname,
                        Remarks = s.Remarks,
                        Status = s.Status
                    }).ToList(),
                    PreSessionAgenda = x.PreSessionAgenda.Select(s => new PreSessionAgendaDto()
                    {
                        Id = s.Id,
                        PlayId = s.PlayId,
                        AccountableId = s.Play.AccountableId,
                        Status = s.Play.Status,
                        StartDate = s.Play.StartDate,
                        ActualEndDate = s.Play.ActualEndDate,
                        ActualStartDate = s.Play.ActualStartDate,
                        AddedOn = s.Play.AddedOn,
                        DeadlineDate = s.Play.DeadlineDate,
                        DependancyId = s.Play.DependancyId,
                        Game = s.Play.Game.Name,
                        Dependancy = s.Play.Dependancy != null ? s.Play.Dependancy.Fname + " " + s.Play.Dependancy.Lname : null,
                        Description = s.Play.Description,
                        FeedbackType = s.Play.FeedbackType,
                        GameId = s.Play.GameId,
                        Name = s.Play.Name,
                        PreSessionId = s.PreSessionId,
                        Priority = s.Play.Priority,
                        SubGameId = s.Play.SubGameId,
                        Type = s.Play.Type,
                        PreSessionAgendaDoc = s.PreSessionAgendaDoc.Select(c => new PreSessionAgendaDocDto()
                        {

                            FileName = c.FileName,
                            Id = c.Id
                        }).ToList()
                    }).ToList()

                };
            }
            else
            {


                var session = await this.repoSession.GetIncludingByIdAsyn(a => a.Id == id, x => x.Include(m => m.SessionParticipant)
                .Include("SessionParticipant.User"));
                model.SessionName = session.Name;
                model.PreSessionParticipant = session.SessionParticipant.Select(x => new PreSessionParticipantDto()
                {
                    ParticipantTyprId = x.ParticipantTyprId,
                    PreSessionId = x.ParticipantTyprId,
                    Remarks = x.Remarks,
                    Status = x.Status,
                    TypeId = x.TypeId,
                    User = x.User.Fname + " " + x.User.Lname,
                    UserId = x.UserId.Value,
                    ParticipantType = ((ParticipantType)x.ParticipantTyprId).GetEnumDescription(),
                    Type = ((PlayersType)x.TypeId).GetEnumDescription(),
                    StatusStr = ((ConfirmStatus)x.Status).GetEnumDescription(),
                }).ToList();


            }

            model.PreSessionAgendaList = repoPlay.GetAllIncluding(x => x.FeedbackType.HasValue, x => x.Include(m => m.Game).Include(m => m.Dependancy)).Select(s => new PreSessionAgendaDto()
            {
                Id = s.Id,
                AccountableId = s.AccountableId,
                FeedbackType = s.FeedbackType,
                GameId = s.GameId,
                ActualEndDate = s.ActualEndDate,
                ActualStartDate = s.ActualStartDate,
                AddedOn = s.AddedOn,
                DeadlineDate = s.DeadlineDate,
                DependancyId = s.DependancyId,
                Description = s.Description,
                Name = s.Name,
                Priority = s.Priority,
                Prioritystr = ((PriorityType)s.Priority).GetEnumDescription(),
                StartDate = s.StartDate,
                Status = s.Status,
                StatusStr = ((StatusType)s.Status).GetEnumDescription(),
                SubGameId = s.SubGameId,
                Type = s.Type,
                TypeStr = ((FeedbackType)s.Type).GetEnumDescription(),
                Game = s.Game.Name,
                Dependancy = s.Dependancy != null ? s.Dependancy.Fname + " " + s.Dependancy.Lname : null,
            }).ToList();

            return model;
        }

        public List<PreSessionAgendaDto> GetPlayAction(int id, DateTime start, DateTime end, int userId, int gameId)
        {
            var preSessionAgendaList = repoPlay.GetAllIncluding(x => x.FeedbackType.HasValue && x.GameId
           == gameId && (x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) || x.DependancyId == userId), x => x.Include(m => m.Game).Include(m => m.Dependancy)).Select(s => new PreSessionAgendaDto()
           {
               Id = s.Id,
               AccountableId = s.AccountableId,
               FeedbackType = s.FeedbackType,
               GameId = s.GameId,
               ActualEndDate = s.ActualEndDate,
               ActualStartDate = s.ActualStartDate,
               AddedOn = s.AddedOn,
               DeadlineDate = s.DeadlineDate,
               DependancyId = s.DependancyId,
               Description = s.Description,
               Name = s.Name,
               Priority = s.Priority,
               Prioritystr = ((PriorityType)s.Priority).GetEnumDescription(),
               StartDate = s.StartDate,
               Status = s.Status,
               StatusStr = ((StatusType)s.Status).GetEnumDescription(),
               SubGameId = s.SubGameId,
               Type = s.Type,
               TypeStr = ((FeedbackType)s.Type).GetEnumDescription(),
               Game = s.Game.Name,
               Dependancy = s.Dependancy != null ? s.Dependancy.Fname + " " + s.Dependancy.Lname : null,
           }).ToList();
            return preSessionAgendaList;
        }
        public async Task<List<PreSessionAgendaListDto>> GetList(int id, DateTime start, DateTime end, int userId)
        {
            var iscordinate = repoSessionParticipant.Any(a => a.SessionId == id && a.UserId == userId && a.ParticipantTyprId == (int)ParticipantType.Coordinator);
            var isDecisionMaker = repoSessionParticipant.Any(a => a.SessionId == id && a.UserId == userId && a.ParticipantTyprId == (int)ParticipantType.DecisionMaker);
            List<PreSessionAgendaListDto> model = new List<PreSessionAgendaListDto>();
            var x = await this.repo.GetAllIncludingAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end, x => x.Include(mbox => mbox.Session).Include(m => m.PreSessionAgenda).Include("PreSessionAgenda.Play").Include("PreSessionAgenda.PreSessionAgendaDoc"));

            foreach (var item in x)
            {
                var user = userLoginRepo.Get(item.CreatedBy.Value);
                foreach (var agenda in item.PreSessionAgenda.Where(w => isDecisionMaker ? w.IsApproved : true))
                {
                    model.Add(new PreSessionAgendaListDto()
                    {
                        UserName = user.Fname + " " + user.Lname,
                        UserId = item.CreatedBy.Value,
                        SessionName = item.Session.Name,
                        Description = agenda.Play.Description,
                        Name = agenda.Play.Name,
                        Id = agenda.Id,
                        Status = agenda.Play.Status,
                        IsCoordinator = iscordinate,
                        IsApproved = agenda.IsApproved,
                        GameId = item.Session.GameId.Value,
                        SessionId = item.SessionId,
                        PreSessionAgendaDoc = agenda.PreSessionAgendaDoc.Select(d => new PreSessionAgendaDocDto()
                        {
                            FileName = d.FileName,
                            Id = d.Id
                        }).ToList()
                    });
                }


            }

            var pendingPresession = this.repoAgenda.GetAllIncluding(x => x.PreSession.SessionId == id && x.PreSession.StartDate != start && x.PreSession.EndDate != end && (x.Play.Status == (int)StatusType.Active || x.Play.Status == (int)StatusType.Delay),
              x => x.Include(m => m.Play).Include(m => m.PreSessionAgendaDoc).Include(m => m.Play.Game).Include(m => m.Play.Dependancy).Include(m => m.PreSession).Include(m => m.PreSession.Session));

            foreach (var agenda in pendingPresession.Where(w => w.IsApproved && isDecisionMaker ? w.IsApproved : true))
            {
                var user = userLoginRepo.Get(agenda.PreSession.CreatedBy.Value);
                model.Add(new PreSessionAgendaListDto()
                {
                    UserName = user.Fname + " " + user.Lname,
                    UserId = agenda.PreSession.CreatedBy.Value,
                    SessionName = agenda.PreSession.Session.Name,
                    Description = agenda.Play.Description,
                    Name = agenda.Play.Name,
                    Id = agenda.Id,
                    Status = agenda.Play.Status,
                    IsCoordinator = iscordinate,
                    IsApproved = agenda.IsApproved,
                    GameId = agenda.PreSession.Session.GameId.Value,
                    SessionId = agenda.PreSession.SessionId,
                    PreSessionAgendaDoc = agenda.PreSessionAgendaDoc.Select(d => new PreSessionAgendaDocDto()
                    {
                        FileName = d.FileName,
                        Id = d.Id
                    }).ToList()
                });
            }

            return model;
        }

        public bool Approve(int id)
        {
            var agenda = repoAgenda.Get(id);
            var alreadyApproved = repoAgenda.Any(x => x.PreSessionId == agenda.Id && x.PlayId == agenda.PlayId && x.IsApproved);
            if (!alreadyApproved)
            {
                agenda.IsApproved = true;
                repoAgenda.Update(agenda);
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Delete(int id)
        {
            try
            {
                var agenda = repoAgenda.GetIncludingById(x => x.Id == id, x => x.Include(m => m.PreSessionAgendaDoc));
                var docList = agenda.PreSessionAgendaDoc.ToList();
                repoAgendaDoc.DeleteRange(docList);
                repoAgenda.Delete(agenda);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }            

        }

        public bool DisApprove(int id)
        {
            var agenda = repoAgenda.Get(id);
            agenda.IsApproved = false;
            repoAgenda.Update(agenda);
            return true;
        }

        public bool PreSessionUpdateStatus(int companyId, int sessionId, DateTime startDate, DateTime endDate)
        {
            return !repoPreSessionStatus.Any(x => x.SessionId == sessionId && (x.Status == (int)PreSessionStatusType.Accept || x.Status == (int)PreSessionStatusType.Reject)
           && x.StartDate == startDate && x.EndDate == endDate);
        }
        public bool UpdateStatus(int companyId, int sessionId, DateTime startDate, DateTime endDate, int statusId, int userId, DateTime? reScheduleDate)
        {
            repoPreSessionStatus.Insert(new PreSessionStatus()
            {
                AddedDate = DateTime.Now,
                CompanyId = companyId,
                DecisionMakerId = userId,
                EndDate = endDate,
                StartDate = startDate,
                ModifiedDate = DateTime.Now,
                SessionId = sessionId,
                ReDateTime = reScheduleDate,
                Status = statusId
            });
            return true;
        }

        public bool UpdateDelegate(int sessionId, int userId, int delegateId)
        {
            var parti = repoSessionParticipant.FirstOrDefault(z => z.SessionId == sessionId && z.UserId == userId && z.ParticipantTyprId == (int)ParticipantType.DecisionMaker);
            if (parti != null)
            {
                parti.UserId = delegateId;
                repoSessionParticipant.Update(parti);
            }

            repoPreSessionDelegate.Insert(new PreSessionDelegate()
            {
                DecisionMakerId = userId,
                SessionId = sessionId,
                DelegateDate = DateTime.Now,
                DelegateId = delegateId
            });
            return true;
        }
    }
}

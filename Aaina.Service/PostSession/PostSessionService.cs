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
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace Aaina.Service
{
    public class PostSessionService : IPostSessionService
    {
        private IRepository<PreSession, int> repoPreSession;
        private IRepository<PreSessionAgenda, int> repoPreAgenda;
        private IRepository<PostSession, int> repo;
        private IRepository<UserLogin, int> userLoginRepo;
        private IRepository<Play, int> repoPlay;
        private IRepository<Session, int> repoSession;
        private IRepository<PostSessionAgenda, int> repoAgenda;

        private IRepository<PreSessionStatus, int> repoPreSessionStatus;

        public PostSessionService(IRepository<PreSession, int> repoPreSession, IRepository<PostSession, int> repo, IRepository<PostSessionAgenda, int> repoAgenda,
           IRepository<Play, int> repoPlay, IRepository<Session, int> repoSession, IRepository<UserLogin, int> userLoginRepo, IRepository<SessionParticipant, int> repoSessionParticipant, IRepository<PreSessionStatus, int> repoPreSessionStatus, IRepository<PreSessionAgenda, int> repoPreAgenda)
        {

            this.repoPreSession = repoPreSession;
            this.repo = repo;
            this.repoAgenda = repoAgenda;
            this.repoPlay = repoPlay;
            this.repoSession = repoSession;
            this.userLoginRepo = userLoginRepo;
            this.repoPreSessionStatus = repoPreSessionStatus;
            this.repoPreAgenda = repoPreAgenda;
        }

        public async Task<int> AddUpdateAsync(PostSessionDto dto)
        {
            if (dto.Id.HasValue)
            {
                var look = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.PostSessionAgenda));


                if (look.PostSessionAgenda != null)
                {
                    var existingGame = look.PostSessionAgenda.Where(x => dto.PostSessionAgenda.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = look.PostSessionAgenda.Where(x => !dto.PostSessionAgenda.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.PostSessionAgenda.Where(x => !look.PostSessionAgenda.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {

                        this.repoAgenda.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {

                            var record = dto.PostSessionAgenda.FirstOrDefault(a => a.Id == e.Id);
                            var playAction = repoPlay.Get(record.PlayId);
                            playAction.Name = record.Name;
                            playAction.GameId = record.GameId;
                            playAction.SubGameId = record.SubGameId;
                            playAction.Description = record.Description;
                            playAction.AddedOn = record.AddedOn;
                            playAction.StartDate = record.StartDate;
                            playAction.DeadlineDate = record.DeadlineDate;
                            playAction.AccountableId = record.AccountableId;
                            playAction.DependancyId = record.DependancyId;
                            playAction.FeedbackType = record.FeedbackType;
                            playAction.Priority = record.Priority;
                            playAction.Status = record.Status;
                            playAction.Type = record.Type;
                            playAction.AccountableId = record.AccountableId;
                            playAction.ActualEndDate = record.ActualEndDate;
                            playAction.ActualStartDate = record.ActualStartDate;
                            playAction.Emotion = Common.EncryptDecrypt.Encrypt(record.Emotions.ToString());
                            playAction.CoordinateEmotion = Common.EncryptDecrypt.Encrypt(record.CoordinateEmotion.ToString());
                            playAction.DecisionMakerEmotion = Common.EncryptDecrypt.Encrypt(record.DecisionMakerEmotion.ToString());
                            playAction.ModifiedDate = DateTime.Now;
                            repoPlay.Update(playAction);

                            e.PlayId = record.PlayId;
                            e.Remarks = record.Remarks;
                            repoAgenda.Update(e);
                        }
                    }

                    if (insertedGame.Any())
                    {
                        List<PostSessionAgenda> addrecords = insertedGame.Select(a => new PostSessionAgenda()
                        {
                            Remarks = a.Remarks,
                            PlayId = a.PlayId,
                            PostSessionId = dto.Id.Value
                        }).ToList();

                        await repoAgenda.InsertRangeAsyn(addrecords);

                        foreach (var record in insertedGame)
                        {
                            var playAction = repoPlay.Get(record.PlayId);
                            playAction.Name = record.Name;
                            playAction.GameId = record.GameId;
                            playAction.SubGameId = record.SubGameId;
                            playAction.Description = record.Description;
                            playAction.AddedOn = record.AddedOn;
                            playAction.StartDate = record.StartDate;
                            playAction.DeadlineDate = record.DeadlineDate;
                            playAction.AccountableId = record.AccountableId;
                            playAction.DependancyId = record.DependancyId;
                            playAction.FeedbackType = record.FeedbackType;
                            playAction.Priority = record.Priority;
                            playAction.Status = record.Status;
                            playAction.Type = record.Type;
                            playAction.AccountableId = record.AccountableId;
                            playAction.ActualEndDate = record.ActualEndDate;
                            playAction.ActualStartDate = record.ActualStartDate;
                            playAction.Emotion = Common.EncryptDecrypt.Encrypt(record.Emotions.ToString());
                            playAction.CoordinateEmotion = Common.EncryptDecrypt.Encrypt(record.CoordinateEmotion.ToString());
                            playAction.DecisionMakerEmotion = Common.EncryptDecrypt.Encrypt(record.DecisionMakerEmotion.ToString());
                            playAction.ModifiedDate = DateTime.Now;
                            repoPlay.Update(playAction);
                        }

                    }
                }



                look.ModifiedDate = DateTime.Now;


                repo.Update(look);
                return dto.Id.Value;
            }
            else
            {
                var lookInfo = new PostSession()
                {
                    CompanyId = dto.CompanyId.Value,
                    CreatedBy = dto.CreatedBy,
                    SessionId = dto.SessionId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    PostSessionAgenda = dto.PostSessionAgenda.Select(a => new PostSessionAgenda()
                    {
                        Remarks = a.Remarks,
                        PlayId = a.PlayId
                    }).ToList()
                };

                await repo.InsertAsync(lookInfo);

                foreach (var record in dto.PostSessionAgenda)
                {
                    var playAction = repoPlay.Get(record.PlayId);
                    playAction.Name = record.Name;
                    playAction.GameId = record.GameId;
                    playAction.SubGameId = record.SubGameId;
                    playAction.Description = record.Description;
                    playAction.AddedOn = record.AddedOn;
                    playAction.StartDate = record.StartDate;
                    playAction.DeadlineDate = record.DeadlineDate;
                    playAction.AccountableId = record.AccountableId;
                    playAction.DependancyId = record.DependancyId;
                    playAction.FeedbackType = record.FeedbackType;
                    playAction.Priority = record.Priority;
                    playAction.Status = record.Status;
                    playAction.Type = record.Type;
                    playAction.AccountableId = record.AccountableId;
                    playAction.ActualEndDate = record.ActualEndDate;
                    playAction.ActualStartDate = record.ActualStartDate;
                    playAction.Emotion = Common.EncryptDecrypt.Encrypt(record.Emotions.ToString());
                    playAction.CoordinateEmotion = Common.EncryptDecrypt.Encrypt(record.CoordinateEmotion.ToString());
                    playAction.DecisionMakerEmotion = Common.EncryptDecrypt.Encrypt(record.DecisionMakerEmotion.ToString());
                    playAction.ModifiedDate = DateTime.Now;
                    repoPlay.Update(playAction);
                }

                return lookInfo.Id;
            }

        }

        public async Task<PostSessionDto> GetById(int id, DateTime start, DateTime end, int userId)
        {
            PostSessionDto model = new PostSessionDto()
            {
                SessionId = id,
                StartDate = start,
                EndDate = end
            };
            var x = await this.repo.GetIncludingByIdAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end && x.CreatedBy == userId,
                x => x.Include(m => m.PostSessionAgenda).Include("PostSessionAgenda.Play").Include("PostSessionAgenda.Play.Game").Include("PostSessionAgenda.Play.Dependancy").Include("PostSessionAgenda.Play.Accountable").Include(m => m.Session));
            if (x != null)
            {
                model = new PostSessionDto()
                {
                    SessionId = x.SessionId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CreatedBy = x.CreatedBy,
                    SessionName = x.Session.Name,
                    ModifiedDate = x.ModifiedDate,
                    PostSessionAgenda = x.PostSessionAgenda.Select(s => new PostSessionAgendaDto()
                    {
                        Id = s.Id,
                        AccountableId = s.Play.AccountableId,
                        Status = s.Play.Status,
                        StatusStr = ((StatusType)s.Play.Status).GetEnumDescription(),
                        Prioritystr = ((PriorityType)s.Play.Priority).GetEnumDescription(),
                        StartDate = s.Play.StartDate,
                        ActualEndDate = s.Play.ActualEndDate,
                        ActualStartDate = s.Play.ActualStartDate,
                        AddedOn = s.Play.AddedOn,
                        DeadlineDate = s.Play.DeadlineDate,
                        DependancyId = s.Play.DependancyId,
                        Game = s.Play.Game.Name,
                        Dependancy = s.Play.Dependancy != null ? s.Play.Dependancy.Fname + " " + s.Play.Dependancy.Lname : null,
                        Accountable = s.Play.Accountable != null ? s.Play.Accountable.Fname + " " + s.Play.Accountable.Lname : null,
                        Description = s.Play.Description,
                        FeedbackType = s.Play.FeedbackType,
                        GameId = s.Play.GameId,
                        Name = s.Play.Name,
                        PostSessionId = s.PostSessionId,
                        Priority = s.Play.Priority,
                        SubGameId = s.Play.SubGameId,
                        Emotions = int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.Emotion)),
                        CoordinateEmotion = s.Play.CoordinateEmotion!=null? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.CoordinateEmotion)):1,
                        DecisionMakerEmotion = s.Play.DecisionMakerEmotion != null ? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.DecisionMakerEmotion)):1,
                        Remarks = s.Remarks,
                        Type = s.Play.Type
                    }).ToList()

                };
            }
            else
            {
                var preSession = await this.repoPreSession.GetIncludingByIdAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end && x.CreatedBy == userId,
                 x => x.Include(m => m.PreSessionParticipant)
             .Include(m => m.PreSessionAgenda).Include("PreSessionAgenda.Play").Include("PreSessionAgenda.PreSessionAgendaDoc")
             .Include("PreSessionAgenda.Play.Game").Include("PreSessionAgenda.Play.Dependancy").Include("PreSessionParticipant.User").Include(m => m.Session));

                model = new PostSessionDto()
                {
                    SessionId = preSession.SessionId,
                    StartDate = preSession.StartDate,
                    EndDate = preSession.EndDate,
                    CompanyId = preSession.CompanyId,
                    CreatedBy = preSession.CreatedBy,
                    SessionName = preSession.Session.Name,
                    ModifiedDate = preSession.ModifiedDate,
                    PostSessionAgenda = preSession.PreSessionAgenda.Select(s => new PostSessionAgendaDto()
                    {
                        Id = s.Id,
                        AccountableId = s.Play.AccountableId,
                        Status = s.Play.Status,
                        StatusStr = ((StatusType)s.Play.Status).GetEnumDescription(),
                        Prioritystr = ((PriorityType)s.Play.Priority).GetEnumDescription(),
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
                        Priority = s.Play.Priority,
                        SubGameId = s.Play.SubGameId,
                        Type = s.Play.Type
                    }).ToList()

                };
            }

            return model;
        }

        public async Task<PostSessionDto> GetByForPostId(int id, DateTime start, DateTime end, int userId)
        {
            PostSessionDto model = new PostSessionDto()
            {
                SessionId = id,
                StartDate = start,
                EndDate = end
            };

            var x = await this.repo.GetIncludingByIdAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end,
                x => x.Include(m => m.PostSessionAgenda).Include("PostSessionAgenda.Play").Include("PostSessionAgenda.Play.Game").Include("PostSessionAgenda.Play.SubGame").Include("PostSessionAgenda.Play.Dependancy").Include("PostSessionAgenda.Play.Accountable").Include(m => m.Session));
            if (x != null)
            {
                model = new PostSessionDto()
                {
                    SessionId = x.SessionId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CreatedBy = x.CreatedBy,
                    SessionName = x.Session.Name,
                    ModifiedDate = x.ModifiedDate,
                    PostSessionAgenda = x.PostSessionAgenda.Select(s => new PostSessionAgendaDto()
                    {
                        Id = s.Id,
                        PlayId = s.PlayId,
                        AccountableId = s.Play.AccountableId,
                        Status = s.Play.Status,
                        StatusStr = ((StatusType)s.Play.Status).GetEnumDescription(),
                        Prioritystr = ((PriorityType)s.Play.Priority).GetEnumDescription(),
                        StartDate = s.Play.StartDate,
                        ActualEndDate = s.Play.ActualEndDate,
                        ActualStartDate = s.Play.ActualStartDate,
                        AddedOn = s.Play.AddedOn,
                        DeadlineDate = s.Play.DeadlineDate,
                        DependancyId = s.Play.DependancyId,
                        Game = s.Play.Game.Name,
                        SubGame = s.Play.SubGameId.HasValue ? s.Play.SubGame.Name : string.Empty,
                        Dependancy = s.Play.Dependancy != null ? s.Play.Dependancy.Fname + " " + s.Play.Dependancy.Lname : null,
                        Accountable = s.Play.Accountable != null ? s.Play.Accountable.Fname + " " + s.Play.Accountable.Lname : null,
                        Description = s.Play.Description,
                        FeedbackType = s.Play.FeedbackType,
                        GameId = s.Play.GameId,
                        Name = s.Play.Name,
                        PostSessionId = s.PostSessionId,
                        Priority = s.Play.Priority,
                        SubGameId = s.Play.SubGameId,
                        Emotions = int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.Emotion)),
                        CoordinateEmotion = s.Play.CoordinateEmotion != null && s.Play.CoordinateEmotion != "CrjD+5Klxck=" ? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.CoordinateEmotion)) : 1,
                        DecisionMakerEmotion = s.Play.DecisionMakerEmotion != null && s.Play.DecisionMakerEmotion!= "CrjD+5Klxck=" ? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.DecisionMakerEmotion)) : 1,
                        Remarks = s.Remarks,
                        Type = s.Play.Type
                    }).ToList()

                };


            }
            else
            {
                var preSessions = await this.repoPreSession.GetAllIncludingAsyn(x => x.SessionId == id && x.StartDate == start && x.EndDate == end && x.PreSessionAgenda.Any(a => a.IsApproved),
                 x => x.Include(m => m.PreSessionParticipant)
             .Include(m => m.PreSessionAgenda).Include("PreSessionAgenda.Play").Include("PreSessionAgenda.PreSessionAgendaDoc")
             .Include("PreSessionAgenda.Play.Game").Include("PreSessionAgenda.Play.SubGame").Include("PreSessionAgenda.Play.Dependancy").Include("PreSessionAgenda.Play.Accountable").Include("PreSessionParticipant.User").Include(m => m.Session));

                if (preSessions != null && preSessions.Any())
                {
                    model = new PostSessionDto()
                    {
                        SessionId = preSessions.FirstOrDefault().SessionId,
                        StartDate = preSessions.FirstOrDefault().StartDate,
                        EndDate = preSessions.FirstOrDefault().EndDate,
                        CompanyId = preSessions.FirstOrDefault().CompanyId,
                        CreatedBy = preSessions.FirstOrDefault().CreatedBy,
                        SessionName = preSessions.FirstOrDefault().Session.Name,
                        ModifiedDate = preSessions.FirstOrDefault().ModifiedDate
                    };

                    foreach (var preSession in preSessions)
                    {
                        model.PostSessionAgenda.AddRange(
                            preSession.PreSessionAgenda.Where(a => a.IsApproved).Select(s => new PostSessionAgendaDto()
                            {
                                Id = s.Id,
                                PlayId = s.PlayId,
                                AccountableId = s.Play.AccountableId,
                                Status = s.Play.Status,
                                StatusStr = ((StatusType)s.Play.Status).GetEnumDescription(),
                                Prioritystr = ((PriorityType)s.Play.Priority).GetEnumDescription(),
                                Accountable = s.Play.Accountable.Fname + " " + s.Play.Accountable.Lname,
                                StartDate = s.Play.StartDate,
                                ActualEndDate = s.Play.ActualEndDate,
                                ActualStartDate = s.Play.ActualStartDate,
                                AddedOn = s.Play.AddedOn,
                                DeadlineDate = s.Play.DeadlineDate,
                                DependancyId = s.Play.DependancyId,
                                Game = s.Play.Game.Name,
                                SubGame = s.Play.SubGameId.HasValue ? s.Play.SubGame.Name : string.Empty,
                                Dependancy = s.Play.Dependancy != null ? s.Play.Dependancy.Fname + " " + s.Play.Dependancy.Lname : null,
                                Description = s.Play.Description,
                                FeedbackType = s.Play.FeedbackType,
                                GameId = s.Play.GameId,
                                Name = s.Play.Name,
                                Priority = s.Play.Priority,
                                SubGameId = s.Play.SubGameId,
                                Type = s.Play.Type,
                                Emotions = int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.Emotion)),
                                CoordinateEmotion = s.Play.CoordinateEmotion != null && s.Play.CoordinateEmotion != "CrjD+5Klxck=" ? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.CoordinateEmotion)) : 1,
                                DecisionMakerEmotion = s.Play.DecisionMakerEmotion != null && s.Play.DecisionMakerEmotion != "CrjD+5Klxck=" ? int.Parse(Common.EncryptDecrypt.Decrypt(s.Play.DecisionMakerEmotion)) : 1,
                            }).ToList()
                            );
                    }

                }

                var pendingPresession = this.repoPreAgenda.GetAllIncluding(x => x.PreSession.SessionId == id && x.IsApproved && x.PreSession.StartDate != start && x.PreSession.EndDate != end && (x.Play.Status == (int)StatusType.Active || x.Play.Status == (int)StatusType.Delay),
          x => x.Include(m => m.Play).Include(m => m.PreSessionAgendaDoc).Include(m => m.Play.Game).Include(m => m.Play.SubGame).Include(m => m.Play.Dependancy).Include(m => m.Play.Accountable).Include(m => m.PreSession.Session));

                if (!model.PostSessionAgenda.Any())
                {
                    model.PostSessionAgenda = new List<PostSessionAgendaDto>();
                }

                foreach (var s in pendingPresession.Where(x => x.IsApproved))
                {
                    model.PostSessionAgenda.Add(new PostSessionAgendaDto()
                    {
                        Id = s.Id,
                        PlayId = s.PlayId,
                        AccountableId = s.Play.AccountableId,
                        Status = s.Play.Status,
                        StatusStr = ((StatusType)s.Play.Status).GetEnumDescription(),
                        Prioritystr = ((PriorityType)s.Play.Priority).GetEnumDescription(),
                        Accountable = s.Play.Accountable.Fname + " " + s.Play.Accountable.Lname,
                        StartDate = s.Play.StartDate,
                        ActualEndDate = s.Play.ActualEndDate,
                        ActualStartDate = s.Play.ActualStartDate,
                        AddedOn = s.Play.AddedOn,
                        DeadlineDate = s.Play.DeadlineDate,
                        DependancyId = s.Play.DependancyId,
                        Game = s.Play.Game.Name,
                        SubGame = s.Play.SubGameId.HasValue ? s.Play.SubGame.Name : string.Empty,
                        Dependancy = s.Play.Dependancy != null ? s.Play.Dependancy.Fname + " " + s.Play.Dependancy.Lname : null,
                        Description = s.Play.Description,
                        FeedbackType = s.Play.FeedbackType,
                        GameId = s.Play.GameId,
                        Name = s.Play.Name,
                        Priority = s.Play.Priority,
                        SubGameId = s.Play.SubGameId,
                        Type = s.Play.Type
                    });
                }
            }

            model.IsCoordinator = repoSession.Any(x => x.Id == id && x.SessionParticipant.Any(s => s.UserId == userId && s.ParticipantTyprId == (int)ParticipantType.Coordinator));
            model.IsDecisionMaker = repoSession.Any(x => x.Id == id && x.SessionParticipant.Any(s => s.UserId == userId && s.ParticipantTyprId == (int)ParticipantType.DecisionMaker));

            return model;
        }
    }
}

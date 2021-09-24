using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class SessionService : ISessionService
    {
        private IRepository<Session, int> repo;
        private IRepository<UserLogin, int> repoUserLogin;
        private IRepository<SessionScheduler, int> repoScheduler;
        private IRepository<SessionParticipant, int> repoParticipant;
        private IRepository<SessionReminder, int> repoReminder;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        public SessionService(IConfiguration configuration, IRepository<Session, int> repo, IRepository<SessionScheduler, int> repoScheduler, IRepository<SessionParticipant, int> repoParticipant, IRepository<SessionReminder, int> repoReminder, IRepository<UserLogin, int> repoUserLogin)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoScheduler = repoScheduler;
            this.repoParticipant = repoParticipant;
            this.repoReminder = repoReminder;
            this.repoUserLogin = repoUserLogin;
        }


        public async Task<SessionDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.SessionScheduler)
            .Include(m => m.SessionReminder).Include(m => m.SessionParticipant));
            SessionSchedulerDto sessionScheduler = new SessionSchedulerDto();

            SessionScheduler sch = x.SessionScheduler;
            sessionScheduler = new SessionSchedulerDto()
            {
                DailyFrequency = sch.DailyFrequency,
                DaysOfWeek = sch.DaysOfWeek,
                EndDate = sch.EndDate,
                ExactDateOfMonth = sch.ExactDateOfMonth,
                ExactWeekdayOfMonth = sch.ExactWeekdayOfMonth,
                ExactWeekdayOfMonthEvery = sch.ExactWeekdayOfMonthEvery,
                Frequency = sch.Frequency,
                MonthlyOccurrence = sch.MonthlyOccurrence,
                Venue = sch.Venue,
                OccursEveryTimeUnit = sch.OccursEveryTimeUnit,
                OccursEveryValue = sch.OccursEveryValue,
                RecurseEvery = sch.RecurseEvery,
                StartDate = sch.StartDate,
                TimeEnd = sch.TimeEnd,
                TimeStart = sch.TimeStart,
                Type = sch.Type,
                ValidDays = sch.ValidDays,
                ColorCode = sch.ColorCode,
                Duration = sch.Duration,
                MeetingUrl = sch.MeetingUrl

            };


            return new SessionDto()
            {
                Deadline = x.Deadline,
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                ModeId = x.ModeId,
                //MessageStatus = x.Sta,
                SessionScheduler = sessionScheduler,
                IsActive = x.IsActive,
                SessionParticipant = x.SessionParticipant.Select(s => new SessionParticipantDto()
                {
                    Id = s.Id,
                    ParticipantTyprId = s.ParticipantTyprId,
                    TypeId = s.TypeId,
                    UserId = s.UserId,
                    IsAdded = true
                }).ToList(),
                SessionReminder = x.SessionReminder.Select(s => new SessionReminderDto()
                {
                    Id = s.Id,
                    Every = s.Every,
                    TypeId = s.TypeId,
                    Unit = s.Unit
                }).ToList()

            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(SessionDto dto)
        {
            if (dto.Id.HasValue)
            {
                var look = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.SessionScheduler)
             .Include(m => m.SessionReminder).Include(m => m.SessionParticipant));

                if (look.SessionParticipant != null)
                {
                    var existingAttribute = look.SessionParticipant.Where(x => dto.SessionParticipant.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedAttribute = look.SessionParticipant.Where(x => !dto.SessionParticipant.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedAttribute = dto.SessionParticipant.Where(x => !look.SessionParticipant.Any(m => m.Id == x.Id)).ToList();


                    if (deletedAttribute.Any())
                    {
                        this.repoParticipant.DeleteRange(deletedAttribute);
                    }

                    if (existingAttribute.Any())
                    {
                        foreach (var e in existingAttribute)
                        {
                            var record = dto.SessionParticipant.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId;
                            e.TypeId = record.TypeId;
                            e.ParticipantTyprId = record.ParticipantTyprId.Value;
                            repoParticipant.Update(e);
                        }
                    }
                    if (insertedAttribute.Any())
                    {
                        List<SessionParticipant> addrecords = insertedAttribute.Select(a => new SessionParticipant()
                        {
                            SessionId = dto.Id.Value,
                            ParticipantTyprId = a.ParticipantTyprId.Value,
                            TypeId = a.TypeId,
                            UserId = a.UserId
                        }).ToList();

                        await repoParticipant.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.SessionReminder != null)
                {
                    var existingGame = look.SessionReminder.Where(x => dto.SessionReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = look.SessionReminder.Where(x => !dto.SessionReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.SessionReminder.Where(x => !look.SessionReminder.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {
                        this.repoReminder.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {
                            var record = dto.SessionReminder.FirstOrDefault(a => a.Id == e.Id);
                            e.TypeId = record.TypeId.Value;
                            e.Unit = record.Unit.Value;
                            e.Every = record.Every.Value;
                            repoReminder.Update(e);
                        }
                    }
                    if (insertedGame.Any())
                    {
                        List<SessionReminder> addrecords = insertedGame.Select(a => new SessionReminder()
                        {
                            SessionId = dto.Id.Value,
                            Every = a.Every.Value,
                            TypeId = a.TypeId.Value,
                            Unit = a.Unit.Value
                        }).ToList();

                        await repoReminder.InsertRangeAsyn(addrecords);
                    }
                }

                repoScheduler.Delete(look.SessionScheduler);

                repoScheduler.Insert(new SessionScheduler()
                {
                    DailyFrequency = dto.SessionScheduler.DailyFrequency,
                    DaysOfWeek = dto.SessionScheduler.DaysOfWeek,
                    EndDate = dto.SessionScheduler.EndDate,
                    ExactDateOfMonth = dto.SessionScheduler.ExactDateOfMonth,
                    ExactWeekdayOfMonth = dto.SessionScheduler.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = dto.SessionScheduler.ExactWeekdayOfMonthEvery,
                    Frequency = dto.SessionScheduler.Frequency,
                    MonthlyOccurrence = dto.SessionScheduler.MonthlyOccurrence,
                    Venue = dto.SessionScheduler.Venue,
                    OccursEveryTimeUnit = dto.SessionScheduler.OccursEveryTimeUnit,
                    OccursEveryValue = dto.SessionScheduler.OccursEveryValue,
                    RecurseEvery = dto.SessionScheduler.RecurseEvery,
                    StartDate = dto.SessionScheduler.StartDate.Value,
                    TimeEnd = dto.SessionScheduler.TimeEnd,
                    TimeStart = dto.SessionScheduler.TimeStart.Value,
                    Type = dto.SessionScheduler.Type,
                    ValidDays = dto.SessionScheduler.ValidDays,
                    SessionId = dto.Id.Value,
                    ColorCode = dto.SessionScheduler.ColorCode,
                    Duration = dto.SessionScheduler.Duration,
                    MeetingUrl = dto.SessionScheduler.MeetingUrl
                });

                look.Name = dto.Name;
                look.Deadline = dto.Deadline;
                look.Desciption = dto.Desciption;
                look.GameId = dto.GameId;
                look.TypeId = dto.TypeId.Value;
                look.ModeId = dto.ModeId.Value;
                look.ModifiedDate = DateTime.Now;
                look.IsActive = dto.IsActive;
                repo.Update(look);
                return dto.Id.Value;
            }
            else
            {
                var lookInfo = new Session()
                {
                    Deadline = dto.Deadline,
                    CompanyId = dto.CompanyId.Value,
                    CreatedBy = dto.CreatedBy,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    TypeId = dto.TypeId.Value,
                    ModeId = dto.ModeId.Value,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    IsActive = true,
                    SessionParticipant = dto.SessionParticipant.Select(a => new SessionParticipant()
                    {

                        ParticipantTyprId = a.ParticipantTyprId.Value,
                        TypeId = a.TypeId,
                        UserId = a.UserId
                    }).ToList(),
                    SessionReminder = dto.SessionReminder.Select(a => new SessionReminder()
                    {

                        Every = a.Every.Value,
                        TypeId = a.TypeId.Value,
                        Unit = a.Unit.Value
                    }).ToList(),

                };

                lookInfo.SessionScheduler = new SessionScheduler()
                {
                    DailyFrequency = dto.SessionScheduler.DailyFrequency,
                    DaysOfWeek = dto.SessionScheduler.DaysOfWeek,
                    EndDate = dto.SessionScheduler.EndDate,
                    ExactDateOfMonth = dto.SessionScheduler.ExactDateOfMonth,
                    ExactWeekdayOfMonth = dto.SessionScheduler.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = dto.SessionScheduler.ExactWeekdayOfMonthEvery,
                    Frequency = dto.SessionScheduler.Frequency,
                    MonthlyOccurrence = dto.SessionScheduler.MonthlyOccurrence,
                    Venue = dto.SessionScheduler.Venue,
                    OccursEveryTimeUnit = dto.SessionScheduler.OccursEveryTimeUnit,
                    OccursEveryValue = dto.SessionScheduler.OccursEveryValue,
                    RecurseEvery = dto.SessionScheduler.RecurseEvery,
                    StartDate = dto.SessionScheduler.StartDate.Value,
                    TimeEnd = dto.SessionScheduler.TimeEnd,
                    TimeStart = dto.SessionScheduler.TimeStart.Value,
                    Type = dto.SessionScheduler.Type,
                    ValidDays = dto.SessionScheduler.ValidDays,
                    ColorCode = dto.SessionScheduler.ColorCode,
                    Duration = dto.SessionScheduler.Duration,
                    MeetingUrl = dto.SessionScheduler.MeetingUrl
                };


                await repo.InsertAsync(lookInfo);
                return lookInfo.Id;
            }

        }

        public List<SessionDto> GetAllByGameId(int companyId, int gameId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId).Select(x => new SessionDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                ModeId = x.ModeId,
                IsActive = x.IsActive
            }).ToList();

        }

        public List<SessionDto> GetAllByGameId(int companyId, int? userId, int gameId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && (userId.HasValue ? x.CreatedBy == userId : true) && x.GameId == gameId).Select(x => new SessionDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                ModeId = x.ModeId,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy
            }).ToList();

        }

        public List<SessionDto> GetAllByCompanyIdAsync(int companyId)
        {
            var allSession = repo.GetAllIncluding(x => x.CompanyId == companyId, x => x.Include(m => m.Game).Include(m => m.SessionScheduler)).ToList();
            List<SessionDto> modelList = new List<SessionDto>();
            foreach (var x in allSession)
            {
                var user = x.CreatedBy.HasValue ? repoUserLogin.Get(x.CreatedBy.Value) : null;
                modelList.Add(new SessionDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    GameId = x.GameId,
                    TypeId = x.TypeId,
                    ModeId = x.ModeId,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.AddedDate,
                    CreatedByStr = x.CreatedBy.HasValue ? $"{user.Fname} {user.Lname}" : string.Empty,
                    Game = x.GameId.HasValue ? x.Game.Name : string.Empty,
                    SessionScheduler = new SessionSchedulerDto()
                    {
                        StartDate = x.SessionScheduler.StartDate,
                        EndDate = x.SessionScheduler.EndDate
                    }

                });
            }
            return modelList;

        }

        public List<SessionDto> GetAllByCompanyId(int companyId, int? userId, int gameId)
        {


            var allSession = repo.GetAllIncluding(x => x.CompanyId == companyId && x.GameId == gameId && (userId.HasValue ? x.CreatedBy == userId : true), x => x.Include(m => m.Game).Include(m => m.SessionScheduler));
            List<SessionDto> modelList = new List<SessionDto>();
            foreach (var x in allSession)
            {

                modelList.Add(new SessionDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    GameId = x.GameId,
                    TypeId = x.TypeId,
                    ModeId = x.ModeId,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.AddedDate,
                    Game = x.Game.Name,
                    SessionScheduler = x.SessionScheduler != null ? new SessionSchedulerDto()
                    {
                        StartDate = x.SessionScheduler.StartDate,
                        EndDate = x.SessionScheduler.EndDate
                    } : new SessionSchedulerDto()

                });
            }
            return modelList;

        }

        public void DeleteById(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var x = this.repo.GetIncludingById(x => x.Id == id, x => x
                .Include(m => m.SessionScheduler)
                .Include(m => m.SessionReminder)
                .Include(m => m.SessionParticipant));
                repoScheduler.Delete(x.SessionScheduler);
                repoReminder.DeleteRange(x.SessionReminder.ToList());
                repoParticipant.DeleteRange(x.SessionParticipant.ToList());
                repo.Delete(x);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw ex;
            }
        }

        public List<SessionScheduleEventDto> GetCompanySesstionEvent(int companyId, int? gameId, DateTime from, DateTime to)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanySesstionEvent]",
                          new SqlParameter("@companyId", companyId),
                          new SqlParameter("@gameId", gameId),
                          new SqlParameter("@userId", null),
                          new SqlParameter("@fromDate", from),
                          new SqlParameter("@toDate", to));

            List<SessionScheduleEventDto> respData = new List<SessionScheduleEventDto>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                respData = SqlHelper.ConvertDataTable<SessionScheduleEventDto>(dt);
            }

            return respData;
        }

        public SessionEventDetails GetSesstionEventDetails(int id, DateTime from, DateTime to, int? userId)
        {
            SessionEventDetails sessionEvent = new SessionEventDetails()
            {
                SessionId = id,
                StartDate = from,
                EndDate = to
            };
            var sess = repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.SessionScheduler));
            sessionEvent.SessionName = sess.Name;
            sessionEvent.GMeetUrl = sess.SessionScheduler != null ? sess.SessionScheduler.MeetingUrl : string.Empty;
            var part = repoParticipant.GetAllIncluding(x => x.SessionId == id, x => x.Include(m => m.User));
            sessionEvent.IsCoordinator = part.Any(x => x.UserId == userId && x.ParticipantTyprId == (int)ParticipantType.Coordinator);
            sessionEvent.Coordinator = string.Join(",", part.Where(x => x.ParticipantTyprId == (int)ParticipantType.Coordinator).Select(s => s.User.Fname + " " + s.User.Lname).ToList());

            sessionEvent.Manager = string.Join(",", part.Where(x => x.ParticipantTyprId == (int)ParticipantType.DecisionMaker).Select(s => s.User.Fname + " " + s.User.Lname).ToList());

            sessionEvent.Confirmed = part.Count(x => x.Status == (int)ConfirmStatus.Confirmed);
            sessionEvent.Rejected = part.Count(x => x.Status == (int)ConfirmStatus.Rejected);
            sessionEvent.Pending = part.Count(x => x.Status == (int)ConfirmStatus.Pending);

            return sessionEvent;
        }

        public List<SessionScheduleEventDto> GetCompanySesstionEvent(int companyId, int? userId, int? gameId, DateTime from, DateTime to)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanySesstionEvent]",
                          new SqlParameter("@companyId", companyId),
                            new SqlParameter("@gameId", gameId),
                          new SqlParameter("@userId", userId),
                          new SqlParameter("@fromDate", from),
                          new SqlParameter("@toDate", to));

            List<SessionScheduleEventDto> respData = new List<SessionScheduleEventDto>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                respData = SqlHelper.ConvertDataTable<SessionScheduleEventDto>(dt);
            }
            return respData;
        }

        public DataTable GetCompanySessionReminderEvent(DateTime currentDateTime, int reminterType)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanySessionReminderNotification]",
                          new SqlParameter("@currentDateTime", currentDateTime),
                          new SqlParameter("@reminterType", reminterType));

            DataTable dt = null;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        public List<StatusFeedbackReminderDto> GetSessionParticipantReminderEvent(DateTime currentDateTime, int reminterType)
        {
            DataTable dt = GetCompanySessionReminderEvent(currentDateTime, reminterType);
            List<int> sessionId = new List<int>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    sessionId.Add(int.Parse(row[0].ToString()));
                }

                return repoParticipant.GetAllIncluding(x => sessionId.Contains(x.SessionId) && x.UserId.HasValue, x => x.Include(m => m.Session).Include(m => m.User).Include(x => x.Session.SessionScheduler)).Select(x => new StatusFeedbackReminderDto()
                {
                    Id = x.SessionId,
                    UserId = x.UserId.Value,
                    Email = x.User.Email,
                    Name = x.Session.Name,
                    MeetingUrl = x.Session.SessionScheduler.MeetingUrl
                }).ToList();
            }
            else
            {
                return new List<StatusFeedbackReminderDto>();
            }

        }

        public async Task<SessionDto> GetSessionByUserId(int sessionId, int userId)
        {
            SessionDto sessionDto = new SessionDto();
            var session = await this.repo.GetIncludingByIdAsyn(x => x.Id == sessionId, x => x.Include(m => m.SessionParticipant));
            var sessionParticipant = session.SessionParticipant.Where(x => x.SessionId == sessionId && x.UserId == userId).FirstOrDefault();
            if (sessionParticipant != null)
            {
                sessionDto.Id = sessionParticipant.Session.Id;
                sessionDto.Name = sessionParticipant.Session.Name;
                sessionDto.MessageStatus = sessionParticipant.Status;
            }
            return sessionDto;
        }

        public SessionParticipant GetSessionParticipantByUserId(int sessionId, int userId)
        {
            return this.repoParticipant.GetAll(x => x.SessionId == sessionId && x.UserId == userId).FirstOrDefault();
        }

        public List<SessionParticipant> GetSessionDecisionMaker(int sessionId)
        {
            return this.repoParticipant.GetAll(x => x.SessionId == sessionId && x.ParticipantTyprId == (int)ParticipantType.DecisionMaker).ToList();
        }

        public List<SessionParticipant> GetAllDecisionParticipant(int sessionId)
        {
            return this.repoParticipant.GetAll(x => x.SessionId == sessionId).ToList();
        }

        public List<SessionParticipant> GetSessionParticipantNonMake(int sessionId)
        {
            return this.repoParticipant.GetAll(x => x.SessionId == sessionId && x.ParticipantTyprId != (int)ParticipantType.DecisionMaker).ToList();
        }

        public bool IsDecisionMaker(int userId, int sessionId)
        {
            return this.repoParticipant.Any(x => x.SessionId == sessionId && x.UserId == userId && x.ParticipantTyprId == (int)ParticipantType.DecisionMaker);
        }

        public bool IsCoordinator(int userId, int sessionId)
        {
            return this.repoParticipant.Any(x => x.SessionId == sessionId && x.UserId == userId && x.ParticipantTyprId == (int)ParticipantType.Coordinator);
        }

        public void UpdateSessionParticipant(SessionParticipant entity)
        {
            this.repoParticipant.Update(entity);
        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {


            IQueryable<Session> query = repo.GetAllIncluding(x => x.CompanyId == parameters.CompanyId && x.GameId == parameters.GameId && (parameters.UserType == (int)UserType.User ? x.CreatedBy == parameters.UserId : true), x => x.Include(m => m.Game).Include(m => m.SessionScheduler));

            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<SessionGridDto> data = new List<SessionGridDto>();
            foreach (Session x in result.Data)
            {
                data.Add(new SessionGridDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    GameId = x.Game.Name,
                    TypeId = ((StatusType)x.TypeId).ToString(),
                    ModeId = ((StatusModeType)x.TypeId).ToString(),
                    IsActive = x.IsActive ? "Active" : "In active",
                    CreatedDate = x.AddedDate.ToString("dd/MM/yyyy"),
                    StartDate = x.SessionScheduler != null ? x.SessionScheduler.StartDate.ToString("dd/MM/yyyy") : "",
                    EndDate = x.SessionScheduler != null && x.SessionScheduler.EndDate.HasValue ? x.SessionScheduler.EndDate.Value.ToString("dd/MM/yyyy") : ""
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}

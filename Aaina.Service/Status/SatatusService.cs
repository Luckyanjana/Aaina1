using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class StatusService : IStatusService
    {
        private IRepository<Status, int> repo;
        private IRepository<StatusScheduler, int> repoScheduler;
        private IRepository<StatusReminder, int> repoReminder;
        private IRepository<StatusGameBy, int> repoGameBy;
        private IRepository<StatusTeamBy, int> repoTeamBy;
        private IRepository<StatusUserBy, int> repoUserBy;
        private IRepository<StatusTeamFor, int> repoTeamFor;
        private IRepository<StatusUserFor, int> repoUserFor;
        private IRepository<Game, int> repoGame;
        private IRepository<Team, int> repoTeam;
        private IRepository<UserLogin, int> repoUser;
        private IRepository<StatusFeedback, int> repostatusFeedback;
        private IRepository<StatusFeedbackDetail, int> repostatusFeedbackDetail;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        public StatusService(IConfiguration configuration, IRepository<Status, int> repo, IRepository<StatusScheduler, int> repoScheduler,
            IRepository<StatusReminder, int> repoReminder, IRepository<StatusGameBy, int> repoGameBy, IRepository<StatusTeamBy, int> repoTeamBy,
            IRepository<StatusUserBy, int> repoUserBy, IRepository<StatusTeamFor, int> repoTeamFor, IRepository<StatusUserFor, int> repoUserFor, IRepository<Game, int> repoGame,
            IRepository<Team, int> repoTeam, IRepository<UserLogin, int> repoUser, IRepository<StatusFeedback, int> repostatusFeedback, IRepository<StatusFeedbackDetail, int> repostatusFeedbackDetail)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoScheduler = repoScheduler;
            this.repoReminder = repoReminder;
            this.repoGameBy = repoGameBy;
            this.repoTeamBy = repoTeamBy;
            this.repoUserBy = repoUserBy;
            this.repoTeamFor = repoTeamFor;
            this.repoUserFor = repoUserFor;
            this.repoGame = repoGame;
            this.repoTeam = repoTeam;
            this.repoUser = repoUser;
            this.repostatusFeedback = repostatusFeedback;
            this.repostatusFeedbackDetail = repostatusFeedbackDetail;
        }

        public async Task<StatusDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.StatusScheduler)
            .Include(m => m.StatusReminder).Include(m => m.StatusGameBy).Include(m => m.StatusTeamBy).Include(m => m.StatusUserBy)
            .Include(m => m.StatusTeamFor).Include(m => m.StatusUserFor));
            StatusSchedulerDto statusScheduler = new StatusSchedulerDto();

            StatusScheduler sch = x.StatusScheduler;
            statusScheduler = new StatusSchedulerDto()
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
                Duration = sch.Duration
            };


            return new StatusDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                StatusModeId = x.StatusId,
                StatusScheduler = statusScheduler,
                IsActive = x.IsActive,
                EstimatedTime = x.EstimatedTime,
                StatusReminder = x.StatusReminder.Select(s => new StatusReminderDto()
                {
                    Id = s.Id,
                    Every = s.Every,
                    TypeId = s.TypeId,
                    Unit = s.Unit
                }).ToList(),
                TeamByIds = x.StatusTeamBy.Select(s => s.TeamId).ToList(),
                GameByIds = x.StatusGameBy.Select(s => s.GameId).ToList(),
                UserByIds = x.StatusUserBy.Select(s => s.UserId).ToList(),
                TeamForIds = x.StatusTeamFor.Select(s => s.TeamId).ToList(),
                UserForIds = x.StatusUserFor.Select(s => s.UserId).ToList(),

            };
        }

        public async Task<StatusFeedbackDto> GetFeedbackDetailsById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.StatusGameBy).Include(m => m.StatusTeamBy).Include(m => m.StatusUserBy)
            .Include(m => m.StatusTeamFor).Include(m => m.StatusUserFor));
            StatusSchedulerDto statusScheduler = new StatusSchedulerDto();


            List<string> byGame = new List<string>();
            List<string> byTeam = new List<string>();
            List<string> byUser = new List<string>();

            List<string> forTeam = new List<string>();
            List<string> forUser = new List<string>();

            foreach (var item in x.StatusGameBy)
            {
                byGame.Add(repoGame.Get(item.GameId).Name);
            }

            foreach (var item in x.StatusTeamBy)
            {
                byTeam.Add(repoTeam.Get(item.TeamId).Name);
            }

            foreach (var item in x.StatusUserBy)
            {
                var user = repoUser.Get(item.UserId);
                byUser.Add(user.Fname + " " + user.Lname);
            }

            foreach (var item in x.StatusTeamFor)
            {
                forTeam.Add(repoTeam.Get(item.TeamId).Name);
            }

            foreach (var item in x.StatusUserFor)
            {
                var user = repoUser.Get(item.UserId);
                forUser.Add(user.Fname + " " + user.Lname);
            }

            return new StatusFeedbackDto()
            {
                Name = x.Name,
                EstimatedTime = x.EstimatedTime,
                Id = x.Id,
                By = string.Join(",", byGame) + "/" + string.Join(",", byTeam) + "/" + string.Join(",", byUser),
                For = string.Join(",", forTeam) + "/" + string.Join(",", forUser)
            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(StatusDto dto)
        {
            if (dto.Id.HasValue)
            {
                var status = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.StatusScheduler)
            .Include(m => m.StatusReminder).Include(m => m.StatusGameBy).Include(m => m.StatusTeamBy).Include(m => m.StatusUserBy)
            .Include(m => m.StatusTeamFor).Include(m => m.StatusUserFor));

                if (status.StatusGameBy != null && status.StatusGameBy.Any())
                {
                    this.repoGameBy.DeleteRange(status.StatusGameBy.ToList());
                }

                if (status.StatusTeamBy != null && status.StatusTeamBy.Any())
                {
                    this.repoTeamBy.DeleteRange(status.StatusTeamBy.ToList());
                }

                if (status.StatusUserBy != null && status.StatusUserBy.Any())
                {
                    this.repoUserBy.DeleteRange(status.StatusUserBy.ToList());
                }

                if (status.StatusTeamFor != null && status.StatusTeamFor.Any())
                {
                    this.repoTeamFor.DeleteRange(status.StatusTeamFor.ToList());
                }

                if (status.StatusUserFor != null && status.StatusUserFor.Any())
                {
                    this.repoUserFor.DeleteRange(status.StatusUserFor.ToList());
                }

                if (status.StatusReminder != null)
                {
                    var existingGame = status.StatusReminder.Where(x => dto.StatusReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = status.StatusReminder.Where(x => !dto.StatusReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.StatusReminder.Where(x => !status.StatusReminder.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {
                        this.repoReminder.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {
                            var record = dto.StatusReminder.FirstOrDefault(a => a.Id == e.Id);
                            e.TypeId = record.TypeId.Value;
                            e.Unit = record.Unit.Value;
                            e.Every = record.Every.Value;
                            repoReminder.Update(e);
                        }
                    }
                    if (insertedGame.Any())
                    {
                        List<StatusReminder> addrecords = insertedGame.Select(a => new StatusReminder()
                        {
                            StatusId = dto.Id.Value,
                            Every = a.Every.Value,
                            TypeId = a.TypeId.Value,
                            Unit = a.Unit.Value
                        }).ToList();

                        await repoReminder.InsertRangeAsyn(addrecords);
                    }
                }
                else
                {
                    status.StatusReminder = dto.StatusReminder.Select(a => new StatusReminder()
                    {

                        Every = a.Every.Value,
                        TypeId = a.TypeId.Value,
                        Unit = a.Unit.Value,
                        StatusId = dto.Id.Value
                    }).ToList();
                }

                repoScheduler.Delete(status.StatusScheduler);

                repoScheduler.Insert(new StatusScheduler()
                {
                    DailyFrequency = dto.StatusScheduler.DailyFrequency,
                    DaysOfWeek = dto.StatusScheduler.DaysOfWeek,
                    EndDate = dto.StatusScheduler.EndDate,
                    ExactDateOfMonth = dto.StatusScheduler.ExactDateOfMonth,
                    ExactWeekdayOfMonth = dto.StatusScheduler.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = dto.StatusScheduler.ExactWeekdayOfMonthEvery,
                    Frequency = dto.StatusScheduler.Frequency,
                    MonthlyOccurrence = dto.StatusScheduler.MonthlyOccurrence,
                    Venue = "test",//dto.StatusScheduler.Venue,
                    OccursEveryTimeUnit = dto.StatusScheduler.OccursEveryTimeUnit,
                    OccursEveryValue = dto.StatusScheduler.OccursEveryValue,
                    RecurseEvery = dto.StatusScheduler.RecurseEvery,
                    StartDate = dto.StatusScheduler.StartDate.Value,
                    TimeEnd = dto.StatusScheduler.TimeEnd,
                    TimeStart = dto.StatusScheduler.TimeStart.Value,
                    Type = dto.StatusScheduler.Type,
                    ValidDays = dto.StatusScheduler.ValidDays,
                    StatusId = dto.Id.Value,
                    ColorCode = dto.StatusScheduler.ColorCode,
                    Duration = dto.StatusScheduler.Duration
                });
                status.EstimatedTime = dto.EstimatedTime;
                status.Name = dto.Name;
                status.Desciption = dto.Desciption;
                status.GameId = dto.GameId;
                status.StatusId = dto.StatusModeId.Value;
                status.ModifiedDate = DateTime.Now;
                status.IsActive = dto.IsActive;
                status.StatusGameBy = dto.GameByIds.Select(a => new StatusGameBy()
                {
                    GameId = a,
                    StatusId = dto.Id.Value
                }).ToList();
                status.StatusTeamBy = dto.TeamByIds.Select(a => new StatusTeamBy()
                {
                    TeamId = a,
                    StatusId = dto.Id.Value
                }).ToList();
                status.StatusUserBy = dto.UserByIds.Select(a => new StatusUserBy()
                {
                    UserId = a,
                    StatusId = dto.Id.Value
                }).ToList();
                status.StatusTeamFor = dto.TeamForIds.Select(a => new StatusTeamFor()
                {
                    TeamId = a,
                    StatusId = dto.Id.Value
                }).ToList();
                status.StatusUserFor = dto.UserForIds.Select(a => new StatusUserFor()
                {
                    UserId = a,
                    StatusId = dto.Id.Value

                }).ToList();
                repo.Update(status);
                return dto.Id.Value;
            }
            else
            {
                var statusInfo = new Status()
                {
                    EstimatedTime = dto.EstimatedTime,
                    CompanyId = dto.CompanyId.Value,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    StatusId = dto.StatusModeId.Value,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    CreatedBy = dto.CreatedBy,
                    IsActive = true,
                    StatusReminder = dto.StatusReminder.Select(a => new StatusReminder()
                    {

                        Every = a.Every.Value,
                        TypeId = a.TypeId.Value,
                        Unit = a.Unit.Value
                    }).ToList(),
                    StatusGameBy = dto.GameByIds.Select(a => new StatusGameBy()
                    {
                        GameId = a
                    }).ToList(),
                    StatusTeamBy = dto.TeamByIds.Select(a => new StatusTeamBy()
                    {
                        TeamId = a
                    }).ToList(),
                    StatusUserBy = dto.UserByIds.Select(a => new StatusUserBy()
                    {
                        UserId = a
                    }).ToList(),
                    StatusTeamFor = dto.TeamForIds.Select(a => new StatusTeamFor()
                    {
                        TeamId = a
                    }).ToList(),
                    StatusUserFor = dto.UserForIds.Select(a => new StatusUserFor()
                    {
                        UserId = a
                    }).ToList()
                };

                statusInfo.StatusScheduler = new StatusScheduler()
                {
                    DailyFrequency = dto.StatusScheduler.DailyFrequency,
                    DaysOfWeek = dto.StatusScheduler.DaysOfWeek,
                    EndDate = dto.StatusScheduler.EndDate,
                    ExactDateOfMonth = dto.StatusScheduler.ExactDateOfMonth,
                    ExactWeekdayOfMonth = dto.StatusScheduler.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = dto.StatusScheduler.ExactWeekdayOfMonthEvery,
                    Frequency = dto.StatusScheduler.Frequency,
                    MonthlyOccurrence = dto.StatusScheduler.MonthlyOccurrence,
                    Venue = "Test",//dto.StatusScheduler.Venue,
                    OccursEveryTimeUnit = dto.StatusScheduler.OccursEveryTimeUnit,
                    OccursEveryValue = dto.StatusScheduler.OccursEveryValue,
                    RecurseEvery = dto.StatusScheduler.RecurseEvery,
                    StartDate = dto.StatusScheduler.StartDate.Value,
                    TimeEnd = dto.StatusScheduler.TimeEnd,
                    TimeStart = dto.StatusScheduler.TimeStart.Value,
                    Type = dto.StatusScheduler.Type,
                    ValidDays = dto.StatusScheduler.ValidDays,
                    ColorCode = dto.StatusScheduler.ColorCode,
                    Duration = dto.StatusScheduler.Duration,

                };


                await repo.InsertAsync(statusInfo);
                return statusInfo.Id;
            }

        }

        public List<StatusDto> GetAllByGameId(int companyId, int gameId, int? userId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId &&
            (userId.HasValue ? x.CreatedBy == userId : true)).Select(x => new StatusDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                StatusModeId = x.StatusId,
                IsActive = x.IsActive,
                CreatedBy=x.CreatedBy
            }).ToList();

        }

        public List<StatusDto> GetAllByCompanyId(int companyId, int? userId,int? gameId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId &&(gameId.HasValue? x.GameId==gameId:true) && ((userId.HasValue ? x.CreatedBy == userId : true) || x.StatusUserBy.Any(a => a.UserId == userId) ||
            x.StatusGameBy.Any(a => a.Game.GamePlayer.Any(b => b.UserId == userId)) || x.StatusTeamBy.Any(a => a.Team.TeamPlayer.Any(b => b.UserId == userId)))).Select(x => new StatusDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                StatusModeId = x.StatusId,
                IsActive = x.IsActive,
                CreatedBy=x.CreatedBy
            }).ToList();

        }

        public void DeleteById(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();

                var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.StatusScheduler)
           .Include(m => m.StatusReminder).Include(m => m.StatusGameBy).Include(m => m.StatusTeamBy).Include(m => m.StatusUserBy)
           .Include(m => m.StatusTeamFor).Include(m => m.StatusUserFor));
                repoScheduler.Delete(x.StatusScheduler);
                repoReminder.DeleteRange(x.StatusReminder.ToList());
                repoGameBy.DeleteRange(x.StatusGameBy.ToList());
                repoTeamBy.DeleteRange(x.StatusTeamBy.ToList());
                repoUserBy.DeleteRange(x.StatusUserBy.ToList());
                repoTeamFor.DeleteRange(x.StatusTeamFor.ToList());
                repoUserFor.DeleteRange(x.StatusUserFor.ToList());
                repo.Delete(x);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw ex;
            }
        }

        public List<StatusFeedbackReminderDto> GetStatusParticipantReminderEventEmail(DateTime currentDateTime, int reminterType)
        {
            return GetStatusSessionReminderEvent(currentDateTime, reminterType);
        }


        public async Task<int> AddFeedbackAsync(StatusFeedbackPostDto dto)
        {
            var entity = await repostatusFeedback.InsertAsync(
                    new StatusFeedback()
                    {
                        CreatedDate = DateTime.Now,
                        StatusId = dto.Id.Value,
                        UserId = dto.UserId,
                        ActualTime = dto.ActualTime,
                        StatusFeedbackDetail = dto.Feedback.Select(s => new StatusFeedbackDetail()
                        {
                            Emotion = EncryptDecrypt.Encrypt(s.Emotion.ToString()),
                            Feedback = s.Feedback,
                            FeedbackDate = s.FeedbackDate,
                            GameId = s.GameId,
                            Progress = s.Progress,
                            SubGameId = s.SubGameId,
                            Status = s.Status
                        }).ToList()
                    }
                    );
            return entity.Id;

        }

        public async Task<int> AddNonStatusAsync(NonStatusDto dto)
        {
            var statusInfo = new Status()
            {
                CompanyId = dto.CompanyId.Value,
                Desciption = "",
                Name = dto.Name,
                StatusId = dto.StatusModeId.Value,
                ModifiedDate = DateTime.Now,
                AddedDate = DateTime.Now,
                GameId = dto.GameId,
                CreatedBy = dto.CreatedBy,
                EstimatedTime = dto.EstimatedTime,
                IsActive = true,

                StatusUserBy = new List<StatusUserBy>()
                {
                    new StatusUserBy()
                    {
                        UserId = dto.CreatedBy.Value
                    }
                },

                StatusTeamFor = dto.TeamForIds.Select(a => new StatusTeamFor()
                {
                    TeamId = a
                }).ToList(),
                StatusUserFor = dto.UserForIds.Select(a => new StatusUserFor()
                {
                    UserId = a
                }).ToList()
            };

            statusInfo.StatusScheduler = new StatusScheduler()
            {

                EndDate = DateTime.Now,
                Venue = "Test",//dto.StatusScheduler.Venue,
                StartDate = DateTime.Now,
                TimeEnd = DateTime.Now.TimeOfDay,
                TimeStart = DateTime.Now.TimeOfDay,
                Type = (int)ScheduleType.OneTime
            };

            await repo.InsertAsync(statusInfo);
            return statusInfo.Id;

        }

        public async Task<List<StatusFeedbackDisplayDto>> ViewResult(int id, int? userId)
        {

            var result = await repostatusFeedback.GetAllIncludingAsyn(x => x.StatusId == id &&
             (userId.HasValue ? x.UserId == userId : true), x => x.Include(m => m.StatusFeedbackDetail).Include(m => m.User)
             .Include("StatusFeedbackDetail.Game").Include("StatusFeedbackDetail.SubGame"));

            return result.Select(x => new StatusFeedbackDisplayDto()
            {
                ActualTime = x.ActualTime,
                UserName = x.User.Fname + " " + x.User.Lname,
                Feedback = x.StatusFeedbackDetail.Select(s => new StatusFeedbackDetailDisplayDto()
                {
                    Emotion = int.Parse(EncryptDecrypt.Decrypt(s.Emotion)),
                    Feedback = s.Feedback,
                    FeedbackDate = s.FeedbackDate,
                    Game = s.Game.Name,
                    Progress = s.Progress,
                    Status = s.Status,
                    SubGame = s.SubGame.Name

                }).ToList()

            }).ToList();

        }

        public async Task<List<SelectedItemDto>> GetParticipentByStatusId(int statusId)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == statusId, x => x.Include(m => m.StatusGameBy).Include(m => m.StatusTeamBy).Include(m => m.StatusUserBy)
            .Include(m => m.StatusTeamFor).Include(m => m.StatusUserFor));
            List<SelectedItemDto> response = new List<SelectedItemDto>();


            foreach (var item in x.StatusGameBy)
            {
                var playes = repoGame.GetIncludingById(x => x.Id == item.GameId, x => x.Include(m => m.GamePlayer).Include("GamePlayer.User"));
                foreach (var ply in playes.GamePlayer)
                {

                    response.Add(new SelectedItemDto() { Name = ply.User.Fname + " " + ply.User.Lname, Id = ply.UserId.ToString() });
                }
            }

            foreach (var item in x.StatusTeamBy)
            {
                var playes = repoTeam.GetIncludingById(x => x.Id == item.TeamId, x => x.Include(m => m.TeamPlayer).Include("TeamPlayer.User"));
                foreach (var ply in playes.TeamPlayer)
                {

                    response.Add(new SelectedItemDto() { Name = ply.User.Fname + " " + ply.User.Lname, Id = ply.UserId.ToString() });
                }
            }

            foreach (var item in x.StatusUserBy)
            {
                var user = repoUser.Get(item.UserId);
                response.Add(new SelectedItemDto() { Name = user.Fname + " " + user.Lname, Id = item.UserId.ToString() });
            }


            return response.Distinct().ToList();


        }
        private List<StatusFeedbackReminderDto> GetStatusSessionReminderEvent(DateTime currentDateTime, int reminterType)
        {
            List<StatusFeedbackReminderDto> list = SqlHelper.ConvertDataTable<StatusFeedbackReminderDto>(SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetStatusReminderNotification]",
                        new SqlParameter("@currentDateTime", currentDateTime), new SqlParameter("@reminterType", reminterType)).Tables[0]);
            return list;
        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {


            IQueryable<Status> query = repo.GetAll(x => x.CompanyId == parameters.CompanyId && x.GameId == parameters.GameId && ((parameters.UserType == (int)UserType.User ? x.CreatedBy == parameters.UserId : true) || x.StatusUserBy.Any(a => a.UserId == parameters.UserId) ||
            x.StatusGameBy.Any(a => a.Game.GamePlayer.Any(b => b.UserId == parameters.UserId)) || x.StatusTeamBy.Any(a => a.Team.TeamPlayer.Any(b => b.UserId == parameters.UserId))));

            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<StatusGridDto> data = new List<StatusGridDto>();
            foreach (Status x in result.Data)
            {
                data.Add(new StatusGridDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    StatusModeId = ((StatusModeType)x.StatusId).ToString(),
                    IsActive = x.IsActive ? "Active" : "In active",
                    CreatedBy = x.CreatedBy
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}

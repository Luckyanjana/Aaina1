using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class PollService : IPollService
    {
        private readonly IConfiguration configuration;
        private IRepository<Poll, int> repo;
        private IRepository<PollQuestion, int> repoPollQuestion;
        private IRepository<PollQuestionOption, int> repoQuestionOption;
        private IRepository<PollParticipants, int> repoParticipant;
        private IRepository<PollReminder, int> repoReminder;
        private IRepository<PollScheduler, int> repoScheduler;
        private IRepository<PollFeedback, int> repoFeedback;
        private IRepository<PollQuestionFeedback, int> repoQuestionFeedback;
        private readonly string Connectionstring;
        public PollService(IConfiguration configuration, IRepository<Poll, int> repo, IRepository<PollReminder, int> repoReminder, IRepository<PollQuestion, int> repoPollQuestion,
            IRepository<PollQuestionOption, int> repoQuestionOption,
 IRepository<PollParticipants, int> repoParticipant, IRepository<PollScheduler, int> repoScheduler, IRepository<PollFeedback, int> repoFeedback, IRepository<PollQuestionFeedback, int> repoQuestionFeedback)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoReminder = repoReminder;
            this.repoQuestionOption = repoQuestionOption;
            this.repoParticipant = repoParticipant;
            this.repoScheduler = repoScheduler;
            this.repoFeedback = repoFeedback;
            this.repoPollQuestion = repoPollQuestion;
            this.repoQuestionFeedback = repoQuestionFeedback;
        }

        public List<PollDto> GetAllByCompanyId(int companyId)
        {
            return repo.GetAllIncluding(x => x.CompanyId == companyId, x => x.Include(m => m.PollFeedback)).Select(x => new PollDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                CreatedBy = x.CreatedBy,
                IsActive = x.IsActive,
                IsEditable = !x.PollFeedback.Any()
            }).ToList();
        }

        public List<PollDto> GetAllByCompanyId(int companyId, int? userid,int gameId)
        {
            return repo.GetAllIncluding(x => x.CompanyId == companyId && x.GameId==gameId && (userid.HasValue?(x.PollParticipants.Any(a => a.UserId == userid) || x.CreatedBy == userid):true),
                x => x.Include(m => m.PollFeedback)).Select(x => new PollDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    GameId = x.GameId,
                    CreatedBy = x.CreatedBy,
                    IsActive = x.IsActive,
                    IsEditable = !x.PollFeedback.Any()
                }).ToList();
        }

        public List<PollDto> GetAllByGameId(int companyId, int gameId)
        {
            return repo.GetAllIncluding(x => x.CompanyId == companyId && x.GameId == gameId,
                x => x.Include(m => m.PollFeedback)).Select(x => new PollDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    GameId = x.GameId,
                    IsActive = x.IsActive,
                    IsEditable = !x.PollFeedback.Any()
                }).ToList();

        }

        public async Task<PollDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.PollScheduler)
            .Include(m => m.PollQuestion).Include("PollQuestion.PollQuestionOption").Include(m => m.PollParticipants).Include(m => m.PollReminder));
            PollSchedulerDto reportScheduler = new PollSchedulerDto();
            if (x.PollScheduler != null)
            {
                PollScheduler sch = x.PollScheduler;
                reportScheduler = new PollSchedulerDto()
                {
                    DailyFrequency = sch.DailyFrequency,
                    DaysOfWeek = sch.DaysOfWeek,
                    EndDate = sch.EndDate,
                    ExactDateOfMonth = sch.ExactDateOfMonth,
                    ExactWeekdayOfMonth = sch.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = sch.ExactWeekdayOfMonthEvery,
                    Frequency = sch.Frequency,
                    MonthlyOccurrence = sch.MonthlyOccurrence,
                    OccursEveryTimeUnit = sch.OccursEveryTimeUnit,
                    OccursEveryValue = sch.OccursEveryValue,
                    RecurseEvery = sch.RecurseEvery,
                    StartDate = sch.StartDate,
                    TimeEnd = sch.TimeEnd,
                    TimeStart = sch.TimeStart,
                    Type = sch.Type,
                    ValidDays = sch.ValidDays
                };
            }

            return new PollDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                SubGameId = x.SubGameId,
                PollScheduler = reportScheduler,
                IsActive = x.IsActive,
                PollQuestion = x.PollQuestion.Select(s => new PollQuestionDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    QuestionTypeId = s.QuestionTypeId,
                    PollQuestionOption = s.PollQuestionOption.Select(c => new PollOptionDto()
                    {
                        FilePath = c.FilePath,
                        Name = c.Name,
                        Id = c.Id
                    }).ToList()
                }).ToList(),
                PollParticipants = x.PollParticipants.Select(s => new PollParticipantsDto()
                {
                    Id = s.Id,
                    TypeId = s.TypeId,
                    UserId = s.UserId,
                    IsAdded = true
                }).ToList(),

                PollReminder = x.PollReminder.Select(s => new PollReminderDto()
                {
                    Id = s.Id,
                    TypeId = s.TypeId,
                    Every = s.Every,
                    PollId = s.PollId,
                    Unit = s.Unit
                }).ToList()

            };
        }

        public async Task<List<SelectedItemDto>> GetParticipantByPollId(int id)
        {
            var partList = await this.repoParticipant.GetAllIncludingAsyn(x => x.PollId == id, x => x.Include(m => m.User));
            return partList.Select(x => new SelectedItemDto()
            {
                Id = x.UserId.ToString(),
                Name = x.User.Fname + " " + x.User.Lname
            }).ToList();

        }


        public async Task<PollFeedbackDisplayDto> GetFeedbackById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.PollQuestion).Include("PollQuestion.PollQuestionOption").Include(m => m.Game).Include(m => m.SubGame).Include(m => m.CreatedByNavigation));
            PollFeedbackDisplayDto reportScheduler = new PollFeedbackDisplayDto();

            return new PollFeedbackDisplayDto()
            {
                Name = x.Name,
                PollId = x.Id,
                Game = x.Game.Name,
                SubGame = x.SubGame.Name,
                UserId = x.CreatedBy,
                CreatedBy = x.CreatedByNavigation.Fname + " " + x.CreatedByNavigation.Lname,
                CreatedOn = x.AddedDate,
                QuestionList = x.PollQuestion.Select(s => new PollQuestionDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    QuestionTypeId = s.QuestionTypeId,
                    PollQuestionOption = s.PollQuestionOption.Select(o => new PollOptionDto()
                    {
                        FilePath = o.FilePath,
                        Name = o.Name,
                        Id = o.Id
                    }).ToList()
                }).ToList()

            };
        }

        public void AddFeedback(PollFeedbackAddDto dto)
        {
            repoFeedback.Insert(new PollFeedback()
            {
                CreatedDate = DateTime.Now,
                PollId = dto.PollId,
                UserId = dto.UserId,
                Remark = dto.Remark,
                PollQuestionFeedback = dto.QuestionList.Select(s => new PollQuestionFeedback()
                {
                    PollQuestionId = s.QuestionId.Value,
                    PollQuestionOptionId = s.AnswerId.Value,
                    Remark = s.Remark
                }).ToList()
            });
        }

        public async Task<PollDto> GetDetailsId(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new PollDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive
            };
        }

        public async Task<PollFeedbackDisplayDto> ViewResult(int id, int? userId)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.PollQuestion).Include(m => m.PollParticipants).Include("PollQuestion.PollQuestionOption").Include(m => m.Game).Include(m => m.SubGame).Include(m => m.CreatedByNavigation));

            double totalParti = (double)x.PollParticipants.Count;

            var result = repoQuestionFeedback.GetAllList(x => x.PollFeedback.PollId == id &&
            (userId.HasValue ? x.PollFeedback.UserId == userId : true)).ToList();

            PollFeedbackDisplayDto reportScheduler = new PollFeedbackDisplayDto();

            if (x != null)
            {

                return new PollFeedbackDisplayDto()
                {
                    Name = x.Name,
                    PollId = x.Id,
                    Game = x.Game.Name,
                    SubGame = x.SubGame.Name,
                    UserId = x.CreatedBy,
                    CreatedBy = x.CreatedByNavigation.Fname + " " + x.CreatedByNavigation.Lname,
                    CreatedOn = x.AddedDate,
                    QuestionList = x.PollQuestion.Select(s => new PollQuestionDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        QuestionTypeId = s.QuestionTypeId,
                        PollQuestionOption = s.PollQuestionOption.Select(o => new PollOptionDto()
                        {
                            FilePath = o.FilePath,
                            Name = o.Name,
                            Id = o.Id,
                            Per = result.Any(a => a.PollQuestionOptionId == o.Id) ? ((result.Count(a => a.PollQuestionOptionId == o.Id) / totalParti) * 100) : 0.0
                        }).ToList()
                    }).ToList()

                };
            }
            else
            {
                return new PollFeedbackDisplayDto();
            }
        }
        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(PollDto dto)
        {
            if (dto.Id.HasValue)
            {
                var poll = this.repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.PollScheduler)
            .Include(m => m.PollQuestion).Include("PollQuestion.PollQuestionOption").Include(m => m.PollParticipants).Include(m => m.PollReminder));

                if (poll.PollQuestion != null)
                {
                    var existingOption = poll.PollQuestion.Where(x => dto.PollQuestion.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedOption = poll.PollQuestion.Where(x => !dto.PollQuestion.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedOption = dto.PollQuestion.Where(x => !poll.PollQuestion.Any(m => m.Id == x.Id)).ToList();


                    if (deletedOption.Any())
                    {
                        foreach (var d in deletedOption)
                        {
                            this.repoQuestionOption.DeleteRange(d.PollQuestionOption.ToList());
                        }
                        this.repoPollQuestion.DeleteRange(deletedOption);
                    }

                    if (existingOption.Any())
                    {
                        foreach (var e in existingOption)
                        {
                            this.repoQuestionOption.DeleteRange(e.PollQuestionOption.ToList());

                            var record = dto.PollQuestion.FirstOrDefault(a => a.Id == e.Id);
                            e.Name = record.Name;
                            e.QuestionTypeId = record.QuestionTypeId;
                            e.PollQuestionOption = record.PollQuestionOption.Select(s => new PollQuestionOption()
                            {
                                FilePath = s.FilePath,
                                Name = s.Name,
                                PollQuestionId = record.Id.Value
                            }).ToList();
                            repoPollQuestion.Update(e);
                        }
                    }
                    if (insertedOption.Any())
                    {
                        List<PollQuestion> addrecords = insertedOption.Select(a => new PollQuestion()
                        {
                            PollId = dto.Id.Value,
                            Name = a.Name,
                            QuestionTypeId = a.QuestionTypeId,
                            PollQuestionOption = a.PollQuestionOption.Select(s => new PollQuestionOption()
                            {
                                FilePath = s.FilePath,
                                Name = s.Name
                            }).ToList()
                        }).ToList();
                        await repoPollQuestion.InsertRangeAsyn(addrecords);
                    }
                }

                if (poll.PollParticipants != null)
                {
                    var existingUser = poll.PollParticipants.Where(x => dto.PollParticipants.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedUser = poll.PollParticipants.Where(x => !dto.PollParticipants.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedUser = dto.PollParticipants.Where(x => !poll.PollParticipants.Any(m => m.Id == x.Id)).ToList();


                    if (deletedUser.Any())
                    {
                        this.repoParticipant.DeleteRange(deletedUser);
                    }

                    if (existingUser.Any())
                    {
                        foreach (var e in existingUser)
                        {
                            var record = dto.PollParticipants.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId.Value;
                            e.TypeId = record.TypeId;
                            repoParticipant.Update(e);
                        }
                    }
                    if (insertedUser.Any())
                    {
                        List<PollParticipants> addrecords = insertedUser.Select(a => new PollParticipants()
                        {
                            PollId = dto.Id.Value,
                            UserId = a.UserId.Value,
                            TypeId = a.TypeId
                        }).ToList();

                        await repoParticipant.InsertRangeAsyn(addrecords);
                    }
                }

                if (poll.PollReminder != null)
                {
                    var existingReminder = poll.PollReminder.Where(x => dto.PollReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedReminder = poll.PollReminder.Where(x => !dto.PollReminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedReminder = dto.PollReminder.Where(x => !poll.PollReminder.Any(m => m.Id == x.Id)).ToList();


                    if (deletedReminder.Any())
                    {
                        this.repoReminder.DeleteRange(deletedReminder);
                    }

                    if (existingReminder.Any())
                    {
                        foreach (var e in existingReminder)
                        {
                            var record = dto.PollReminder.FirstOrDefault(a => a.Id == e.Id);
                            e.Unit = record.Unit.Value;
                            e.Every = record.Every.Value;
                            e.TypeId = record.TypeId.Value;
                            repoReminder.Update(e);
                        }
                    }
                    if (insertedReminder.Any())
                    {
                        List<PollReminder> addrecords = insertedReminder.Select(a => new PollReminder()
                        {
                            PollId = dto.Id.Value,
                            Every = a.Every.Value,
                            TypeId = a.TypeId.Value,
                            Unit = a.Unit.Value
                        }).ToList();

                        await repoReminder.InsertRangeAsyn(addrecords);
                    }
                }



                if (poll.PollScheduler != null)
                {
                    repoScheduler.Delete(poll.PollScheduler);
                }

                if (dto.PollScheduler != null)
                {
                    repoScheduler.Insert(new PollScheduler()
                    {
                        DailyFrequency = dto.PollScheduler.DailyFrequency,
                        DaysOfWeek = dto.PollScheduler.DaysOfWeek,
                        EndDate = dto.PollScheduler.EndDate,
                        ExactDateOfMonth = dto.PollScheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.PollScheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.PollScheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.PollScheduler.Frequency,
                        MonthlyOccurrence = dto.PollScheduler.MonthlyOccurrence,
                        OccursEveryTimeUnit = dto.PollScheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.PollScheduler.OccursEveryValue,
                        RecurseEvery = dto.PollScheduler.RecurseEvery,
                        StartDate = dto.PollScheduler.StartDate.Value,
                        TimeEnd = dto.PollScheduler.TimeEnd,
                        TimeStart = dto.PollScheduler.TimeStart.Value,
                        Type = dto.PollScheduler.Type,
                        ValidDays = dto.PollScheduler.ValidDays,
                        PollId = dto.Id.Value
                    });
                }


                poll.Name = dto.Name;
                poll.Desciption = dto.Desciption;
                poll.GameId = dto.GameId;
                poll.SubGameId = dto.SubGameId;
                poll.IsActive = dto.IsActive;
                poll.ModifiedDate = DateTime.Now;
                repo.Update(poll);
                return dto.Id.Value;
            }
            else
            {
                var pollInfo = new Poll()
                {
                    CompanyId = dto.CompanyId.Value,
                    CreatedBy = dto.CreatedBy,
                    Desciption = dto.Desciption,
                    Name = dto.Name,

                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    SubGameId = dto.SubGameId,
                    IsActive = dto.IsActive,
                    PollQuestion = dto.PollQuestion.Select(a => new PollQuestion()
                    {
                        Name = a.Name,
                        QuestionTypeId = a.QuestionTypeId,
                        PollQuestionOption = a.PollQuestionOption.Select(s => new PollQuestionOption()
                        {
                            FilePath = s.FilePath,
                            Name = s.Name
                        }).ToList()
                    }).ToList(),
                    PollParticipants = dto.PollParticipants.Select(s => new PollParticipants()
                    {
                        UserId = s.UserId.Value,
                        TypeId = s.TypeId
                    }).ToList(),
                    PollReminder = dto.PollReminder.Select(s => new PollReminder()
                    {
                        Every = s.Every.Value,
                        TypeId = s.TypeId.Value,
                        Unit = s.Unit.Value
                    }).ToList(),

                };
                if (dto.PollScheduler != null)
                {
                    pollInfo.PollScheduler = new PollScheduler()
                    {
                        DailyFrequency = dto.PollScheduler.DailyFrequency,
                        DaysOfWeek = dto.PollScheduler.DaysOfWeek,
                        EndDate = dto.PollScheduler.EndDate,
                        ExactDateOfMonth = dto.PollScheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.PollScheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.PollScheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.PollScheduler.Frequency,
                        MonthlyOccurrence = dto.PollScheduler.MonthlyOccurrence,
                        OccursEveryTimeUnit = dto.PollScheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.PollScheduler.OccursEveryValue,
                        RecurseEvery = dto.PollScheduler.RecurseEvery,
                        StartDate = dto.PollScheduler.StartDate.Value,
                        TimeEnd = dto.PollScheduler.TimeEnd,
                        TimeStart = dto.PollScheduler.TimeStart.Value,
                        Type = dto.PollScheduler.Type,
                        ValidDays = dto.PollScheduler.ValidDays
                    };
                }

                await repo.InsertAsync(pollInfo);
                return pollInfo.Id;
            }

        }

        public void DeleteBy(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();

                var x =  this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.PollScheduler)
          .Include(m => m.PollQuestion).Include("PollQuestion.PollQuestionOption").Include(m => m.PollParticipants).Include(m => m.PollReminder));

                repoScheduler.Delete(x.PollScheduler);
                foreach (var item in x.PollQuestion)
                {
                    repoQuestionOption.DeleteRange(item.PollQuestionOption.ToList());
                }
                repoParticipant.DeleteRange(x.PollParticipants.ToList());
                repoReminder.DeleteRange(x.PollReminder.ToList());
                repo.Delete(x);

                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }
        }

        public List<StatusFeedbackReminderDto> GetPollReminderMailNotification(DateTime currentDateTime, int reminterType)
        {
            DataTable dt = GetPollReminderMail(currentDateTime, reminterType);
            List<int> reportTemplateId = new List<int>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    reportTemplateId.Add(int.Parse(row[0].ToString()));
                }

                return repoParticipant.GetAllIncluding(x => reportTemplateId.Contains(x.PollId), x => x.Include(x => x.Poll).Include(x => x.User)).Select(x => new StatusFeedbackReminderDto()
                {
                    Id = x.PollId,
                    UserId = x.UserId,
                    Email = x.User.Email,
                    Name = x.Poll.Name
                }).ToList();
            }
            else
            {
                return new List<StatusFeedbackReminderDto>();
            }
        }

        private DataTable GetPollReminderMail(DateTime currentDateTime, int reminterType)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanyPollReminderNotification]",
                          new SqlParameter("@currentDateTime", currentDateTime),
                          new SqlParameter("@reminterType", reminterType));

            DataTable dt = null;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }
    }
}
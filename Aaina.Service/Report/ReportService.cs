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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class ReportService : IReportService
    {
        private readonly IConfiguration configuration;
        private IRepository<ReportTemplate, int> repo;
        private IRepository<ReportTemplateGame, int> repoGame;
        private IRepository<ReportTemplateUser, int> repoUser;
        private IRepository<ReportTemplateReminder, int> repoReminder;
        private IRepository<ReportTemplateScheduler, int> repoScheduler;
        private IRepository<ReportGive, int> repoReportGive;
        private IRepository<ReportGiveDetails, int> repoReportGiveDetail;
        private IRepository<ReportGiveAttributeValue, int> repoGiveAttributeValue;


        private IRepository<Game, int> GameRepo;
        private IRepository<Team, int> TeamRepo;
        private IRepository<UserLogin, int> UserRepo;
        private readonly IRepository<ReportTemplateEntity, int> repoEntity;
        private readonly string Connectionstring;
        public ReportService(IConfiguration configuration, IRepository<ReportTemplate, int> repo, IRepository<ReportTemplateReminder, int> repoReminder, IRepository<ReportTemplateGame, int> repoGame, IRepository<ReportTemplateUser, int> repoUser, IRepository<ReportTemplateScheduler, int> repoScheduler, IRepository<ReportGiveDetails, int> repoReportGiveDetail,
            IRepository<ReportGive, int> repoReportGive, IRepository<Game, int> GameRepo, IRepository<Team, int> TeamRepo, IRepository<UserLogin, int> UserRepo,
            IRepository<ReportTemplateEntity, int> repoEntity, IRepository<ReportGiveAttributeValue, int> repoGiveAttributeValue)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoReminder = repoReminder;
            this.repoGame = repoGame;
            this.repoUser = repoUser;
            this.repoScheduler = repoScheduler;
            this.repoReportGive = repoReportGive;
            this.GameRepo = GameRepo;
            this.TeamRepo = TeamRepo;
            this.UserRepo = UserRepo;
            this.repoReportGiveDetail = repoReportGiveDetail;
            this.repoEntity = repoEntity;
            this.repoGiveAttributeValue = repoGiveAttributeValue;
        }

        public List<ReportDto> GetAllByCompanyId(int companyId, int? userId, int? gameId)
        {
            return repo.GetAllIncluding(x => x.CompanyId == companyId && (gameId.HasValue ? x.GameId == gameId : true) && (userId.HasValue ? (x.ReportTemplateUser.Any(a => a.UserId == userId) || x.CreatedBy == userId) : true), x => x.Include(m => m.ReportTemplateUser)).Select(x => new ReportDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                IsCreator = x.ReportTemplateUser.Any(a => a.UserId == userId && a.AccountAbilityId == (int)AccountAbilityType.Creator),
                IsUpdateGive = x.ReportGive.Any(a => a.AssignToType == x.ReportTemplateUser.FirstOrDefault(a => a.UserId == userId).AccountAbilityId),
                IsView = x.ReportGive.Any()
            }).ToList();
        }

        public List<ReportDto> GetAllByGameId(int companyId, int gameId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId).Select(x => new ReportDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                IsActive = x.IsActive
            }).ToList();

        }

        public async Task<ReportDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.ReportTemplateGame)
            .Include(m => m.ReportTemplateUser).Include(m => m.ReportTemplateReminder).Include(m => m.ReportTemplateEntity)
            .Include(m => m.ReportTemplateScheduler));
            ReportSchedulerDto reportScheduler = new ReportSchedulerDto();
            if (x.ReportTemplateScheduler != null)
            {
                ReportTemplateScheduler sch = x.ReportTemplateScheduler;
                reportScheduler = new ReportSchedulerDto()
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

            return new ReportDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                Scheduler = reportScheduler,
                IsActive = x.IsActive,
                FormBuilderId = x.FormBuilderId,
                EntityType = x.EntityType,
                EntityIds = x.ReportTemplateEntity.Any() ? x.ReportTemplateEntity.Select(x => x.EntityId).ToList() : new List<int>(),
                Entity = x.ReportTemplateGame.Select(s => new ReportGameDto()
                {
                    Id = s.Id,
                    GameId = s.GameId,
                    IsAdded = true
                }).ToList(),
                ReportJourney = x.ReportTemplateUser.Select(s => new ReportParticipantDto()
                {
                    Id = s.Id,
                    AccountAbilityId = s.AccountAbilityId,
                    TypeId = s.TypeId,
                    UserId = s.UserId,
                    IsAdded = true
                }).ToList(),

                Reminder = x.ReportTemplateReminder.Select(s => new ReportReminderDto()
                {
                    Id = s.Id,
                    TypeId = s.TypeId,
                    Every = s.Every,
                    ReportId = s.ReportTemplateId,
                    Unit = s.Unit
                }).ToList()

            };
        }

        public async Task<ReportGiveDto> GetGiveByReportId(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.FormBuilder).Include("FormBuilder.FormBuilderAttribute").Include("FormBuilder.FormBuilderAttribute.FormBuilderAttributeLookUp").Include(m => m.Game));

            return new ReportGiveDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                ReportId = x.Id,
                CompanyId = x.CompanyId,
                Game = x.Game.Name,
                TypeId = x.TypeId,
                FormBuilder = new FormBuilderDto()
                {
                    Name = x.FormBuilder.Name,
                    Header = x.FormBuilder.Header,
                    CompanyId = x.CompanyId,
                    Id = x.Id,
                    Footer = x.FormBuilder.Footer,
                    FormBuilderAttribute = x.FormBuilder.FormBuilderAttribute.OrderBy(o => o.OrderNo).Select(a => new FormBuilderAttributeDto()
                    {
                        AttributeName = a.AttributeName,
                        DataType = a.DataType,
                        Id = a.Id,
                        IsRequired = a.IsRequired,
                        OrderNo = a.OrderNo,
                        FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(l => new FormBuilderAttributeLookUpDto()
                        {
                            Id = l.Id,
                            OptionName = l.OptionName
                        }).ToList()
                    }).ToList()

                }
            };
        }

        public async Task<ReportGiveSaveDto> GetGiveUpdateByReportId(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.FormBuilder).Include(m => m.ReportTemplateUser).Include("ReportTemplateUser.User").Include("FormBuilder.FormBuilderAttribute").Include("FormBuilder.FormBuilderAttribute.FormBuilderAttributeLookUp").Include(m => m.Game).Include(m => m.ReportGive).Include("ReportGive.ReportGiveDetails")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue").Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.FormBuilderAttribute")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.LookUp"));

            var reportGive = x.ReportGive.LastOrDefault(a => a.AssignToType != 0);
            var model = new ReportGiveSaveDto()
            {
                Name = x.Name,
                ReportId = x.Id,
                Game = x.Game.Name,
                Id = reportGive.Id,
                Remark = reportGive.Remark,
                IsEdit = reportGive.AssignToType == (int)AccountAbilityType.Creator,
                FormBuilderAttribute = x.FormBuilder.FormBuilderAttribute.OrderBy(o => o.OrderNo).Select(a => new FormBuilderAttributeDto()
                {
                    AttributeName = a.AttributeName,
                    DataType = a.DataType,
                    Id = a.Id,
                    IsRequired = a.IsRequired,
                    OrderNo = a.OrderNo,
                    FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(l => new FormBuilderAttributeLookUpDto()
                    {
                        Id = l.Id,
                        OptionName = l.OptionName
                    }).ToList()
                }).ToList(),
                Accountability = x.ReportTemplateUser.GroupBy(x => x.AccountAbilityId).Select(x => new SelectedItemDto()
                {
                    Id = x.Key.ToString(),
                    Name = string.Join(",", x.Select(s => s.User.Fname + " " + s.User.Lname).ToList())
                }).ToList()
            };

            foreach (var d in reportGive.ReportGiveDetails)
            {
                string nameStr = string.Empty;
                if (d.EntityId.HasValue)
                {
                    if (x.EntityType == (int)EmotionsFor.Game)
                    {
                        nameStr = GameRepo.Get(d.EntityId.Value).Name;
                    }
                    else if (x.EntityType == (int)EmotionsFor.Team)
                    {
                        nameStr = TeamRepo.Get(d.EntityId.Value).Name;
                    }
                    else if (x.EntityType == (int)EmotionsFor.Player)
                    {
                        var u = UserRepo.Get(d.EntityId.Value);
                        nameStr = u.Fname + u.Lname;
                    }
                }

                model.Details.Add(new ReportGiveSaveDetailsDto()
                {
                    EntityId = d.EntityId,
                    Id = d.Id,
                    Remark = d.Remark,
                    Name = nameStr,
                    Attribute = d.ReportGiveAttributeValue.Select(a => new FormBuilderAttributeValueDto()
                    {
                        AttributeValue = a.AttributeValue,
                        FormBuilderAttributeId = a.FormBuilderAttributeId,
                        FormBuilderAttribute = a.FormBuilderAttribute.AttributeName,
                        LookUpId = a.LookUpId,
                        LookUp = a.LookUpId.HasValue ? a.LookUp.OptionName : string.Empty,
                        Id = a.Id
                    }).ToList()

                });
            }

            return model;
        }

        public async Task<List<ReportGiveSaveDto>> GetGiveUpdateByReportRange(int id, DateTime start, DateTime end)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.FormBuilder).Include(m => m.ReportTemplateUser).Include("ReportTemplateUser.User").Include("FormBuilder.FormBuilderAttribute").Include("FormBuilder.FormBuilderAttribute.FormBuilderAttributeLookUp").Include(m => m.Game).Include(m => m.ReportGive).Include("ReportGive.ReportGiveDetails")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue").Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.FormBuilderAttribute")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.LookUp"));

            var reportGives = x.ReportGive.Where(a => a.AssignToType != 0 && a.CreatedDate.Date >= start && a.CreatedDate.Date <= end).ToList();
            List<ReportGiveSaveDto> response = new List<ReportGiveSaveDto>();

            foreach (var reportGive in reportGives.Where(a => a.ReportGiveDetails.Any()))
            {
                var model = new ReportGiveSaveDto()
                {
                    Name = x.Name,
                    ReportId = x.Id,
                    Game = x.Game.Name,
                    Id = reportGive.Id,
                    Remark = reportGive.Remark,
                    GiveDate = reportGive.CreatedDate,
                    IsEdit = reportGive.AssignToType == (int)AccountAbilityType.Creator,
                    FormBuilderAttribute = x.FormBuilder.FormBuilderAttribute.OrderBy(o => o.OrderNo).Select(a => new FormBuilderAttributeDto()
                    {
                        AttributeName = a.AttributeName,
                        DataType = a.DataType,
                        Id = a.Id,
                        IsRequired = a.IsRequired,
                        OrderNo = a.OrderNo,
                        FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(l => new FormBuilderAttributeLookUpDto()
                        {
                            Id = l.Id,
                            OptionName = l.OptionName
                        }).ToList()
                    }).ToList(),
                    Accountability = x.ReportTemplateUser.GroupBy(x => x.AccountAbilityId).Select(x => new SelectedItemDto()
                    {
                        Id = x.Key.ToString(),
                        Name = string.Join(",", x.Select(s => s.User.Fname + " " + s.User.Lname).ToList())
                    }).ToList()
                };

                foreach (var d in reportGive.ReportGiveDetails)
                {
                    string nameStr = string.Empty;
                    if (d.EntityId.HasValue)
                    {
                        if (x.EntityType == (int)EmotionsFor.Game)
                        {
                            nameStr = GameRepo.Get(d.EntityId.Value).Name;
                        }
                        else if (x.EntityType == (int)EmotionsFor.Team)
                        {
                            nameStr = TeamRepo.Get(d.EntityId.Value).Name;
                        }
                        else if (x.EntityType == (int)EmotionsFor.Player)
                        {
                            var u = UserRepo.Get(d.EntityId.Value);
                            nameStr = u.Fname + u.Lname;
                        }
                    }

                    model.Details.Add(new ReportGiveSaveDetailsDto()
                    {
                        EntityId = d.EntityId,
                        Id = d.Id,
                        Remark = d.Remark,
                        Name = nameStr,
                        Attribute = d.ReportGiveAttributeValue.Select(a => new FormBuilderAttributeValueDto()
                        {
                            AttributeValue = a.AttributeValue,
                            FormBuilderAttributeId = a.FormBuilderAttributeId,
                            FormBuilderAttribute = a.FormBuilderAttribute.AttributeName,
                            LookUpId = a.LookUpId,
                            LookUp = a.LookUpId.HasValue ? a.LookUp.OptionName : string.Empty,
                            Id = a.Id
                        }).ToList()

                    });
                }

                response.Add(model);
            }


            return response;
        }

        public async Task<List<SelectedItemDto>> GetEntityByReportId(int id)
        {
            List<SelectedItemDto> response = new List<SelectedItemDto>();
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.ReportTemplateEntity));
            List<int> entityId = x.ReportTemplateEntity.Any() ? x.ReportTemplateEntity.Select(x => x.EntityId).ToList() : new List<int>();
            if (x.EntityType == (int)EmotionsFor.Game)
            {
                response = GameRepo.GetAllList(x => entityId.Contains(x.Id)).Select(x => new SelectedItemDto()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name
                }).ToList();

            }
            else if (x.EntityType == (int)EmotionsFor.Team)
            {
                response = TeamRepo.GetAllList(x => entityId.Contains(x.Id)).Select(x => new SelectedItemDto()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name
                }).ToList();
            }
            else if (x.EntityType == (int)EmotionsFor.Player)
            {
                response = UserRepo.GetAllList(x => entityId.Contains(x.Id)).Select(x => new SelectedItemDto()
                {
                    Id = x.Id.ToString(),
                    Name = x.Fname + " " + x.Lname
                }).ToList();
            }

            return response;
        }
        public async Task<bool> SaveReortGive(ReportGiveSaveDto model)
        {
            repoReportGive.Insert(new ReportGive()
            {
                CreatedBy = model.UserId,
                ReportId = model.ReportId,
                CreatedDate = DateTime.Now,
                Remark = model.Remark,
                RemarkedBy = model.UserId,
                AssignToType = (int)AccountAbilityType.Confirmed,
                ReportGiveDetails = model.Details.Select(d => new ReportGiveDetails()
                {
                    EntityId = d.EntityId,
                    Remark = d.Remark,
                    RemarkedBy = model.UserId,
                    ReportGiveAttributeValue = d.Attribute.Select(x => new ReportGiveAttributeValue()
                    {
                        AttributeValue = x.AttributeValue,
                        FormBuilderAttributeId = x.FormBuilderAttributeId,
                        LookUpId = x.LookUpId
                    }).ToList()
                }).ToList(),

            });
            return true;
        }

        public async Task<bool> UpdateReortGive(ReportGiveSaveDto model)
        {
            var give = await this.repoReportGive.GetIncludingByIdAsyn(x => x.Id == model.Id, x => x.Include(m => m.ReportGiveDetails)
            .Include("ReportGiveDetails.ReportGiveAttributeValue"));
            if (model.IsManual && model.IsEdit)
            {
                var existing = give.ReportGiveDetails.Where(x => model.Details.Any(scdet => scdet.Id == x.Id)).ToList();
                var delete = give.ReportGiveDetails.Where(x => !model.Details.Any(scdet => scdet.Id == x.Id)).ToList();
                var insert = model.Details.Where(x => !give.ReportGiveDetails.Any(m => m.Id == x.Id)).ToList();

                if (delete.Any())
                {
                    foreach (var item in delete)
                    {
                        this.repoGiveAttributeValue.DeleteRange(item.ReportGiveAttributeValue.ToList());
                    }

                    this.repoReportGiveDetail.DeleteRange(delete);
                }

                if (existing.Any())
                {
                    foreach (var e in existing)
                    {
                        var record = model.Details.FirstOrDefault(a => a.Id == e.Id);
                        if (!model.IsDuplicate)
                        {

                            e.Remark = record.Remark;
                            e.RemarkedBy = model.UserId;
                            repoReportGiveDetail.Update(e);
                            foreach (var a in record.Attribute)
                            {
                                var attr = e.ReportGiveAttributeValue.FirstOrDefault(z => z.Id == a.Id);
                                attr.LookUpId = a.LookUpId;
                                attr.AttributeValue = a.AttributeValue;
                                attr.FormBuilderAttributeId = a.FormBuilderAttributeId;
                                repoGiveAttributeValue.Update(attr);
                            }
                        }
                        else
                        {
                            ReportGiveDetails addrecords = new ReportGiveDetails();

                            addrecords.EntityId = e.EntityId;
                            addrecords.Remark = e.Remark;
                            addrecords.RemarkedBy = model.UserId;
                            addrecords.ReportGiveId = model.Id.Value;
                            repoReportGiveDetail.Insert(addrecords);
                            foreach (var a in record.Attribute)
                            {
                                ReportGiveAttributeValue onjAttr = new ReportGiveAttributeValue();
                                onjAttr.LookUpId = a.LookUpId;
                                onjAttr.AttributeValue = a.AttributeValue;
                                onjAttr.FormBuilderAttributeId = a.FormBuilderAttributeId;
                                onjAttr.ReportGiveDetailId = addrecords.Id;
                                repoGiveAttributeValue.Insert(onjAttr);
                            }
                        }
                    }
                }
                if (insert.Any())
                {
                    List<ReportGiveDetails> addrecords = insert.Select(d => new ReportGiveDetails()
                    {
                        EntityId = d.EntityId,
                        Remark = d.Remark,
                        RemarkedBy = model.UserId,
                        ReportGiveId = model.Id.Value,
                        ReportGiveAttributeValue = d.Attribute.Select(x => new ReportGiveAttributeValue()
                        {
                            AttributeValue = x.AttributeValue,
                            FormBuilderAttributeId = x.FormBuilderAttributeId,
                            LookUpId = x.LookUpId
                        }).ToList()
                    }).ToList();

                    await repoReportGiveDetail.InsertRangeAsyn(addrecords);
                }
            }
            else
            {
                foreach (var d in model.Details)
                {
                    var item = give.ReportGiveDetails.FirstOrDefault(a => a.Id == d.Id);
                    item.Remark = d.Remark;
                    item.RemarkedBy = model.UserId;
                    repoReportGiveDetail.Update(item);
                    if (model.IsEdit)
                    {
                        foreach (var a in d.Attribute)
                        {
                            var attr = item.ReportGiveAttributeValue.FirstOrDefault(z => z.Id == a.Id);
                            attr.LookUpId = a.LookUpId;
                            attr.AttributeValue = a.AttributeValue;
                            attr.FormBuilderAttributeId = a.FormBuilderAttributeId;
                            repoGiveAttributeValue.Update(attr);
                        }
                    }
                }
            }


            give.Remark = model.Remark;
            give.CreatedBy = model.UserId;
            give.RemarkedBy = model.UserId;

            if (model.IsReject)
            {

                if (give.AssignToType == (int)AccountAbilityType.Confirmed)
                    give.AssignToType = (int)AccountAbilityType.Creator;
                else if (give.AssignToType == (int)AccountAbilityType.Verifier)
                    give.AssignToType = (int)AccountAbilityType.Confirmed;
                else if (give.AssignToType == (int)AccountAbilityType.Approved)
                    give.AssignToType = (int)AccountAbilityType.Verifier;
            }
            else
            {
                if (give.AssignToType == (int)AccountAbilityType.Creator)
                    give.AssignToType = (int)AccountAbilityType.Confirmed;
                else if (give.AssignToType == (int)AccountAbilityType.Confirmed)
                    give.AssignToType = (int)AccountAbilityType.Verifier;
                else if (give.AssignToType == (int)AccountAbilityType.Verifier)
                    give.AssignToType = (int)AccountAbilityType.Approved;
                else if (give.AssignToType == (int)AccountAbilityType.Approved)
                    give.AssignToType = 0;
            }
            repoReportGive.Update(give);

            return true;
        }

        public async Task<ReportGiveResultDto> GetGiveListByReportId(int id)
        {
            var report = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x
            .Include(m => m.FormBuilder)
            .Include(m => m.Game)
            .Include(m => m.ReportGive)
            .Include("ReportGive.ReportGiveDetails")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.FormBuilderAttribute")
            .Include("ReportGive.ReportGiveDetails.ReportGiveAttributeValue.LookUp")
            );

            var response = new ReportGiveResultDto()
            {
                Name = report.Name,
                Desciption = report.Desciption,
                Game = report.Game.Name
            };
            var details = report.ReportGive.LastOrDefault().ReportGiveDetails;
            foreach (var s in details)
            {
                string nameStr = string.Empty;
                if (report.EntityType == (int)EmotionsFor.Game)
                {
                    nameStr = GameRepo.Get(s.EntityId.Value).Name;
                }
                else if (report.EntityType == (int)EmotionsFor.Team)
                {
                    nameStr = TeamRepo.Get(s.EntityId.Value).Name;
                }
                else if (report.EntityType == (int)EmotionsFor.Player)
                {
                    var u = UserRepo.Get(s.EntityId.Value);
                    nameStr = u.Fname + u.Lname;
                }

                response.Result.Add(new ReportGiveDetailsDto()
                {
                    Attribute = s.ReportGiveAttributeValue.OrderBy(o => o.FormBuilderAttribute.OrderNo).Select(a => new ReportGiveAttributeDto()
                    {
                        Attribute = a.FormBuilderAttribute.AttributeName,
                        AttributeValue = a.LookUpId.HasValue ? a.LookUp.OptionName : a.AttributeValue
                    }).ToList(),
                    UserName = nameStr
                });
            }

            return response;
        }

        public async Task<ReportDto> GetDetailsId(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new ReportDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                IsActive = x.IsActive
            };
        }
        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(ReportDto dto)
        {
            if (dto.Id.HasValue)
            {
                var report = this.repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.ReportTemplateScheduler).Include(m => m.ReportTemplateEntity)
            .Include(m => m.ReportTemplateGame).Include(m => m.ReportTemplateUser).Include(m => m.ReportTemplateReminder));

                if (report.ReportTemplateGame != null)
                {
                    var existingGame = report.ReportTemplateGame.Where(x => dto.Entity.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = report.ReportTemplateGame.Where(x => !dto.Entity.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.Entity.Where(x => !report.ReportTemplateGame.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {
                        this.repoGame.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {
                            var record = dto.Entity.FirstOrDefault(a => a.Id == e.Id);
                            e.GameId = record.GameId;
                            repoGame.Update(e);
                        }
                    }
                    if (insertedGame.Any())
                    {
                        List<ReportTemplateGame> addrecords = insertedGame.Select(a => new ReportTemplateGame()
                        {
                            ReportTemplateId = dto.Id.Value,
                            GameId = a.GameId,
                        }).ToList();

                        await repoGame.InsertRangeAsyn(addrecords);
                    }
                }

                if (report.ReportTemplateUser != null)
                {
                    var existingUser = report.ReportTemplateUser.Where(x => dto.ReportJourney.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedUser = report.ReportTemplateUser.Where(x => !dto.ReportJourney.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedUser = dto.ReportJourney.Where(x => !report.ReportTemplateUser.Any(m => m.Id == x.Id)).ToList();


                    if (deletedUser.Any())
                    {
                        this.repoUser.DeleteRange(deletedUser);
                    }

                    if (existingUser.Any())
                    {
                        foreach (var e in existingUser)
                        {
                            var record = dto.ReportJourney.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId.Value;
                            e.TypeId = record.TypeId;
                            e.AccountAbilityId = record.AccountAbilityId.Value;
                            repoUser.Update(e);
                        }
                    }
                    if (insertedUser.Any())
                    {
                        List<ReportTemplateUser> addrecords = insertedUser.Select(a => new ReportTemplateUser()
                        {
                            ReportTemplateId = dto.Id.Value,
                            UserId = a.UserId.Value,
                            TypeId = a.TypeId,
                            AccountAbilityId = a.AccountAbilityId.Value
                        }).ToList();

                        await repoUser.InsertRangeAsyn(addrecords);
                    }
                }

                if (report.ReportTemplateReminder != null)
                {
                    var existingReminder = report.ReportTemplateReminder.Where(x => dto.Reminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedReminder = report.ReportTemplateReminder.Where(x => !dto.Reminder.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedReminder = dto.Reminder.Where(x => !report.ReportTemplateReminder.Any(m => m.Id == x.Id)).ToList();


                    if (deletedReminder.Any())
                    {
                        this.repoReminder.DeleteRange(deletedReminder);
                    }

                    if (existingReminder.Any())
                    {
                        foreach (var e in existingReminder)
                        {
                            var record = dto.Reminder.FirstOrDefault(a => a.Id == e.Id);
                            e.Unit = record.Unit.Value;
                            e.Every = record.Every.Value;
                            e.TypeId = record.TypeId.Value;
                            repoReminder.Update(e);
                        }
                    }
                    if (insertedReminder.Any())
                    {
                        List<ReportTemplateReminder> addrecords = insertedReminder.Select(a => new ReportTemplateReminder()
                        {
                            ReportTemplateId = dto.Id.Value,
                            Every = a.Every.Value,
                            TypeId = a.TypeId.Value,
                            Unit = a.Unit.Value
                        }).ToList();

                        await repoReminder.InsertRangeAsyn(addrecords);
                    }
                }



                if (report.ReportTemplateScheduler != null)
                {
                    repoScheduler.Delete(report.ReportTemplateScheduler);
                }

                if (dto.Scheduler != null)
                {
                    repoScheduler.Insert(new ReportTemplateScheduler()
                    {
                        DailyFrequency = dto.Scheduler.DailyFrequency,
                        DaysOfWeek = dto.Scheduler.DaysOfWeek,
                        EndDate = dto.Scheduler.EndDate,
                        ExactDateOfMonth = dto.Scheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.Scheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.Scheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.Scheduler.Frequency,
                        MonthlyOccurrence = dto.Scheduler.MonthlyOccurrence,
                        OccursEveryTimeUnit = dto.Scheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.Scheduler.OccursEveryValue,
                        RecurseEvery = dto.Scheduler.RecurseEvery,
                        StartDate = dto.Scheduler.StartDate.Value,
                        TimeEnd = dto.Scheduler.TimeEnd,
                        TimeStart = dto.Scheduler.TimeStart.Value,
                        Type = dto.Scheduler.Type,
                        ValidDays = dto.Scheduler.ValidDays,
                        ReportTemplateId = dto.Id.Value,
                        Venue = "test"
                    });
                }

                if (report.ReportTemplateEntity.Any())
                {
                    repoEntity.DeleteRange(report.ReportTemplateEntity.ToList());
                }

                if (dto.EntityIds.Any())
                {
                    repoEntity.InsertRange(dto.EntityIds.Select(e => new ReportTemplateEntity()
                    {
                        EntityId = e,
                        ReportId = dto.Id.Value
                    }).ToList()
                    );

                }
                report.EntityType = dto.EntityType;
                report.Name = dto.Name;
                report.Desciption = dto.Desciption;
                report.GameId = dto.GameId;
                report.TypeId = dto.TypeId;
                report.FormBuilderId = dto.FormBuilderId;
                report.IsActive = dto.IsActive;
                report.ModifiedDate = DateTime.Now;
                repo.Update(report);
                return dto.Id.Value;
            }
            else
            {
                var reportInfo = new ReportTemplate()
                {
                    CompanyId = dto.CompanyId.Value,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    FormBuilderId = dto.FormBuilderId,
                    TypeId = dto.TypeId,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    EntityType = dto.EntityType,
                    ReportTemplateEntity = dto.EntityIds.Any() ? dto.EntityIds.Select(e => new ReportTemplateEntity()
                    {
                        EntityId = e
                    }).ToList() : new List<ReportTemplateEntity>(),
                    ReportTemplateGame = dto.Entity.Select(s => new ReportTemplateGame()
                    {
                        GameId = s.GameId
                    }).ToList(),
                    ReportTemplateUser = dto.ReportJourney.Select(s => new ReportTemplateUser()
                    {
                        UserId = s.UserId.Value,
                        TypeId = s.TypeId,
                        AccountAbilityId = s.AccountAbilityId.Value
                    }).ToList(),
                    ReportTemplateReminder = dto.Reminder.Select(s => new ReportTemplateReminder()
                    {
                        Every = s.Every.Value,
                        TypeId = s.TypeId.Value,
                        Unit = s.Unit.Value

                    }).ToList(),

                };
                if (dto.Scheduler != null)
                {
                    reportInfo.ReportTemplateScheduler = new ReportTemplateScheduler()
                    {
                        DailyFrequency = dto.Scheduler.DailyFrequency,
                        DaysOfWeek = dto.Scheduler.DaysOfWeek,
                        EndDate = dto.Scheduler.EndDate,
                        ExactDateOfMonth = dto.Scheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.Scheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.Scheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.Scheduler.Frequency,
                        MonthlyOccurrence = dto.Scheduler.MonthlyOccurrence,
                        OccursEveryTimeUnit = dto.Scheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.Scheduler.OccursEveryValue,
                        RecurseEvery = dto.Scheduler.RecurseEvery,
                        StartDate = dto.Scheduler.StartDate.Value,
                        TimeEnd = dto.Scheduler.TimeEnd,
                        TimeStart = dto.Scheduler.TimeStart.Value,
                        Type = dto.Scheduler.Type,
                        ValidDays = dto.Scheduler.ValidDays,
                        Venue = "test"
                    };
                }

                await repo.InsertAsync(reportInfo);
                return reportInfo.Id;
            }

        }

        public void DeleteBy(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.ReportTemplateGame)
           .Include(m => m.ReportTemplateUser).Include(m => m.ReportTemplateReminder).Include(m => m.ReportTemplateEntity)
           .Include(m => m.ReportTemplateScheduler));

                repoEntity.DeleteRange(x.ReportTemplateEntity.ToList());
                repoGame.DeleteRange(x.ReportTemplateGame.ToList());
                repoUser.DeleteRange(x.ReportTemplateUser.ToList());
                repoReminder.DeleteRange(x.ReportTemplateReminder.ToList());
                repoScheduler.Delete(x.ReportTemplateScheduler);
                repo.Delete(x);

                context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }
        }

        public List<StatusFeedbackReminderDto> GetReportTemplateReminderMailNotification(DateTime currentDateTime, int reminterType)
        {
            DataTable dt = GetReportTemplateReminderMail(currentDateTime, reminterType);
            List<int> reportTemplateId = new List<int>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    reportTemplateId.Add(int.Parse(row[0].ToString()));
                }

                return repoUser.GetAllIncluding(x => reportTemplateId.Contains(x.ReportTemplateId), x => x.Include(m => m.ReportTemplate).Include(x => x.User)).Select(x => new StatusFeedbackReminderDto()
                {
                    Id = x.ReportTemplateId,
                    UserId = x.UserId,
                    Email = x.User.Email,
                    Name = x.ReportTemplate.Name
                }).ToList();
            }
            else
            {
                return new List<StatusFeedbackReminderDto>();
            }
        }

        public List<SessionScheduleEventDto> GetCompanyReportEvent(int companyId, int? userId, int? gameId, DateTime from, DateTime to)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanyReportEvent]",
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
        private DataTable GetReportTemplateReminderMail(DateTime currentDateTime, int reminterType)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetReportTemplateReminderMail]",
                          new SqlParameter("@currentDateTime", currentDateTime),
                          new SqlParameter("@reminterType", reminterType));

            DataTable dt = null;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {


            IQueryable<ReportTemplate> query = repo.GetAllIncluding(x => x.CompanyId == parameters.CompanyId && x.GameId == parameters.GameId && (parameters.UserType == (int)UserType.User ? (x.ReportTemplateUser.Any(a => a.UserId == parameters.UserId) || x.CreatedBy == parameters.UserId) : true), x => x.Include(m => m.ReportTemplateUser).Include(m => m.ReportGive));

            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<ReportGridDto> data = new List<ReportGridDto>();
            foreach (ReportTemplate x in result.Data)
            {
                data.Add(new ReportGridDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    TypeId = ((ReportType)x.TypeId).ToString(),
                    IsActive = x.IsActive ? "Active" : "In active",
                    CreatedBy = x.CreatedBy,
                    IsCreator = x.ReportTemplateUser.Any(a => a.UserId == parameters.UserId && a.AccountAbilityId == (int)AccountAbilityType.Creator),
                    IsUpdateGive = x.ReportTemplateUser.Any(a => a.UserId == parameters.UserId) && x.ReportGive.Any(a => a.AssignToType == x.ReportTemplateUser.FirstOrDefault(a => a.UserId == parameters.UserId).AccountAbilityId),
                    IsView = x.ReportGive.Any()
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}

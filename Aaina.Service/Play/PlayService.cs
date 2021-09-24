using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Aaina.Common;
using Newtonsoft.Json.Schema;
using Aaina.Data;

namespace Aaina.Service
{
    public class PlayService : IPlayService
    {
        private IRepository<Play, int> repo;
        private IRepository<PlayDelegate, int> repoDelegate;
        private IRepository<PlayPersonInvolved, int> repoInvolved;
        private IRepository<PlayFeedback, int> feedbackrepo;
        private IRepository<UserLogin, int> UserLogin;
        private IRepository<Game, int> repoGame;
        private IRepository<GamePlayer, int> repoGameplayer;
        public PlayService(IRepository<Play, int> repo, IRepository<PlayDelegate, int> repoDelegate, IRepository<PlayPersonInvolved, int> repoInvolved, IRepository<PlayFeedback, int> feedbackrepo, IRepository<UserLogin, int> UserLogin, IRepository<Game, int> Game, IRepository<GamePlayer, int> GamePlayers)
        {
            this.repo = repo;
            this.repoDelegate = repoDelegate;
            this.repoInvolved = repoInvolved;
            this.feedbackrepo = feedbackrepo;
            this.UserLogin = UserLogin;
            this.repoGame = Game;
            this.repoGameplayer = GamePlayers;
        }

        public PlayDto GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.PlayPersonInvolved));
            return new PlayDto()
            {
                Name = x.Name,
                ParentId = x.ParentId,
                Id = x.Id,
                AccountableId = x.AccountableId,
                ActualEndDate = x.ActualEndDate,
                ActualStartDate = x.ActualStartDate,
                AddedOn = x.AddedOn,
                Comments = x.Comments,
                DeadlineDate = x.DeadlineDate,
                DependancyId = x.DependancyId,
                Description = x.Description,
                Emotion = !string.IsNullOrEmpty(x.Emotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Emotion)) : (int?)null,
                FeedbackType = x.FeedbackType,
                GameId = x.GameId,
                IsToday = x.IsToday,
                Phoemotion = !string.IsNullOrEmpty(x.Phoemotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Phoemotion)) : (int?)null,
                Priority = x.Priority,
                StartDate = x.StartDate,
                Status = x.Status,
                SubGameId = x.SubGameId,
                Type = x.Type,
                PersonInvolved = x.PlayPersonInvolved.Select(x => x.UserId).ToList(),
                IsRequested = x.IsRequested,
                GameType = x.GameType
            };
        }

        public void UpdateStatus(PlayDelegateDto dto)
        {
            var x = this.repo.Get(dto.PlayId);
            x.Status = dto.StatusId;
            x.Comments = dto.Description;
            x.ModifiedDate = DateTime.Now;

            if (dto.StatusId == (int)StatusType.Active)
            {
                x.ActualStartDate = DateTime.Now;
            }

            if (dto.StatusId == (int)StatusType.Completed)
            {
                x.ActualEndDate = DateTime.Now;
            }

            if (dto.StatusId == (int)StatusType.Delegate)
            {
                x.Status = (int)StatusType.Inactive;
                x.AccountableId = dto.DelegateId;
            }
            repo.Update(x);
            if (dto.StatusId == (int)StatusType.Delegate)
            {
                repoDelegate.Insert(new PlayDelegate()
                {
                    AccountableId = dto.AccountableId,
                    DelegateId = dto.DelegateId,
                    DelegateDate = DateTime.Now,
                    Description = dto.Description,
                    PlayId = dto.PlayId
                });
            }
        }

        public async Task AddFeedBack(PlayFeedbackDto dto)
        {

            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var filterInfo = new PlayFeedback()
                {
                    Comment = dto.Description,
                    Complition = dto.Percentage,
                    IsDeleted = false,
                    Emations = dto.emoji,
                    Created = DateTime.Now,
                    PlayId = dto.PlayId,
                    Status = dto.FeedbackStatus,
                    Priority = dto.FeedbackPriority,
                    CretedBy = dto.CreatedBy
                };
                await feedbackrepo.InsertAsync(filterInfo);

                var x = await this.repo.GetAsync(dto.PlayId.Value);
                x.Status = dto.FeedbackStatus.Value;
                x.Priority = dto.FeedbackPriority.Value;
                await this.repo.UpdateAsync(x);

                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }




        }

        public void MoveToday(List<int> ids)
        {
            var allRecord = this.repo.GetAllList(x => ids.Contains(x.Id));
            allRecord.ForEach(x =>
            {
                x.IsToday = true;
                x.ModifiedDate = DateTime.Now;
            });
            repo.UpdateRange(allRecord);

        }

        public async Task<int> AddUpdateAsync(PlayDto dto)
        {

            //int Role = this.repoGameplayer.GetAllIncluding(x => x.GameId == dto.GameId.Value && x.UserId == dto.UserId).FirstOrDefault()?.RoleId ?? 0;
            int Role = this.repoGameplayer.GetIncludingById(x => x.GameId == dto.GameId.Value && x.UserId == dto.UserId)?.RoleId ?? 0;

            if (dto.Id.HasValue)
            {
                var filter = this.repo.GetIncludingById(x => x.Id == dto.Id.Value, x => x.Include(m => m.PlayPersonInvolved));

                if (filter.PlayPersonInvolved != null && filter.PlayPersonInvolved.Any())
                {
                    repoInvolved.DeleteRange(filter.PlayPersonInvolved.ToList());
                }

                filter.ParentId = dto.ParentId;
                filter.Name = dto.Name;
                filter.GameId = dto.GameId.Value;
                filter.SubGameId = dto.SubGameId;
                filter.Description = dto.Description;
                filter.AddedOn = dto.AddedOn.Value;
                filter.StartDate = dto.StartDate.Value;
                filter.DeadlineDate = dto.DeadlineDate.Value;
                filter.AccountableId = dto.AccountableId.Value;
                filter.DependancyId = dto.DependancyId;
                filter.Emotion = dto.Emotion.HasValue ? EncryptDecrypt.Encrypt(dto.Emotion.Value.ToString()) : null;
                filter.FeedbackType = dto.FeedbackType;
                filter.IsActive = dto.IsToday;
                filter.Priority = dto.Priority.Value;
                filter.Status = dto.Status.Value;
                filter.Type = dto.Type;
                filter.ModifiedDate = DateTime.Now;
                filter.PlayPersonInvolved = dto.PersonInvolved.Select(x => new PlayPersonInvolved()
                {
                    UserId = x,
                    PlayId = dto.Id.Value

                }).ToList();
                filter.GameType = dto.GameType;

                filter.IsRequested = (dto.GameType == 1 ? false : (Role == 1 ? true : false));
                repo.Update(filter);
                return dto.Id.Value;
            }
            else
            {
                Game objgame = new Game();

                //int Role = this.repoGame.GetAsync(dto.GameId.Value).Result.GamePlayer?.Where(x => x.UserId == x.UserId).FirstOrDefault().RoleId ?? 0;
                var filterInfo = new Play()
                {
                    CompanyId = dto.CompanyId,
                    ParentId = dto.ParentId,
                    Name = dto.Name,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    DeadlineDate = dto.DeadlineDate.Value,
                    GameId = dto.GameId.Value,
                    SubGameId = dto.SubGameId,
                    AddedOn = dto.AddedOn.Value,
                    StartDate = dto.StartDate.Value,
                    AccountableId = dto.AccountableId.Value,
                    DependancyId = dto.DependancyId,
                    Priority = dto.Priority.Value,
                    Status = dto.Status.Value,
                    Emotion = dto.Emotion.HasValue ? EncryptDecrypt.Encrypt(dto.Emotion.Value.ToString()) : null,
                    IsActive = true,
                    Description = dto.Description,
                    FeedbackType = dto.FeedbackType,
                    Type = dto.Type,
                    GameType = dto.GameType,
                    IsRequested = (dto.GameType == 1 ? false : (Role == 1 ? true : false)),
                    PlayPersonInvolved = dto.PersonInvolved.Select(x => new PlayPersonInvolved()
                    {
                        UserId = x
                    }).ToList()
                };
                await repo.InsertAsync(filterInfo);
                return filterInfo.Id;
            }

        }

        public PreSessionAgendaDto GetPlayAction(int id)
        {
            var s = repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.Game).Include(m => m.Dependancy));
            return new PreSessionAgendaDto()
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
            };
        }
        public List<SelectedItemDto> GetAllParentDrop(int companyId, int typeId, int? gId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && (gId.HasValue ? x.GameId == gId : true) && x.Type == typeId && !x.ParentId.HasValue).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

        }
        public List<PlayFeedbackDto> GetAllFeedBack(int playid)
        {
            List<PlayFeedbackDto> playfeeddtoList = new List<PlayFeedbackDto>();

            List<PlayFeedback> playfeed = new List<PlayFeedback>();
            playfeed = feedbackrepo.GetAllList(x => x.PlayId == playid).OrderByDescending(x => x.PlayFeedbackId).ToList();

            foreach (var x in playfeed)
            {
                PlayFeedbackDto playfeeddto = new PlayFeedbackDto();
                playfeeddto.Description = x.Comment;
                playfeeddto.emoji = x.Emations;
                playfeeddto.FeedbackPriority = x.Priority;
                playfeeddto.FeedbackStatus = x.Status;
                playfeeddto.Percentage = x.Complition;
                playfeeddto.PlayId = x.PlayId;
                playfeeddto.Date = x.Created.ToString();
                playfeeddto.UserName = (x.Created.HasValue ? this.UserLogin.Get(x.CretedBy.Value).UserName : "");
                playfeeddtoList.Add(playfeeddto);
            }
            return playfeeddtoList;
        }

        public List<PlayDto> GetAll(int companyId, int typeId, int? gId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && (gId.HasValue ? x.GameId == gId : true) && x.Type == typeId).Select(x => new PlayDto()
            {
                Name = x.Name,
                Id = x.Id,
                AccountableId = x.AccountableId,
                ActualEndDate = x.ActualEndDate,
                ActualStartDate = x.ActualStartDate,
                AddedOn = x.AddedOn,
                Comments = x.Comments,
                DeadlineDate = x.DeadlineDate,
                DependancyId = x.DependancyId,
                Description = x.Description,
                Emotion = !string.IsNullOrEmpty(x.Emotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Emotion)) : (double?)null,
                FeedbackType = x.FeedbackType,
                GameId = x.GameId,
                IsToday = x.IsToday,
                Phoemotion = !string.IsNullOrEmpty(x.Phoemotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Phoemotion)) : (double?)null,
                Priority = x.Priority,
                StartDate = x.StartDate,
                Status = x.Status,
                SubGameId = x.SubGameId,
                Type = x.Type
            }).ToList();

        }

        public List<PlayDto> GetAll(int companyId, int typeId, bool istoday, int? userId, int? gId)
        {
            var parentList = repo.GetAllIncluding(x => x.CompanyId == companyId && (gId.HasValue ? x.GameId == gId : true) && (!istoday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == istoday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame)
                              .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User")).Select(x => new PlayDto()
                              {
                                  Name = x.Name,
                                  ParentId = x.ParentId,
                                  Id = x.Id,
                                  AccountableId = x.AccountableId,
                                  Accountable = x.Accountable.Fname + " " + x.Accountable.Lname,
                                  Dependancy = x.Dependancy != null ? x.Dependancy.Fname + " " + x.Dependancy.Lname : null,
                                  ActualEndDate = x.ActualEndDate,
                                  ActualStartDate = x.ActualStartDate,
                                  AddedOn = x.AddedOn,
                                  Comments = x.Comments,
                                  DeadlineDate = x.DeadlineDate,
                                  DependancyId = x.DependancyId,
                                  Description = x.Description,
                                  Emotion = !string.IsNullOrEmpty(x.Emotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Emotion)) : (int?)null,
                                  FeedbackType = x.FeedbackType,
                                  GameId = x.GameId,
                                  Game = x.Game.Name,
                                  SubGame = x.SubGame != null ? x.SubGame.Name : string.Empty,
                                  IsToday = x.IsToday,
                                  Phoemotion = !string.IsNullOrEmpty(x.Phoemotion) ? int.Parse(EncryptDecrypt.Decrypt(x.Phoemotion)) : (int?)null,
                                  Priority = x.Priority,
                                  StartDate = x.StartDate,
                                  Status = x.Status,
                                  SubGameId = x.SubGameId,
                                  Type = x.Type,
                                  PersonInvolvedStr = x.PlayPersonInvolved.Any() ? string.Join(",", x.PlayPersonInvolved.Select(x => x.User.Fname + " " + x.User.Lname).ToList()) : string.Empty
                              }).ToList();


            var palyList = parentList.Where(a => !a.ParentId.HasValue || (a.ParentId.HasValue && !parentList.Any(q => q.ParentId == a.Id))).ToList();
            palyList.ForEach(a =>
            {
                a.ChildList = parentList.Where(w => w.ParentId == a.Id).ToList();
            });

            return palyList;

        }

        public async Task<bool> IsExist(int companyId, string name, int? id, int typeId)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Type == typeId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task Delete(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var filter = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.PlayPersonInvolved).Include(m => m.PlayDelegate));
                if (filter.PlayPersonInvolved != null && filter.PlayPersonInvolved.Any())
                {
                    repoInvolved.DeleteRange(filter.PlayPersonInvolved.ToList());
                }
                if (filter.PlayDelegate != null && filter.PlayDelegate.Any())
                {
                    repoDelegate.DeleteRange(filter.PlayDelegate.ToList());
                }
                await this.repo.DeleteAsync(filter);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }

        }

        public async Task Approve(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var filter = this.repo.GetIncludingById(x => x.Id == id);
                filter.IsRequested = true;
                filter.ModifiedDate = DateTime.Now;
                await this.repo.UpdateAsync(filter);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }

        }
        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {
            bool isToday = true;
            int typeId = 0;
            int? userId = null;

            string[] prmsarry = parameters.Parm1.Split(',');

            switch (prmsarry[0])
            {
                case "today":
                    typeId = (int)PlayType.Play;
                    isToday = true;
                    break;
                case "action":
                    typeId = (int)PlayType.Play;
                    isToday = false;
                    break;
                case "feedback":
                    typeId = (int)PlayType.Feedback;
                    isToday = false;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(parameters.Parm2))
            {
                userId = int.Parse(parameters.Parm2);
            }

            //int Role = this.repoGameplayer.GetAllIncluding(x => x.GameId == parameters.GameId.Value && x.UserId == parameters.UserId).FirstOrDefault()?.RoleId ?? 0;
            int Role = this.repoGameplayer.GetIncludingById(x => x.GameId == parameters.GameId.Value && x.UserId == parameters.UserId)?.RoleId ?? 0;
            IQueryable<Play> query = null;
            if (prmsarry[1].ToUpper() == "ALL")
            {

                query = repo.GetAllIncluding(x => x.CompanyId == parameters.CompanyId && (parameters.GameId.HasValue ? x.GameId == parameters.GameId : true) && (!isToday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == isToday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame).Include(m => m.Parent)
                                   .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User"));
            }
            else if (prmsarry[1].ToUpper() == "INDIVIDUAL")
            {
                query = repo.GetAllIncluding(x => x.GameType == (int)GameType.Individual && x.CompanyId == parameters.CompanyId && (parameters.GameId.HasValue ? x.GameId == parameters.GameId : true) && (!isToday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == isToday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame).Include(m => m.Parent)
                                    .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User"));
            }
            else if (prmsarry[1].ToUpper() == "GAMELEVEL")
            {
                query = repo.GetAllIncluding(x => x.GameType == (int)GameType.GameLevel && x.CompanyId == parameters.CompanyId && (parameters.GameId.HasValue ? x.GameId == parameters.GameId : true) && (!isToday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == isToday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame).Include(m => m.Parent)
                                    .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User"));
            }
            else if (prmsarry[1].ToUpper() == "STATUS")
            {
                query = repo.GetAllIncluding(x => x.CompanyId == parameters.CompanyId && (parameters.GameId.HasValue ? x.GameId == parameters.GameId : true) && (!isToday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == isToday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame).Include(m => m.Parent)
                                    .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User"));
            }
            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<PlayGridDto> data = new List<PlayGridDto>();
            foreach (Play x in result.Data)
            {
                data.Add(new PlayGridDto()
                {

                    Name = x.Name,
                    ParentId = x.ParentId.HasValue ? x.Parent.Name : string.Empty,
                    Id = x.Id,
                    AccountableId = x.Accountable.Fname + " " + x.Accountable.Lname,
                    Accountable = x.AccountableId,
                    DependancyId = x.DependancyId.HasValue ? x.Dependancy.Fname + " " + x.Dependancy.Lname : "",
                    ActualEndDate = x.ActualEndDate.HasValue ? x.ActualEndDate.Value.ToString("dd/MM/yyyy") : "",
                    ActualStartDate = x.ActualStartDate.HasValue ? x.ActualStartDate.Value.ToString("dd/MM/yyyy") : "",
                    DeadlineDate = x.DeadlineDate.ToString("dd/MM/yyyy"),
                    Description = x.Description,
                    CompanyId = x.CompanyId,
                    Emotion = !string.IsNullOrEmpty(x.Emotion) ? EncryptDecrypt.Decrypt(x.Emotion) : "",
                    GameId = x.Game.Name,
                    SubGameId = x.SubGame != null ? x.SubGame.Name : string.Empty,
                    Phoemotion = !string.IsNullOrEmpty(x.Phoemotion) ? EncryptDecrypt.Decrypt(x.Phoemotion) : "",
                    Priority = ((PriorityType)x.Priority).ToString(),
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    Status = ((StatusType)x.Status).ToString(),
                    IsRequested = x.IsRequested,
                    Role = Role,
                    GameType = x.GameType,
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }

        public List<PlayGridDto> GetPlayList(bool isToday, int typeId, int? GameId, int CompanyId, int? userId)
        {

            List<PlayGridDto> data = new List<PlayGridDto>();


            IQueryable<Play> query = repo.GetAllIncluding(x => x.CompanyId == CompanyId && (GameId.HasValue ? x.GameId == GameId : true) && (!isToday ? x.Type == typeId : true) && (userId.HasValue ? x.AccountableId == userId || x.PlayPersonInvolved.Any(a => a.UserId == userId) : true) && x.IsToday == isToday && x.Status != (int)StatusType.Completed, x => x.Include(m => m.Game).Include(m => m.SubGame).Include(m => m.Parent)
                             .Include(m => m.Accountable).Include(m => m.Dependancy).Include(m => m.PlayPersonInvolved).Include("PlayPersonInvolved.User"));


            foreach (Play x in query.ToList())
            {
                data.Add(new PlayGridDto()
                {

                    Name = x.Name,
                    ParentId = x.ParentId.HasValue ? x.Parent.Name : string.Empty,
                    Id = x.Id,
                    AccountableId = x.Accountable.Fname + " " + x.Accountable.Lname,
                    Accountable = x.AccountableId,
                    DependancyId = x.DependancyId.HasValue ? x.Dependancy.Fname + " " + x.Dependancy.Lname : "",
                    ActualEndDate = x.ActualEndDate.HasValue ? x.ActualEndDate.Value.ToString("dd/MM/yyyy") : "",
                    ActualStartDate = x.ActualStartDate.HasValue ? x.ActualStartDate.Value.ToString("dd/MM/yyyy") : "",
                    DeadlineDate = x.DeadlineDate.ToString("dd/MM/yyyy"),
                    Description = x.Description,
                    CompanyId = x.CompanyId,
                    Emotion = !string.IsNullOrEmpty(x.Emotion) ? EncryptDecrypt.Decrypt(x.Emotion) : "",
                    GameId = x.Game.Name,
                    SubGameId = x.SubGame != null ? x.SubGame.Name : string.Empty,
                    Phoemotion = !string.IsNullOrEmpty(x.Phoemotion) ? EncryptDecrypt.Decrypt(x.Phoemotion) : "",
                    Priority = ((PriorityType)x.Priority).ToString(),
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    Status = ((StatusType)x.Status).ToString()
                });

            }
            return data;
        }

    }
}

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

namespace Aaina.Service
{
    public class GameFeedbackService : IGameFeedbackService
    {
        private IRepository<GameFeedback, int> repo;
        private IRepository<Look, int> repoLook;
        private IRepository<GameFeedbackDetails, int> repoDetail;

        private IRepository<TeamFeedback, int> repoTeamFeedback;
        private IRepository<TeamFeedbackDetails, int> repoTeamFeedbackDetails;

        private IRepository<UserFeedback, int> repoUserFeedback;
        private IRepository<UserFeedbackDetails, int> repoDetailUserFeedbackDetails;
        public GameFeedbackService(IRepository<GameFeedback, int> repo, IRepository<GameFeedbackDetails, int> repoDetail, IRepository<TeamFeedback, int> repoTeamFeedback, 
            IRepository<TeamFeedbackDetails, int> repoTeamFeedbackDetails, IRepository<UserFeedback, int> repoUserFeedback, IRepository<UserFeedbackDetails, int> repoDetailUserFeedbackDetails, IRepository<Look, int> repoLook)
        {
            this.repo = repo;
            this.repoDetail = repoDetail;
            this.repoTeamFeedback = repoTeamFeedback;
            this.repoTeamFeedbackDetails = repoTeamFeedbackDetails;
            this.repoUserFeedback = repoUserFeedback;
            this.repoDetailUserFeedbackDetails = repoDetailUserFeedbackDetails;
            this.repoLook = repoLook;
        }

        public async Task<int> AddUpdateGameAsync(GameFeedbackDto dto)
        {
            if (dto.Id.HasValue)
            {
                var game = repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.GameFeedbackDetails));

                if (game.GameFeedbackDetails != null)
                {
                    var existingPlayer = game.GameFeedbackDetails.Where(x => dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = game.GameFeedbackDetails.Where(x => !dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.GameFeedbackDetails.Where(x => !game.GameFeedbackDetails.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoDetail.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.GameFeedbackDetails.FirstOrDefault(a => a.Id == e.Id);
                            e.Feedback = record.Feedback.HasValue ? EncryptDecrypt.Encrypt(record.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0");
                            e.Quantity = record.Quantity;
                            repoDetail.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<GameFeedbackDetails> addrecords = insertedPlayer.Select(a => new GameFeedbackDetails()
                        {
                            GameFeedbackId = dto.Id.Value,
                            GameId = a.GameId.Value,
                            AttributeId = a.AttributeId.Value,
                            SubAttributeId = a.SubAttributeId.Value,
                            Feedback = a.Feedback.HasValue ? EncryptDecrypt.Encrypt(a.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                            Quantity = a.Quantity,
                            Percentage = a.Percentage
                        }).ToList();

                        await repoDetail.InsertRangeAsyn(addrecords);
                    }
                }

                game.IsDraft = dto.IsDraft;
                game.FeedbackDate = DateTime.Now;
                game.ModifiedDate = DateTime.Now;
                repo.Update(game);
                return dto.Id.Value;
            }
            else
            {
                var gameInfo = new GameFeedback()
                {
                    CompanyId = dto.CompanyId,
                    LookId = dto.LookId,
                    UserId = dto.UserId,
                    IsDraft = dto.IsDraft,
                    FeedbackDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameFeedbackDetails = dto.GameFeedbackDetails.Select(s => new GameFeedbackDetails()
                    {
                        GameId = s.GameId.Value,
                        AttributeId = s.AttributeId.Value,
                        SubAttributeId = s.SubAttributeId.Value,
                        Feedback = s.Feedback.HasValue ? EncryptDecrypt.Encrypt(s.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                        Quantity = s.Quantity,
                        Percentage = s.Percentage
                    }).ToList()
                };
                await repo.InsertAsync(gameInfo);
                return gameInfo.Id;
            }

        }
        public async Task<int> AddUpdateTeamAsync(GameFeedbackDto dto)
        {
            if (dto.Id.HasValue)
            {
                var game = repoTeamFeedback.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.TeamFeedbackDetails));

                if (game.TeamFeedbackDetails != null)
                {
                    var existingPlayer = game.TeamFeedbackDetails.Where(x => dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = game.TeamFeedbackDetails.Where(x => !dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.GameFeedbackDetails.Where(x => !game.TeamFeedbackDetails.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoTeamFeedbackDetails.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.GameFeedbackDetails.FirstOrDefault(a => a.Id == e.Id);
                            e.Feedback = record.Feedback.HasValue ? EncryptDecrypt.Encrypt(record.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0");
                            e.Quantity = record.Quantity;
                            repoTeamFeedbackDetails.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<TeamFeedbackDetails> addrecords = insertedPlayer.Select(a => new TeamFeedbackDetails()
                        {
                            TeamFeedbackId = dto.Id.Value,
                            TeamId = a.GameId.Value,
                            AttributeId = a.AttributeId.Value,
                            SubAttributeId = a.SubAttributeId.Value,
                            Feedback = a.Feedback.HasValue ? EncryptDecrypt.Encrypt(a.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                            Quantity = a.Quantity,
                            Percentage = a.Percentage
                        }).ToList();

                        await repoTeamFeedbackDetails.InsertRangeAsyn(addrecords);
                    }
                }

                game.IsDraft = dto.IsDraft;
                game.FeedbackDate = DateTime.Now;
                game.ModifiedDate = DateTime.Now;
                repoTeamFeedback.Update(game);
                return dto.Id.Value;
            }
            else
            {
                var gameInfo = new TeamFeedback()
                {
                    CompanyId = dto.CompanyId,
                    LookId = dto.LookId,
                    UserId = dto.UserId,
                    IsDraft = dto.IsDraft,
                    FeedbackDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    TeamFeedbackDetails = dto.GameFeedbackDetails.Select(s => new TeamFeedbackDetails()
                    {
                        TeamId = s.GameId.Value,
                        AttributeId = s.AttributeId.Value,
                        SubAttributeId = s.SubAttributeId.Value,
                        Feedback = s.Feedback.HasValue ? EncryptDecrypt.Encrypt(s.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                        Quantity = s.Quantity,
                        Percentage = s.Percentage
                    }).ToList()
                };
                await repoTeamFeedback.InsertAsync(gameInfo);
                return gameInfo.Id;
            }

        }
        public async Task<int> AddUpdateUserAsync(GameFeedbackDto dto)
        {
            if (dto.Id.HasValue)
            {
                var game = repoUserFeedback.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.UserFeedbackDetails));

                if (game.UserFeedbackDetails != null)
                {
                    var existingPlayer = game.UserFeedbackDetails.Where(x => dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = game.UserFeedbackDetails.Where(x => !dto.GameFeedbackDetails.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.GameFeedbackDetails.Where(x => !game.UserFeedbackDetails.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoDetailUserFeedbackDetails.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.GameFeedbackDetails.FirstOrDefault(a => a.Id == e.Id);
                            e.Feedback = record.Feedback.HasValue ? EncryptDecrypt.Encrypt(record.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0");
                            e.Quantity = record.Quantity;
                            repoDetailUserFeedbackDetails.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<UserFeedbackDetails> addrecords = insertedPlayer.Select(a => new UserFeedbackDetails()
                        {
                            UserFeedbackId = dto.Id.Value,
                            UserId = a.GameId.Value,
                            AttributeId = a.AttributeId.Value,
                            SubAttributeId = a.SubAttributeId.Value,
                            Feedback = a.Feedback.HasValue ? EncryptDecrypt.Encrypt(a.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                            Quantity = a.Quantity,
                            Percentage = a.Percentage
                        }).ToList();

                        await repoDetailUserFeedbackDetails.InsertRangeAsyn(addrecords);
                    }
                }

                game.IsDraft = dto.IsDraft;
                game.FeedbackDate = DateTime.Now;
                game.ModifiedDate = DateTime.Now;
                repoUserFeedback.Update(game);
                return dto.Id.Value;
            }
            else
            {
                var gameInfo = new UserFeedback()
                {
                    CompanyId = dto.CompanyId,
                    LookId = dto.LookId,
                    UserId = dto.UserId,
                    IsDraft = dto.IsDraft,
                    FeedbackDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    UserFeedbackDetails = dto.GameFeedbackDetails.Select(s => new UserFeedbackDetails()
                    {
                        UserId = s.GameId.Value,
                        AttributeId = s.AttributeId.Value,
                        SubAttributeId = s.SubAttributeId.Value,
                        Feedback = s.Feedback.HasValue ? EncryptDecrypt.Encrypt(s.Feedback.Value.ToString()) : EncryptDecrypt.Encrypt("0"),
                        Quantity = s.Quantity,
                        Percentage = s.Percentage
                    }).ToList()
                };
                await repoUserFeedback.InsertAsync(gameInfo);
                return gameInfo.Id;
            }

        }

        public async Task<GameFeedbackDto> GetGameByLookId(int id,List<int> gameids,int userId)
        {
            var look = repoLook.Get(id);
            if (this.repo.GetAll(x => x.IsDraft && x.LookId == id && x.UserId == userId).Any())
            {
                var maxId = this.repo.GetAll(x => x.IsDraft && x.LookId == id && x.UserId== userId).Max(m => m.Id);
                var x = this.repo.GetIncludingById(x => x.LookId == id && x.Id == maxId && x.FeedbackDate>= look.FromDate, x => x.Include(m => m.GameFeedbackDetails));
                return new GameFeedbackDto()
                {

                    CompanyId = x.CompanyId,
                    IsDraft = x.IsDraft,
                    LookId = x.LookId,
                    UserId = x.UserId,
                    Id=x.Id,
                    GameFeedbackDetails = x.GameFeedbackDetails.Where(s => gameids.Any() ? gameids.Contains(s.GameId) : true).Select(s => new GameFeedbackDetailsDto()
                    {
                        GameId = s.GameId,
                        Id = s.Id,
                        AttributeId = s.AttributeId,
                        SubAttributeId = s.SubAttributeId,
                        Feedback = !string.IsNullOrEmpty(s.Feedback)? int.Parse(EncryptDecrypt.Decrypt(s.Feedback)) : (int?)null,
                        Quantity = s.Quantity.HasValue ? s.Quantity.Value : 0,
                        Percentage = s.Percentage.HasValue ? s.Percentage.Value : 0
                    }).ToList()
                };
            }
            else {
                return new GameFeedbackDto();
            }
        }
        public async Task<GameFeedbackDto> GetTeamByLookId(int id,List<int> teamIds, int userId)
        {
            var look = repoLook.Get(id);
            if (this.repoTeamFeedback.GetAll(x => x.IsDraft && x.LookId == id && x.UserId == userId).Any())
            {
                var maxId = this.repoTeamFeedback.GetAll(x => x.IsDraft && x.LookId == id && x.UserId==userId).Max(m => m.Id);
                var x = this.repoTeamFeedback.GetIncludingById(x => x.LookId == id && x.Id == maxId && x.FeedbackDate >= look.FromDate, x => x.Include(m => m.TeamFeedbackDetails));
                return new GameFeedbackDto()
                {

                    CompanyId = x.CompanyId,
                    IsDraft = x.IsDraft,
                    LookId = x.LookId,
                    UserId = x.UserId,
                    Id=x.Id,
                    GameFeedbackDetails = x.TeamFeedbackDetails.Where(s => teamIds.Any() ? teamIds.Contains(s.TeamId) : true).Select(s => new GameFeedbackDetailsDto()
                    {
                        GameId = s.TeamId,
                        Id = s.Id,
                        AttributeId = s.AttributeId,
                        SubAttributeId = s.SubAttributeId,
                        Feedback = !string.IsNullOrEmpty(s.Feedback) ? int.Parse(EncryptDecrypt.Decrypt(s.Feedback)) : (int?)null,
                        Quantity = s.Quantity.HasValue ? s.Quantity.Value : 0,
                        Percentage = s.Percentage.HasValue ? s.Percentage.Value : 0
                    }).ToList()
                };
            }
            else {

                return new GameFeedbackDto();
            }
        }

        public async Task<GameFeedbackDto> GetUserByLookId(int id,List<int> userIds, int userId)
        {
            var look = repoLook.Get(id);
            if (this.repoUserFeedback.GetAll(x =>x.IsDraft && x.LookId == id && x.UserId == userId).Any())
            {
                var maxId = this.repoUserFeedback.GetAll(x => x.IsDraft && x.LookId == id && x.UserId==userId).Max(m => m.Id);
                var x = this.repoUserFeedback.GetIncludingById(x => x.LookId == id && x.Id == maxId && x.FeedbackDate >= look.FromDate, x => x.Include(m => m.UserFeedbackDetails));
                return new GameFeedbackDto()
                {

                    CompanyId = x.CompanyId,
                    IsDraft = x.IsDraft,
                    LookId = x.LookId,
                    UserId = x.UserId,
                    Id=x.Id,
                    GameFeedbackDetails = x.UserFeedbackDetails.Where(s => userIds.Any() ? userIds.Contains(s.UserId) : true).Select(s => new GameFeedbackDetailsDto()
                    {
                        GameId = s.UserId,
                        Id = s.Id,
                        AttributeId = s.AttributeId,
                        SubAttributeId = s.SubAttributeId,
                        Feedback = !string.IsNullOrEmpty(s.Feedback) ? int.Parse(EncryptDecrypt.Decrypt(s.Feedback)) : (int?)null,
                        Quantity = s.Quantity.HasValue ? s.Quantity.Value : 0,
                        Percentage = s.Percentage.HasValue ? s.Percentage.Value : 0
                    }).ToList()
                };
            }
            else
            {
                return new GameFeedbackDto();
            }
        }

        public async Task<bool> IsDraftExist(int lookId, int userId) => await this.repo.AnyAsync(x => x.LookId == lookId && x.UserId == userId && x.IsDraft);

        public async Task<bool> IsDraftExistTeam(int lookId, int userId) => await this.repoTeamFeedback.AnyAsync(x => x.LookId == lookId && x.UserId == userId && x.IsDraft);

        public async Task<bool> IsDraftExistUser(int lookId, int userId) => await this.repoUserFeedback.AnyAsync(x => x.LookId == lookId && x.UserId == userId && x.IsDraft);
    }
}

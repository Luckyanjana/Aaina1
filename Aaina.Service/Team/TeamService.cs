using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Service
{
    public class TeamService : ITeamService
    {
        private IRepository<Team, int> repo;
        private IRepository<TeamPlayer, int> repoPlayer;
        public TeamService(IRepository<Team, int> repo, IRepository<TeamPlayer, int> repoPlayer)
        {
            this.repo = repo;
            this.repoPlayer = repoPlayer;
        }

        public async Task<TeamDto> GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.TeamPlayer));
            return new TeamDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive,
                TeamPlayer = x.TeamPlayer.Select(s => new TeamPlayerDto()
                {
                    TeamId = s.TeamId,
                    Id = s.Id,
                    RoleId = s.RoleId,
                    TypeId = s.TypeId,
                    UserId = s.UserId
                }).ToList()
            };
        }

        public async Task<TeamDto> GetDetailsId(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new TeamDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                CompanyId = x.CompanyId,
                GameId = x.GameId
            };
        }
        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(TeamDto dto)
        {
            if (dto.Id.HasValue)
            {
                var team = repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.TeamPlayer));

                if (team.TeamPlayer != null)
                {
                    var existingPlayer = team.TeamPlayer.Where(x => dto.TeamPlayer.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = team.TeamPlayer.Where(x => !dto.TeamPlayer.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.TeamPlayer.Where(x => !team.TeamPlayer.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoPlayer.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.TeamPlayer.FirstOrDefault(a => a.Id == e.Id);
                            e.RoleId = record.RoleId.Value;
                            e.UserId = record.UserId;
                            e.TypeId = record.TypeId;
                            e.ModifiedDate = DateTime.Now;
                            repoPlayer.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<TeamPlayer> addrecords = insertedPlayer.Select(a => new TeamPlayer()
                        {
                            TeamId = dto.Id.Value,
                            RoleId = a.RoleId.Value,
                            TypeId = a.TypeId,
                            UserId = a.UserId,
                            CompanyId = team.CompanyId,
                            AddedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        }).ToList();

                        await repoPlayer.InsertRangeAsyn(addrecords);
                    }
                }

                team.Name = dto.Name;
                team.Weightage = dto.Weightage.Value;
                team.Desciption = dto.Desciption;
                team.GameId = dto.GameId;
                team.IsActive = dto.IsActive;
                team.Weightage = dto.Weightage.Value;
                team.ModifiedDate = DateTime.Now;
                repo.Update(team);
                return dto.Id.Value;
            }
            else
            {
                var teamInfo = new Team()
                {
                    CompanyId = dto.CompanyId,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    Weightage = dto.Weightage.Value,
                    IsActive = dto.IsActive,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    CreatedBy = dto.CreatedBy,
                    TeamPlayer = dto.TeamPlayer.Select(s => new TeamPlayer()
                    {
                        AddedDate = DateTime.Now,
                        CompanyId = dto.CompanyId,
                        ModifiedDate = DateTime.Now,
                        RoleId = s.RoleId.Value,
                        TypeId = s.TypeId,
                        UserId = s.UserId
                    }).ToList()
                };
                await repo.InsertAsync(teamInfo);
                return teamInfo.Id;
            }

        }
        public List<TeamDto> GetAll(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new TeamDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive
            }).ToList();

        }

        public List<TeamDto> GetAll(int companyId, int? userId, int gameId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId && (userId.HasValue ? x.CreatedBy == userId : true)).Select(x => new TeamDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy
            }).ToList();

        }

        public List<TeamDto> GetAllActive(int companyId, int? userId, int gameId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId && (userId.HasValue ? x.CreatedBy == userId : true) && x.IsActive).Select(x => new TeamDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                CreatedBy = x.CreatedBy
            }).ToList();

        }

        public List<SelectedItemDto> GetAllDrop(int? id, int companyId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && x.Id != id && x.IsActive).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
        }
        public List<SelectedItemDto> GetTeamList(int? id, int companyId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId && x.Id != id && x.IsActive).Select(user => new SelectedItemDto()
            {

                Id = user.Id.ToString(),
                Name = user.Name

            }).ToList();
        }
        //public List<SelectedItemDto> TeamSearchByName(string userName)
        //{
        //    return repo.GetAll(x => x.Name.ToLower().StartsWith(userName.ToLower())).Select(user => new SelectedItemDto()
        //    {

        //        Id = user.Id.ToString(),
        //        Name = user.Name

        //    }).ToList();
        //}
        public string[] TeamPlayerIds(int[] teamIds)
        {
            var playerIds = repoPlayer.GetAll(x => teamIds.Contains(x.TeamId)).Select(x => x.UserId).ToArray();
            return Array.ConvertAll(playerIds, s => Convert.ToString(s));
        }
        public void DeleteBy(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.TeamPlayer));
                repoPlayer.DeleteRange(x.TeamPlayer.ToList());
                repo.Delete(x);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw ex;
            }
        }
    }
}

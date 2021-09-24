using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public class GameService : IGameService
    {
        private IRepository<Game, int> repo;
        private IRepository<Team, int> repoTeam;
        private IRepository<GamePlayer, int> repoPlayer;
        private IRepository<GameLocation, int> repoLocation;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        public GameService(IConfiguration configuration, IRepository<Game, int> repo, IRepository<GamePlayer, int> repoPlayer, IRepository<Team, int> repoTeam,
            IRepository<GameLocation, int> repoLocation)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");
            this.repo = repo;
            this.repoPlayer = repoPlayer;
            this.repoTeam = repoTeam;
            this.repoLocation = repoLocation;
        }

        public async Task<GameDto> GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.GamePlayer).Include(m => m.GameLocation));
            return new GameDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                IsActive = x.IsActive,
                GameLocation = x.GameLocation.Select(s => s.Location).ToList(),
                GamePlayers = x.GamePlayer.Select(s => new GamePlayerDto()
                {
                    GameId = s.GameId,
                    Id = s.Id,
                    RoleId = s.RoleId,
                    TypeId = s.TypeId,
                    UserId = s.UserId
                }).ToList(),
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate
            };
        }
        public string GetClientName(int id)
        {
            return this.repo.Get(id).ClientName;

        }

        public async Task<GameDto> GetDetailsId(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new GameDto()

            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate,
                IsActive = x.IsActive
            };
        }
        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(GameDto dto)
        {
            if (dto.Id.HasValue)
            {
                var game = repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.GamePlayer).Include(m => m.GameLocation));

                if (game.GamePlayer != null)
                {
                    var existingPlayer = game.GamePlayer.Where(x => dto.GamePlayers.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = game.GamePlayer.Where(x => !dto.GamePlayers.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.GamePlayers.Where(x => !game.GamePlayer.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoPlayer.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.GamePlayers.FirstOrDefault(a => a.Id == e.Id);
                            e.RoleId = record.RoleId.Value;
                            e.UserId = record.UserId;
                            e.TypeId = record.TypeId;
                            e.ModifiedDate = DateTime.Now;
                            repoPlayer.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<GamePlayer> addrecords = insertedPlayer.Select(a => new GamePlayer()
                        {
                            GameId = dto.Id.Value,
                            RoleId = a.RoleId.Value,
                            TypeId = a.TypeId,
                            UserId = a.UserId,
                            CompanyId = game.CompanyId,
                            AddedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        }).ToList();

                        await repoPlayer.InsertRangeAsyn(addrecords);
                    }
                }

                if (game.GameLocation.Any())
                {
                    repoLocation.DeleteRange(game.GameLocation.ToList());
                }

                game.Name = dto.Name;
                game.Weightage = dto.Weightage.Value;
                game.Desciption = dto.Desciption;
                game.ApplyForChild = dto.ApplyForChild;
                game.ClientName = dto.ClientName;
                game.ContactNumber = dto.ContactNumber;
                game.ContactPerson = dto.ContactPerson;
                game.Location = dto.Location;
                game.FromDate = dto.FromDate;
                game.Todate = dto.Todate;
                game.Weightage = dto.Weightage.Value;
                game.ModifiedDate = DateTime.Now;
                game.IsActive = dto.IsActive;
                game.GameLocation = dto.GameLocation.Select(s => new GameLocation()
                {
                    GameId = dto.Id.Value,
                    Location = s
                }).ToList();
                repo.Update(game);
                return dto.Id.Value;
            }
            else
            {

                //await repo.InsertAsync(gameInfo);

                var teamInfo = new Team()
                {
                    CompanyId = dto.CompanyId,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    Weightage = dto.Weightage.Value,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    IsActive = true,
                    CreatedBy = dto.CreatedBy,
                    TeamPlayer = dto.GamePlayers.Select(s => new TeamPlayer()
                    {
                        AddedDate = DateTime.Now,
                        CompanyId = dto.CompanyId,
                        ModifiedDate = DateTime.Now,
                        RoleId = s.RoleId.Value,
                        TypeId = s.TypeId,
                        UserId = s.UserId
                    }).ToList(),
                    Game = new Game()
                    {
                        CompanyId = dto.CompanyId,
                        Desciption = dto.Desciption,
                        Name = dto.Name,
                        Weightage = dto.Weightage.Value,
                        ModifiedDate = DateTime.Now,
                        AddedDate = DateTime.Now,
                        ApplyForChild = dto.ApplyForChild,
                        ClientName = dto.ClientName,
                        ContactNumber = dto.ContactNumber,
                        ContactPerson = dto.ContactPerson,
                        FromDate = dto.FromDate,
                        ParentId = dto.ParentId,
                        Todate = dto.Todate,
                        Location = dto.Location,
                        CreatedBy = dto.CreatedBy,
                        IsActive = true,
                        GameLocation = dto.GameLocation.Select(a => new GameLocation()
                        {
                            Location = a
                        }).ToList(),
                        GamePlayer = dto.GamePlayers.Select(s => new GamePlayer()
                        {
                            AddedDate = DateTime.Now,
                            CompanyId = dto.CompanyId,
                            ModifiedDate = DateTime.Now,
                            RoleId = s.RoleId.Value,
                            TypeId = s.TypeId,
                            UserId = s.UserId
                        }).ToList()
                    }
                };

                await repoTeam.InsertAsync(teamInfo);

                return teamInfo.Game.Id;
            }

        }
        public List<GameDto> GetAll(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new GameDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate,
                IsActive = x.IsActive,
                LastUpdate = x.ModifiedDate
            }).ToList();

        }

        public List<GameDto> GetTopParent(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && !x.ParentId.HasValue).Select(x => new GameDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate,
                IsActive = x.IsActive,
                LastUpdate = x.ModifiedDate
            }).ToList();

        }

        public List<GameDto> GetAll(int companyId, int? userId, int gameId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && (userId.HasValue ? x.CreatedBy == userId : true) && x.ParentId == gameId).Select(x => new GameDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate,
                IsActive = x.IsActive,
                LastUpdate = x.ModifiedDate,
                CreatedBy = x.CreatedBy
            }).ToList();

        }

        public GameDto GetFirstGame(int companyId)
        {
            var x = repo.FirstOrDefault(x => x.CompanyId == companyId && !x.ParentId.HasValue);

            return x != null ? new GameDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ApplyForChild = x.ApplyForChild,
                ClientName = x.ClientName,
                CompanyId = x.CompanyId,
                ContactNumber = x.ContactNumber,
                ContactPerson = x.ContactPerson,
                FromDate = x.FromDate,
                Location = x.Location,
                ParentId = x.ParentId,
                Todate = x.Todate
            } : new GameDto();

        }

        public List<SelectedItemDto> GetAllDrop(int? id, int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.Id != id).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

        }


        public List<SelectedItemDto> GetAllDropParent(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && !x.ParentId.HasValue).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

        }

        public List<SelectedItemDto> GetAllDropSecondParent(int companyId)
        {

            var parentId = repo.GetAllList(x => x.CompanyId == companyId && !x.ParentId.HasValue).Select(x => x.Id).ToList();
            return repo.GetAll(x => x.ParentId.HasValue && parentId.Contains(x.ParentId.Value)).ToList().Select(x => new SelectedItemDto()
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();

        }

        public List<SelectedItemDto> GetAllDropByParent(int parentId)
        {

            return repo.GetAllList(x => x.ParentId == parentId).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

        }



        public List<GameGridDto> GetAllByParentId(int? parentId, int companyId)
        {
            var list = repo.GetAllList(x => x.CompanyId == companyId).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();


            List<GameGridDto> response = list.Where(x => x.ParentId == parentId).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();

            response.ForEach(x =>
            {
                x.ChildGame = GetChild(list, x.Id);
            });

            return response;

        }

        public List<GameGridDto> GetAllByParentId(int? parentId, int companyId, List<int> gameIds)
        {
            var list = repo.GetAllList(x => x.CompanyId == companyId && (gameIds.Any() ? gameIds.Contains(x.Id) : true)).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();


            List<GameGridDto> response = list.Where(x => x.ParentId == parentId).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();

            response.ForEach(x =>
            {
                x.ChildGame = GetChild(list, x.Id);
            });

            return response;

        }

        public List<int> GetAllIdByParentId(int? parentId, int companyId)
        {
            var list = repo.GetAllList(x => x.CompanyId == companyId).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();

            List<int> response1 = list.Where(x => x.ParentId == parentId).Select(x => x.Id).ToList();

            List<GameGridDto> response = list.Where(x => x.ParentId == parentId).Select(x => new GameGridDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ParentId = x.ParentId,
                Todate = x.Todate
            }).ToList();

            foreach (var x in response)
            {
                response1.AddRange(GetChildId(list, x.Id));
            }


            return response1;

        }
        private List<GameGridDto> GetChild(List<GameGridDto> list, int parentId)
        {
            List<GameGridDto> response = list.Where(x => x.ParentId == parentId).ToList();
            response.ForEach(x =>
            {
                x.ChildGame = GetChild(list, x.Id);
            });

            return response;

        }

        private List<int> GetChildId(List<GameGridDto> list, int parentId)
        {
            List<int> response1 = list.Where(x => x.ParentId == parentId).Select(s => s.Id).ToList();
            List<GameGridDto> response = list.Where(x => x.ParentId == parentId).ToList();
            foreach (var x in response)
            {
                response1.AddRange(GetChildId(list, x.Id));
            }

            return response1;

        }

        public async Task<List<GameMenuDto>> GetMenu(int companyId)
        {
            List<GameMenuDto> response = new List<GameMenuDto>();
            var alla = await repo.GetAllListAsync(x => x.CompanyId == companyId);
            var all = alla.Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId
            }).ToList();

            var allParent = all.Where(x => !x.ParentId.HasValue).ToList();

            List<int> parentId = allParent.Select(x => x.Id).ToList();

            var allChild1 = all.Where(x => x.ParentId.HasValue && parentId.Contains(x.ParentId.Value)).ToList();

            List<int> childId = allChild1.Select(x => x.Id).ToList();

            var allChilds2 = all.Where(x => x.ParentId.HasValue && childId.Contains(x.ParentId.Value)).ToList();

            var allChild2 = allChilds2.Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId
            }).ToList();

            foreach (var par in allParent)
            {
                GameMenuDto parentResponse = new GameMenuDto();
                parentResponse = par;
                foreach (var child in allChild1.Where(x => x.ParentId == par.Id).ToList())
                {
                    GameMenuDto childResponse = new GameMenuDto();
                    childResponse = child;
                    childResponse.ChildGame = allChild2.Where(x => x.ParentId == child.Id).ToList();
                    parentResponse.ChildGame.Add(childResponse);
                }
                response.Add(parentResponse);
            }

            return response;
        }

        public async Task<List<GameMenuDto>> GetMenuNLevel(int companyId)
        {
            List<GameMenuDto> response = new List<GameMenuDto>();
            var alla = await repo.GetAllListAsync(x => x.CompanyId == companyId);
            var response1 = alla.Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId
            }).OrderBy(o => o.ParentId).ThenBy(c => c.Id).ToList();


            response.AddRange(response1.Where(x => !x.ParentId.HasValue).Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId
            }).OrderBy(o => o.ParentId).ThenBy(c => c.Id).ToList());
            response.ForEach(s =>
            {
                s.ChildGame.AddRange(GetForChartMenu(response1, s.Id));
            });

            return response;
        }
        public async Task<List<GameMenuDto>> GetAllForChart(int companyId)
        {
            List<GameMenuDto> response = new List<GameMenuDto>();
            var alla = await repo.GetAllListAsync(x => x.CompanyId == companyId);
            response = alla.Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0
            }).OrderBy(o => o.ParentId).ThenBy(c => c.Id).ToList();

            return response;
        }

        public async Task<List<GameMenuDto>> GetAllForChart(int parentId, int companyId)
        {
            List<GameMenuDto> response = new List<GameMenuDto>();
            var alla = await repo.GetAllListAsync(x => x.CompanyId == companyId);
            var response1 = alla.Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0
            }).OrderBy(o => o.ParentId).ThenBy(c => c.Id).ToList();


            response.AddRange(response1.Where(x => (x.Id == parentId || x.ParentId == parentId)).Select(x => new GameMenuDto()
            {
                Name = x.Name,
                Id = x.Id,
                ParentId = x.Id == parentId ? 0 : x.ParentId.HasValue ? x.ParentId.Value : 0
            }).OrderBy(o => o.ParentId).ThenBy(c => c.Id).ToList());
            response.Select(x => x.Id).ToList().ForEach(s =>
            {
                response.AddRange(GetForChart(response1, s));
            });

            return response;
        }

        private List<GameMenuDto> GetForChart(List<GameMenuDto> list, int parentId)
        {
            List<GameMenuDto> response = list.Where(x => x.ParentId == parentId).ToList();
            response.Select(x => x.Id).ToList().ForEach(s =>
            {
                response.AddRange(GetForChart(list, s));
            });
            return response;

        }

        private List<GameMenuDto> GetForChartMenu(List<GameMenuDto> list, int parentId)
        {
            List<GameMenuDto> response = list.Where(x => x.ParentId == parentId).ToList();
            response.ForEach(s =>
            {
                s.ChildGame.AddRange((GetForChartMenu(list, s.Id)));
            });
            return response;

        }

        public async Task<List<UserGameRole>> GetPlayerGamleRole(int companyId, int userId)
        {
            return await repoPlayer.GetAll().Where(x => x.CompanyId == companyId && x.UserId == userId).Select(x => new UserGameRole { Id = x.Id, GameId = x.GameId, RoleId = x.RoleId }).ToListAsync();
        }

        public async Task<List<GameUser>> GetGamlePlayer(int companyId) =>
            repoPlayer.GetAllIncluding(x => x.CompanyId == companyId && x.User.IsActive, x => x.Include(m => m.Role).Include(m => m.User)).ToList().GroupBy(x => x.UserId).Select(x => new GameUser { Role = x.OrderByDescending(o => o.Role.Weightage).FirstOrDefault().Role.Name, UserId = x.Key, Name = x.FirstOrDefault().User.Fname + " " + x.FirstOrDefault().User.Lname, UserTypeId = x.FirstOrDefault().TypeId }).ToList();


        public async Task<List<WeightagePresetDetailsDto>> GetOnlyGamlePlayer(int companyId, List<int> gameId) =>
            repoPlayer.GetAllIncluding(x => x.CompanyId == companyId && gameId.Contains(x.GameId), x => x.Include(m => m.Role)).ToList().Select(x => new WeightagePresetDetailsDto
            {
                Role = x.Role.Name,
                UserId = x.UserId,
                RoleId = x.RoleId,
                GameId = x.GameId,
                ColorCode = x.Role.ColorCode,
                Weightage = x.Role.Weightage
            }).ToList();

        public List<SelectedItemDto> GetGamePlayerList(int gameId) =>
            repoPlayer.GetAllIncluding(x => x.GameId == gameId, x => x.Include(m => m.User)).ToList().Select(x => new SelectedItemDto
            {
                Id = x.UserId.ToString(),
                Name = x.User.Fname + " " + x.User.Lname
            }).ToList();


        public async Task<bool> AddUpdatePlayerGamleRole(int companyId, int userId, List<UserGameRole> dto)
        {
            var allGameRole = repoPlayer.GetAllList(x => x.UserId == userId);

            var existingPlayer = allGameRole.Where(x => dto.Any(scdet => scdet.Id == x.Id)).ToList();
            var deletedPlayer = allGameRole.Where(x => !dto.Any(scdet => scdet.Id == x.Id)).ToList();
            var insertedPlayer = dto.Where(x => !allGameRole.Any(m => m.Id == x.Id)).ToList();

            if (deletedPlayer.Any())
            {
                this.repoPlayer.DeleteRange(deletedPlayer);
            }

            if (existingPlayer.Any())
            {
                foreach (var e in existingPlayer)
                {
                    var record = dto.FirstOrDefault(a => a.Id == e.Id);
                    e.RoleId = record.RoleId.Value;
                    e.UserId = userId;
                    e.TypeId = (int)PlayersType.Manpower;
                    e.ModifiedDate = DateTime.Now;
                    repoPlayer.Update(e);
                }
            }
            if (insertedPlayer.Any())
            {
                List<GamePlayer> addrecords = insertedPlayer.Select(a => new GamePlayer()
                {
                    GameId = a.GameId,
                    RoleId = a.RoleId.Value,
                    TypeId = (int)PlayersType.Manpower,
                    UserId = userId,
                    CompanyId = companyId,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                }).ToList();

                await repoPlayer.InsertRangeAsyn(addrecords);
            }

            return true;
        }

        public void DeleteBy(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.GamePlayer).Include(m => m.GameLocation));
                repoPlayer.DeleteRange(x.GamePlayer.ToList());
                repoLocation.DeleteRange(x.GameLocation.ToList());
                repo.Delete(x);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }
        }

        public async Task<List<MenuPermissionListDto>> GetMenuStatic(int userId, int gameId)
        {
            List<MenuPermissionListDto> response = new List<MenuPermissionListDto>();
            var allmenus = GetMenuWithPermission(userId, gameId);
            var allParentMenu = allmenus.Where(x => !x.ParentId.HasValue).ToList();


            foreach (var par in allParentMenu)
            {
                MenuPermissionListDto parentResponse = new MenuPermissionListDto();
                parentResponse = par;
                foreach (var chid in allmenus.Where(x => x.ParentId == par.MenuId).ToList())
                {
                    MenuPermissionListDto childResponse = new MenuPermissionListDto();
                    childResponse = chid;
                    parentResponse.ChildMenu.Add(childResponse);
                }
                response.Add(parentResponse);
            }

            return response;
        }

        public async Task<List<MenuPermissionListDto>> GetUserMenuStatic(int userId)
        {
            List<MenuPermissionListDto> response = new List<MenuPermissionListDto>();
            var allmenus = GetUserMenuWithPermission(userId);
            var allParentMenu = allmenus.Where(x => !x.ParentId.HasValue).ToList();


            foreach (var par in allParentMenu)
            {
                MenuPermissionListDto parentResponse = new MenuPermissionListDto();
                parentResponse = par;
                foreach (var chid in allmenus.Where(x => x.ParentId == par.MenuId).ToList())
                {
                    MenuPermissionListDto childResponse = new MenuPermissionListDto();
                    childResponse = chid;
                    parentResponse.ChildMenu.Add(childResponse);
                }
                response.Add(parentResponse);
            }

            return response;
        }
        private List<MenuPermissionListDto> GetMenuWithPermission(int userId, int gameId)
        {
            List<MenuPermissionListDto> list = SqlHelper.ConvertDataTable<MenuPermissionListDto>(SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetMenuWithPermission]",
                       new SqlParameter("@userId", userId), new SqlParameter("@gameId", gameId)).Tables[0]);
            return list;
        }

        private List<MenuPermissionListDto> GetUserMenuWithPermission(int userId)
        {
            List<MenuPermissionListDto> list = SqlHelper.ConvertDataTable<MenuPermissionListDto>(SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetUserMenuWithPermission]",
                       new SqlParameter("@userId", userId)).Tables[0]);
            return list;
        }

        public MenuDto GetPagePermission(int userId, int roleId, string controllerName, string actionName)
        {
            MenuDto list = SqlHelper.ConvertDataTable<MenuDto>(SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetPagePermission]",
                      new SqlParameter("@userId", userId), new SqlParameter("@roleId", roleId),
                      new SqlParameter("@controllerName", controllerName), new SqlParameter("@actionName", actionName)).Tables[0]).FirstOrDefault();
            return list;
        }
    }
}

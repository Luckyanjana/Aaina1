using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Service
{
    public class FilterService : IFilterService
    {
        private IRepository<Filter, int> repo;
        private IRepository<FilterAttributes, int> repoAttributes;
        private IRepository<FilterPlayers, int> repoPlayers;
        private IRepository<FilterEmotionsFor, int> repoEmotionsFor;
        private IRepository<FilterEmotionsFrom, int> repoEmotionsFrom;
        private IRepository<FilterEmotionsFromP, int> repoEmotionsFromP;
        public FilterService(IRepository<Filter, int> repo, IRepository<FilterAttributes, int> repoAttributes, IRepository<FilterPlayers, int> repoPlayers, IRepository<FilterEmotionsFor, int> repoEmotionsFor,
            IRepository<FilterEmotionsFrom, int> repoEmotionsFrom, IRepository<FilterEmotionsFromP, int> repoEmotionsFromP)
        {
            this.repo = repo;
            this.repoAttributes = repoAttributes;
            this.repoPlayers = repoPlayers;
            this.repoEmotionsFor = repoEmotionsFor;
            this.repoEmotionsFrom = repoEmotionsFrom;
            this.repoEmotionsFromP = repoEmotionsFromP;
        }

        public async Task<FilterDto> GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.FilterEmotionsFor).Include(m => m.FilterEmotionsFrom).Include(m => m.FilterEmotionsFromP).Include(m => m.FilterAttributes).Include(m => m.FilterPlayers));
            return new FilterDto()
            {
                Name = x.Name,
                AttributeIds = x.FilterAttributes.Select(s => s.AttributeId).ToList(),
                CalculationType = x.CalculationType,
                EmotionsFor = x.EmotionsFor,
                FromIds = x.FilterEmotionsFrom.Select(s => s.FromId).ToList(),
                EmotionsFrom = x.EmotionsFrom,
                EmotionsFromP = x.EmotionsFromP,
                EndDateTime = x.EndDateTime,
                ForIds = x.FilterEmotionsFor.Select(s => s.ForId).ToList(),
                FromPIds = x.FilterEmotionsFromP.Select(s => s.FromId).ToList(),
                Id = x.Id,
                StartDateTime = x.StartDateTime,
                IsSelf=x.IsSelf,
                Players = x.FilterPlayers.Select(s => new FilterPlayersDto()
                {
                    IsCalculation = s.IsCalculation,
                    IsView = s.IsView,
                    UserId = s.UserId
                }).ToList()
            };
        }

        public async Task<int> AddUpdateAsync(FilterDto dto)
        {
            if (dto.Id.HasValue)
            {
                var filter = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.FilterEmotionsFor).Include(m => m.FilterEmotionsFrom).Include(m => m.FilterEmotionsFromP).Include(m => m.FilterAttributes).Include(m => m.FilterPlayers));


                if (filter.FilterPlayers != null && filter.FilterPlayers.Any())
                {
                    this.repoPlayers.DeleteRange(filter.FilterPlayers.ToList());
                }

                if (filter.FilterEmotionsFor != null && filter.FilterEmotionsFor.Any())
                {
                    this.repoEmotionsFor.DeleteRange(filter.FilterEmotionsFor.ToList());
                }

                if (filter.FilterEmotionsFrom != null && filter.FilterEmotionsFrom.Any())
                {
                    this.repoEmotionsFrom.DeleteRange(filter.FilterEmotionsFrom.ToList());
                }

                if (filter.FilterEmotionsFromP != null && filter.FilterEmotionsFromP.Any())
                {
                    this.repoEmotionsFromP.DeleteRange(filter.FilterEmotionsFromP.ToList());
                }

                if (filter.FilterAttributes != null && filter.FilterAttributes.Any())
                {
                    this.repoAttributes.DeleteRange(filter.FilterAttributes.ToList());
                }


                filter.IsSelf = dto.IsSelf;
                filter.Name = dto.Name;
                filter.CalculationType = dto.CalculationType.Value;
                filter.EmotionsFor = dto.EmotionsFor.Value;
                filter.EmotionsFrom = dto.EmotionsFrom.Value;
                filter.EmotionsFromP = dto.EmotionsFromP.Value;
                filter.EndDateTime = dto.EndDateTime.Value;
                filter.StartDateTime = dto.StartDateTime.Value;
                filter.Name = dto.Name;
                filter.GameId = dto.GameId;
                filter.ModifiedDate = DateTime.Now;
                filter.FilterAttributes = dto.AttributeIds.Select(s => new FilterAttributes()
                {
                    AttributeId = s,
                    FilterId = dto.Id.Value
                }).ToList();

                filter.FilterEmotionsFor = dto.ForIds.Select(s => new FilterEmotionsFor()
                {
                    ForId = s,
                    FilterId = dto.Id.Value
                }).ToList();

                filter.FilterEmotionsFrom = dto.FromIds.Select(s => new FilterEmotionsFrom()
                {
                    FromId = s,
                    FilterId = dto.Id.Value
                }).ToList();

                filter.FilterEmotionsFromP = dto.FromPIds.Select(s => new FilterEmotionsFromP()
                {
                    FromId = s,
                    FilterId = dto.Id.Value
                }).ToList();

                filter.FilterPlayers = dto.Players.Select(s => new FilterPlayers()
                {
                    UserId = s.UserId,
                    FilterId = dto.Id.Value,
                    IsView = s.IsView,
                    IsCalculation = s.IsCalculation
                }).ToList();

                repo.Update(filter);
                return dto.Id.Value;
            }
            else
            {
                var filterInfo = new Filter()
                {
                    IsSelf = dto.IsSelf,
                    CompanyId = dto.CompanyId,
                    Name = dto.Name,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    CalculationType = dto.CalculationType.Value,
                    EmotionsFor = dto.EmotionsFor.Value,
                    EmotionsFrom = dto.EmotionsFrom.Value,
                    EndDateTime = dto.EndDateTime.Value,
                    StartDateTime = dto.StartDateTime.Value,
                    CreatedBy=dto.CreatedBy,
                    GameId=dto.GameId,
                    FilterAttributes = dto.AttributeIds.Select(s => new FilterAttributes()
                    {
                        AttributeId = s,
                    }).ToList(),

                    FilterEmotionsFor = dto.ForIds.Select(s => new FilterEmotionsFor()
                    {
                        ForId = s,
                    }).ToList(),

                    FilterEmotionsFrom = dto.FromIds.Select(s => new FilterEmotionsFrom()
                    {
                        FromId = s,
                    }).ToList(),


                    FilterEmotionsFromP = dto.FromPIds.Select(s => new FilterEmotionsFromP()
                    {
                        FromId = s,
                    }).ToList(),

                    FilterPlayers = dto.Players.Select(s => new FilterPlayers()
                    {
                        UserId = s.UserId,
                        IsView = s.IsView,
                        IsCalculation = s.IsCalculation
                    }).ToList()
                };
                await repo.InsertAsync(filterInfo);
                return filterInfo.Id;
            }

        }

        public List<FilterDto> GetAll(int companyId, int typeId)
        {
            var currentDate = DateTime.Now;
            return repo.GetAllList(x => x.CompanyId == companyId && x.StartDateTime<= currentDate && x.EndDateTime>= currentDate && (typeId!=0?x.EmotionsFor == typeId:true)).Select(x => new FilterDto()
            {
                Name = x.Name,
                Id = x.Id,
                CalculationType = x.CalculationType,
                StartDateTime = x.StartDateTime,
                EndDateTime = x.EndDateTime,
                EmotionsFrom = x.EmotionsFrom,
                EmotionsFor = x.EmotionsFor
            }).ToList();

        }
        public List<FilterDto> GetAll(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new FilterDto()
            {
                Name = x.Name,
                Id = x.Id,
                CalculationType = x.CalculationType,
                StartDateTime = x.StartDateTime,
                EndDateTime = x.EndDateTime,
                EmotionsFrom = x.EmotionsFrom,
                EmotionsFor = x.EmotionsFor
            }).ToList();

        }

        public List<FilterDto> GetAll(int companyId,int? userId,int gameId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId==gameId && (userId.HasValue?x.CreatedBy==userId:true)).Select(x => new FilterDto()
            {
                Name = x.Name,
                Id = x.Id,
                CalculationType = x.CalculationType,
                StartDateTime = x.StartDateTime,
                EndDateTime = x.EndDateTime,
                EmotionsFrom = x.EmotionsFrom,
                EmotionsFor = x.EmotionsFor,
                CreatedBy=x.CreatedBy
            }).ToList();

        }

        public Tuple<List<int>,bool> EmotionsForId(int filterId)
        {
            var list = repoEmotionsFor.GetAllList(x => x.FilterId == filterId).Select(x => x.ForId).ToList();
            var isSelf = repo.Get(filterId).IsSelf;

            return new Tuple<List<int>, bool>(list, isSelf);

        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task Delete(int id)
        {
            var context = this.repo.GetContext();
            
            try
            {
                context.Database.BeginTransaction();
                var filter = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.FilterEmotionsFor).Include(m => m.FilterEmotionsFrom).Include(m => m.FilterEmotionsFromP).Include(m => m.FilterAttributes).Include(m => m.FilterPlayers));
                if (filter.FilterPlayers != null && filter.FilterPlayers.Any())
                {
                    this.repoPlayers.DeleteRange(filter.FilterPlayers.ToList());
                }

                if (filter.FilterEmotionsFor != null && filter.FilterEmotionsFor.Any())
                {
                    this.repoEmotionsFor.DeleteRange(filter.FilterEmotionsFor.ToList());
                }

                if (filter.FilterEmotionsFrom != null && filter.FilterEmotionsFrom.Any())
                {
                    this.repoEmotionsFrom.DeleteRange(filter.FilterEmotionsFrom.ToList());
                }

                if (filter.FilterEmotionsFromP != null && filter.FilterEmotionsFromP.Any())
                {
                    this.repoEmotionsFromP.DeleteRange(filter.FilterEmotionsFromP.ToList());
                }

                if (filter.FilterAttributes != null && filter.FilterAttributes.Any())
                {
                    this.repoAttributes.DeleteRange(filter.FilterAttributes.ToList());
                }

                this.repo.Delete(filter);
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();

                throw;
            }
            
        }
    }
}

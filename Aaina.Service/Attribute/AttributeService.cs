using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Service
{
    public class AttributeService : IAttributeService
    {
        private IRepository<Data.Models.Attribute, int> repo;
        private IRepository<SubAttribute, int> repoSubAttribute;
        public AttributeService(IRepository<Data.Models.Attribute, int> repo, IRepository<SubAttribute, int> repoSubAttribute)
        {
            this.repo = repo;
            this.repoSubAttribute = repoSubAttribute;
        }
        public async Task<AttributeDto> GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.SubAttribute));
            return new AttributeDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive,
                SubAttribute = x.SubAttribute.Select(s => new SubAttributeDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Desciption = s.Desciption,
                    Weightage = s.Weightage,
                    IsQuantity = s.IsQuantity,
                    UnitId = s.UnitId
                }).ToList()
            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }


        public List<AttributeDto> GetAll(int companyId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new AttributeDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                IsActive = x.IsActive
            }).ToList();

        }

        public List<AttributeDto> GetAllWithSub(int companyId)
        {

            return this.repo.GetAllIncluding(x => x.CompanyId == companyId, x => x.Include(m => m.SubAttribute)).Select(x => new AttributeDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                SubAttribute = x.SubAttribute.Select(s => new SubAttributeDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Desciption = s.Desciption,
                    Weightage = s.Weightage,
                    UnitId = s.UnitId,
                    IsQuantity = s.IsQuantity
                }).ToList()
            }).ToList();

        }

        public List<SelectedItemDto> GetSubList(int attrId)
        {

            return this.repoSubAttribute.GetAll(x => x.AttributeId == attrId).ToList().Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString(),

            }).ToList();

        }
        public async Task<int> AddUpdateAsync(AttributeDto dto)
        {
            if (dto.Id.HasValue)
            {
                var attribute = repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.SubAttribute));

                if (attribute.SubAttribute != null)
                {
                    var existingSubAttribute = attribute.SubAttribute.Where(x => dto.SubAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedSubAttribute = attribute.SubAttribute.Where(x => !dto.SubAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedSubAttribute = dto.SubAttribute.Where(x => !attribute.SubAttribute.Any(m => m.Id == x.Id)).ToList();


                    if (deletedSubAttribute.Any())
                    {
                        this.repoSubAttribute.DeleteRange(deletedSubAttribute);
                    }

                    if (existingSubAttribute.Any())
                    {
                        foreach (var e in existingSubAttribute)
                        {
                            var record = dto.SubAttribute.FirstOrDefault(a => a.Id == e.Id);
                            e.Name = record.Name;
                            e.Weightage = record.Weightage;
                            e.Desciption = record.Desciption;
                            e.IsQuantity = record.IsQuantity;
                            e.UnitId = record.UnitId;
                            repoSubAttribute.Update(e);
                        }
                    }
                    if (insertedSubAttribute.Any())
                    {
                        List<SubAttribute> addrecords = insertedSubAttribute.Select(a => new SubAttribute()
                        {
                            Name = a.Name,
                            Weightage = a.Weightage,
                            Desciption = a.Desciption,
                            IsQuantity = a.IsQuantity,
                            UnitId = a.UnitId,
                            AttributeId = dto.Id.Value
                        }).ToList();

                        await repoSubAttribute.InsertRangeAsyn(addrecords);
                    }
                }

                attribute.Name = dto.Name;
                attribute.Desciption = dto.Desciption;
                attribute.GameId = dto.GameId;
                attribute.ModifiedDate = DateTime.Now;
                attribute.IsActive = dto.IsActive;
                repo.Update(attribute);
                return dto.Id.Value;
            }
            else
            {
                var AttributeInfo = new Data.Models.Attribute()
                {
                    CompanyId = dto.CompanyId,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    IsActive = dto.IsActive,
                    SubAttribute = dto.SubAttribute.Select(s => new SubAttribute()
                    {
                        Name = s.Name,
                        Desciption = s.Desciption,
                        Weightage = s.Weightage,
                        IsQuantity = s.IsQuantity,
                        UnitId = s.UnitId
                    }).ToList()
                };
                await repo.InsertAsync(AttributeInfo);
                return AttributeInfo.Id;
            }

        }
        public void DeleteBy(int id)
        {
            repo.Delete(id);
        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {
            IQueryable<Data.Models.Attribute> query = repo.GetAll(x => x.CompanyId == parameters.CompanyId);
            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<AttributeGridDto> data = new List<AttributeGridDto>();
            foreach (Data.Models.Attribute x in result.Data)
            {
                data.Add(new AttributeGridDto()
                {
                    Name = x.Name,
                    Desciption = x.Desciption,
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    IsActive = x.IsActive ? "Active" : "In active"
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}

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
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Aaina.Service
{
    public class FormBuilderService : IFormBuilderService
    {
        private IRepository<FormBuilder, int> repo;
        private IRepository<FormBuilderAttribute, int> repoAttribute;
        private IRepository<FormBuilderAttributeLookUp, int> repoAttributeLook;
        public FormBuilderService(IRepository<FormBuilder, int> repo, IRepository<FormBuilderAttribute, int> repoAttribute, IRepository<FormBuilderAttributeLookUp, int> repoAttributeLook)
        {
            this.repo = repo;
            this.repoAttribute = repoAttribute;
            this.repoAttributeLook = repoAttributeLook;
        }

        public async Task<FormBuilderDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.FormBuilderAttribute).Include("FormBuilderAttribute.FormBuilderAttributeLookUp"));
            return new FormBuilderDto()
            {
                Name = x.Name,
                Header = x.Header,
                CompanyId = x.CompanyId,
                Id = x.Id,
                Footer = x.Footer,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                FormBuilderAttribute = x.FormBuilderAttribute.Select(a => new FormBuilderAttributeDto()
                {
                    AttributeName = a.AttributeName,
                    DataType = a.DataType,
                    Id = a.Id,
                    IsRequired = a.IsRequired,
                    OrderNo = a.OrderNo,
                    DbcolumnName = a.DbcolumnName,
                    FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(l => new FormBuilderAttributeLookUpDto()
                    {
                        Id = l.Id,
                        OptionName = l.OptionName
                    }).ToList()
                }).ToList()

            };
        }
        public async Task<int> Add(FormBuilderDto requestDto)
        {
            var buildTemplate = new FormBuilder()
            {
                CompanyId = requestDto.CompanyId,
                Header = requestDto.Header,
                Name = requestDto.Name,
                Footer = requestDto.Footer,
                IsActive = requestDto.IsActive,
                CreatedBy = requestDto.CreatedBy,
                Modified = DateTime.Now,
                Created = DateTime.Now,
                FormBuilderAttribute = requestDto.FormBuilderAttribute.Select(a => new FormBuilderAttribute()
                {
                    AttributeName = a.AttributeName,
                    DataType = a.DataType,
                    IsRequired = a.IsRequired,
                    OrderNo = a.OrderNo,
                    FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(o => new FormBuilderAttributeLookUp()
                    {
                        OptionName = o.OptionName
                    }).ToList()
                }).ToList()
            };
            await repo.InsertAsync(buildTemplate);
            return buildTemplate.Id;
        }

        public async Task<int> Update(FormBuilderDto dto)
        {

            var buildTemplate = await this.repo.GetIncludingByIdAsyn(x => x.Id == dto.Id, x => x.Include(m => m.FormBuilderAttribute).Include("FormBuilderAttribute.FormBuilderAttributeLookUp"));
            if (buildTemplate.FormBuilderAttribute != null)
            {
                var existingAttribute = buildTemplate.FormBuilderAttribute.Where(x => dto.FormBuilderAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                var deletedAttribute = buildTemplate.FormBuilderAttribute.Where(x => !dto.FormBuilderAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                var insertedAttribute = dto.FormBuilderAttribute.Where(x => !buildTemplate.FormBuilderAttribute.Any(m => m.Id == x.Id)).ToList();


                if (deletedAttribute.Any())
                {
                    foreach (var item in deletedAttribute)
                    {
                        repoAttributeLook.DeleteRange(item.FormBuilderAttributeLookUp.ToList());
                    }
                    this.repoAttribute.DeleteRange(deletedAttribute);
                }

                if (existingAttribute.Any())
                {
                    foreach (var e in existingAttribute)
                    {
                        var record = dto.FormBuilderAttribute.FirstOrDefault(a => a.Id == e.Id);

                        var existingAttributeOptions = e.FormBuilderAttributeLookUp.Where(x => record.FormBuilderAttributeLookUp.Any(scdet => scdet.Id == x.Id)).ToList();
                        var deletedAttributeOptions = e.FormBuilderAttributeLookUp.Where(x => !record.FormBuilderAttributeLookUp.Any(scdet => scdet.Id == x.Id)).ToList();
                        var insertedAttributeOptions = record.FormBuilderAttributeLookUp.Where(x => !e.FormBuilderAttributeLookUp.Any(m => m.Id == x.Id)).ToList();

                        if (deletedAttributeOptions.Any())
                        {
                            repoAttributeLook.DeleteRange(deletedAttributeOptions);
                        }

                        if (existingAttributeOptions.Any())
                        {
                            foreach (var eo in existingAttributeOptions)
                            {
                                var recordo = record.FormBuilderAttributeLookUp.FirstOrDefault(a => a.Id == eo.Id);
                                eo.OptionName = recordo.OptionName;
                                repoAttributeLook.Update(eo);
                            }
                        }
                        if (insertedAttributeOptions.Any())
                        {
                            List<FormBuilderAttributeLookUp> addrecordso = insertedAttributeOptions.Select(a => new FormBuilderAttributeLookUp()
                            {
                                OptionName = a.OptionName,
                                FormBuilderAttributeId = record.Id.Value

                            }).ToList();

                            await repoAttributeLook.InsertRangeAsyn(addrecordso);
                        }
                        e.AttributeName = record.AttributeName;
                        e.DataType = record.DataType;
                        e.IsRequired = record.IsRequired;
                        e.OrderNo = record.OrderNo;
                        e.DbcolumnName = record.DbcolumnName;
                        repoAttribute.Update(e);
                    }
                }

                if (insertedAttribute.Any())
                {
                    List<FormBuilderAttribute> addrecords = insertedAttribute.Select(a => new FormBuilderAttribute()
                    {
                        FormBuilderId = dto.Id.Value,
                        AttributeName = a.AttributeName,
                        DataType = a.DataType,
                        IsRequired = a.IsRequired,
                        OrderNo = a.OrderNo,
                        DbcolumnName = a.DbcolumnName,
                        FormBuilderAttributeLookUp = a.FormBuilderAttributeLookUp.Select(o => new FormBuilderAttributeLookUp()
                        {
                            OptionName = o.OptionName
                        }).ToList()

                    }).ToList();

                    await repoAttribute.InsertRangeAsyn(addrecords);
                }
            }

            buildTemplate.Header = dto.Header;
            buildTemplate.Name = dto.Name;
            buildTemplate.Footer = dto.Footer;
            buildTemplate.IsActive = dto.IsActive;
            buildTemplate.Modified = DateTime.Now;
            await repo.UpdateAsync(buildTemplate);
            return dto.Id.Value;
        }

        public List<FormBuilderDto> GetAll(int companyId)
        {
            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new FormBuilderDto()
            {
                Id = x.Id,
                Name = x.Name,
                Header = x.Header,
                Footer = x.Footer,
                IsActive = x.IsActive
            }).ToList();

        }

        public void DeleteBy(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.FormBuilderAttribute).Include("FormBuilderAttribute.FormBuilderAttributeLookUp"));
            foreach (var item in x.FormBuilderAttribute)
            {
                repoAttributeLook.DeleteRange(item.FormBuilderAttributeLookUp.ToList());
            }
            repoAttribute.DeleteRange(x.FormBuilderAttribute.ToList());
            repo.Delete(x);

        }

        public void DeleteAtterbute(int id)
        {
            repoAttribute.Delete(id);

        }

        public void DeleteAtterbuteLook(int id)
        {
            repoAttributeLook.Delete(id);

        }

    }
}

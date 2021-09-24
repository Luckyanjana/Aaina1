using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

namespace Aaina.Service
{
    public class RoleService : IRoleService
    {
        private IRepository<Role, int> repo;
        public RoleService(IRepository<Role, int> repo)
        {
            this.repo = repo;
        }

        public async Task<RoleDto> GetById(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new RoleDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ColorCode=x.ColorCode,
                IsActive=x.IsActive,
                PlayerType=x.PlayerType
            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> Add(RoleDto requestDto)
        {

            var userInfo = new Role()
            {
                CompanyId = requestDto.CompanyId,
                Desciption = requestDto.Desciption,
                Name = requestDto.Name,
                Weightage = requestDto.Weightage.Value,
                ColorCode=requestDto.ColorCode,
                IsActive=requestDto.IsActive,
                PlayerType=requestDto.PlayerType,
                ModifiedDate = DateTime.Now,
                AddedDate = DateTime.Now
            };
            await repo.InsertAsync(userInfo);
            return userInfo.Id;
        }

        public async Task<int> Update(RoleDto requestDto)
        {
            var user = repo.Get(requestDto.Id.Value);
            user.Name = requestDto.Name;
            user.Weightage = requestDto.Weightage.Value;
            user.Desciption = requestDto.Desciption;
            user.ColorCode = requestDto.ColorCode;
            user.IsActive = requestDto.IsActive;
            user.PlayerType = requestDto.PlayerType;
            user.ModifiedDate = DateTime.Now;
            await repo.UpdateAsync(user);
            return requestDto.Id.Value;
        }

        public List<RoleDto> GetAll(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new RoleDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                Weightage = x.Weightage,
                ColorCode=x.ColorCode,
                PlayerType=x.PlayerType,
                IsActive=x.IsActive
            }).ToList();

        }

        public void DeleteBy(int id)
        {
            try
            {
                repo.Delete(id);
            }
            catch (Exception ex)
            {

                throw;
            }
            
            
        }
    }
}

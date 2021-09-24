using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

namespace Aaina.Service
{
    public class CompanyService : ICompanyService
    {
        private IRepository<Company, int> repo;
        public CompanyService(IRepository<Company, int> repo)
        {
            this.repo = repo;
        }

        public async Task<CompanyDto> GetById(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new CompanyDto()
            {
                Name = x.Name,
                Address = x.Address,
                Id = x.Id,
                Location = x.Location,
                Desciption = x.Desciption,
                IsActive = x.IsActive
            };
        }


        public async Task<Company> FirstOrDefault()
        {
            var x = await this.repo.FirstOrDefaultAsync(x=>x.Id>1);
            return x;
        }
        public async Task<bool> IsExist(string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> Add(CompanyDto requestDto)
        {

            var userInfo = new Company()
            {
                Address = requestDto.Address,
                Name = requestDto.Name,
                Desciption = requestDto.Desciption,
                IsActive = true,
                Location = requestDto.Location,
                ModifiedDate = DateTime.Now,
                AddedDate = DateTime.Now
            };
            await repo.InsertAsync(userInfo);
            return userInfo.Id;
        }

        public async Task<int> Update(CompanyDto requestDto)
        {
            var user = repo.Get(requestDto.Id);
            user.Name = requestDto.Name;
            user.Address = requestDto.Address;
            user.Desciption = requestDto.Desciption;
            user.Location = requestDto.Location;
            user.ModifiedDate = DateTime.Now;
            await repo.UpdateAsync(user);
            return requestDto.Id;
        }

        public List<CompanyDto> GetAll()
        {

            return repo.GetAllList(x => x.Id > 1).Select(x => new CompanyDto()
            {
                Name = x.Name,
                Address = x.Address,
                Id = x.Id,
                Location = x.Location,
                Desciption = x.Desciption,
                IsActive = x.IsActive
            }).ToList();

        }

        public void UpdateDriveId(int companyId, string driveId)
        {
            var user = repo.Get(companyId);
            user.DriveId = driveId;
            repo.Update(user);
        }

        public void DeleteBy(int id)
        {
            repo.Delete(id);
        }
    }
}

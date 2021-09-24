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
    public class WeightageService : IWeightageService
    {
        private IRepository<Weightage, int> repo;
        public WeightageService(IRepository<Weightage, int> repo)
        {
            this.repo = repo;
        }

        public async Task<WeightageDto> GetById(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new WeightageDto()
            {
                Name = x.Name,
                Emoji = x.Emoji,
                Id = x.Id,
                Rating = x.Rating,
                IsActive = x.IsActive
            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x =>x.CompanyId== companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<bool> IsExistRating(int companyId, double rating, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId==companyId && x.Id != id && x.Rating == rating);
            return result > 0;
        }

        public async Task<int> Add(WeightageDto requestDto)
        {

            var userInfo = new Weightage()
            {
                CompanyId = requestDto.CompanyId,
                Emoji = requestDto.Emoji,
                Name = requestDto.Name,
                Rating = requestDto.Rating.Value,
                IsActive = requestDto.IsActive,
                ModifiedDate = DateTime.Now,
                AddedDate = DateTime.Now
            };
            await repo.InsertAsync(userInfo);
            return userInfo.Id;
        }

        public async Task<int> Update(WeightageDto requestDto)
        {
            var user = repo.Get(requestDto.Id.Value);
            user.Name = requestDto.Name;
            user.Rating = requestDto.Rating.Value;
            user.Emoji = requestDto.Emoji;
            user.IsActive = requestDto.IsActive;
            user.ModifiedDate = DateTime.Now;
            await repo.UpdateAsync(user);
            return requestDto.Id.Value;
        }

        public List<WeightageDto> GetAll(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId).Select(x => new WeightageDto()
            {
                Name = x.Name,
                Emoji = x.Emoji,
                Id = x.Id,
                Rating = x.Rating,
                IsActive = x.IsActive
            }).ToList();

        }

        public List<WeightageDto> GetAllActive(int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.IsActive).Select(x => new WeightageDto()
            {
                Name = x.Name,
                Emoji = x.Emoji,
                Id = x.Id,
                Rating = x.Rating,
                IsActive = x.IsActive
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

                throw ex;
            }
             
            
        }
    }
}

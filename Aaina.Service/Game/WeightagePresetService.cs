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
    public class WeightagePresetService: IWeightagePresetService
    {
        private IRepository<WeightagePreset, int> repo;
        private IRepository<WeightagePresetDetails, int> repoDetails;
        public WeightagePresetService(IRepository<WeightagePreset, int> repo, IRepository<WeightagePresetDetails, int> repoDetails)
        {
            this.repo = repo;
            this.repoDetails = repoDetails;
        }

        public async Task<WeightagePresetDto> GetById(int id)
        {
            var x = await this.repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.WeightagePresetDetails).Include("WeightagePresetDetails.Role"));
            return new WeightagePresetDto()
            {
                Name = x.Name,
                GameId = x.GameId,
                WeightagePresetDetails = x.WeightagePresetDetails.Select(s => new WeightagePresetDetailsDto()
                {
                    GameId = s.GameId,
                    RoleId = s.RoleId,
                    UserId = s.UserId,
                    Weightage = s.Weightage,
                    ColorCode=s.Role.ColorCode,
                    Role=s.Role.Name
                }).ToList()
            };
        }

        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.Game.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> Add(WeightagePresetDto dto)
        {

            var userInfo = new WeightagePreset()
            {
                GameId = dto.GameId,
                Name = dto.Name,
                IsDefault=dto.IsDefault,
                AddedDate=DateTime.Now,
                WeightagePresetDetails = dto.WeightagePresetDetails.Select(s => new WeightagePresetDetails()
                {
                    GameId = s.GameId.Value,
                    RoleId = s.RoleId.Value,
                    UserId = s.UserId,
                    Weightage = s.Weightage.Value
                }).ToList()
            };
            await repo.InsertAsync(userInfo);
            return userInfo.Id;
        }

        public List<SelectedItemDto> GetAllDropdown(int companyId, int gameId)
        {

            return repo.GetAllList(x => x.Game.CompanyId == companyId && x.GameId == gameId).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();

        }

        public void DeleteBy(int id)
        {            repo.Delete(id);
        }
    }
}

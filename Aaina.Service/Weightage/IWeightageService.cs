using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
   public interface IWeightageService
    {
        Task<WeightageDto> GetById(int id);
        Task<bool> IsExist(int companyId, string name, int? id);

        Task<bool> IsExistRating(int companyId, double rating, int? id);

        Task<int> Add(WeightageDto requestDto);

        Task<int> Update(WeightageDto requestDto);

        List<WeightageDto> GetAll(int companyId);

        List<WeightageDto> GetAllActive(int companyId);

        void DeleteBy(int id);
    }
}

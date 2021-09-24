using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IWeightagePresetService
    {
        Task<WeightagePresetDto> GetById(int id);
        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> Add(WeightagePresetDto dto);

        List<SelectedItemDto> GetAllDropdown(int companyId, int gameId);

    }
}

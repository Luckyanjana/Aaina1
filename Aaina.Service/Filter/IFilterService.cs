using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IFilterService
    {
        Task<FilterDto> GetById(int id);

        Task<int> AddUpdateAsync(FilterDto dto);

        List<FilterDto> GetAll(int companyId, int typeId);

        List<FilterDto> GetAll(int companyId);

        List<FilterDto> GetAll(int companyId, int? userId, int gameId);

        Tuple<List<int>, bool> EmotionsForId(int filterId);
        Task<bool> IsExist(int companyId, string name, int? id);
        Task Delete(int id);
    }
}

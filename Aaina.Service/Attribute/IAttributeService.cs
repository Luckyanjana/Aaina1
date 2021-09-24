using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IAttributeService
    {
        Task<AttributeDto> GetById(int id);
        Task<bool> IsExist(int companyId, string name, int? id);

        List<AttributeDto> GetAll(int companyId);

        List<AttributeDto> GetAllWithSub(int companyId);

        Task<int> AddUpdateAsync(AttributeDto dto);

        void DeleteBy(int id);

        List<SelectedItemDto> GetSubList(int attrId);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);
    }
}

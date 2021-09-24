using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IRoleService
    {
        Task<RoleDto> GetById(int id);
        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> Add(RoleDto requestDto);

        Task<int> Update(RoleDto requestDto);

        List<RoleDto> GetAll(int companyId);

        void DeleteBy(int id);
    }
}

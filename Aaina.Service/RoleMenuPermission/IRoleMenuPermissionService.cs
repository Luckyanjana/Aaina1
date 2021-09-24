using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Service
{
    public interface IRoleMenuPermissionService
    {
        void Save(List<RoleMenuPermissionDto> roleMenuPermissionlist, int createdBy, int roleId);
        List<RoleMenuPermissionDto> GetAll();

        List<MenuPermissionDto> GetByRoleIdAll(int roleId);
    }
}

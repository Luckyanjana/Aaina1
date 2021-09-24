using System;
using System.Collections.Generic;
using System.Text;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IUserMenuPermissionService
    {
        void Save(List<UserMenuPermissionDto> roleMenuPermissionlist, int createdBy, int userId);
        List<UserMenuPermissionDto> GetAll();

        List<MenuPermissionDto> GetByUserIdAll(int userId);
            }
}

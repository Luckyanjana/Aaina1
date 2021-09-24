using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface IMenuService
    {
        List<MenuDto> GetAll();
        void ActivateDeactivate(int id, bool isActive);

        MenuDto GetById(int id);

        bool AddUpdate(MenuDto dto);

        Task<bool> IsExist(string name, int? id);
        List<SelectedItemDto> GetDropdown();

        List<MenuDto> GetMenuWithChildAll();
    }
}

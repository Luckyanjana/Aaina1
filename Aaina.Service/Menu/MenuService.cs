using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public class MenuService : IMenuService
    {

        private readonly IRepository<Menu, int> _repoMenu;
        public MenuService(IRepository<Menu, int> repoMenu)
        {
            this._repoMenu = repoMenu;
        }

        public void ActivateDeactivate(int id, bool IsActive)
        {
            var menu = _repoMenu.Get(id);
            menu.IsActive = IsActive;
            _repoMenu.Update(menu);
        }

        public List<MenuDto> GetAll()
        {
            var menuitems = _repoMenu.GetAllList();
            var menulst = new List<MenuDto>();
            foreach (var menuitem in menuitems)
            {
                var parentid = menuitem.ParentId.HasValue ? menuitem.ParentId.Value : 0;
                var menu = new MenuDto
                {
                    Id = menuitem.Id,
                    Name = menuitem.Name,
                    IsMain = menuitem.IsMain,
                    Action = menuitem.Action,
                    Controller = menuitem.Controller,
                    ParentId = menuitem.ParentId,
                    IsActive = menuitem.IsActive,
                    Order = menuitem.Order,
                    ParentName = menuitems.Find(x => x.Id == parentid)?.Name
                };
                menulst.Add(menu);
            }
            return menulst;
        }

        public List<MenuDto> GetMenuWithChildAll()
        {
            var entities = _repoMenu.GetAllList();
            var menulst = new List<MenuDto>();
            foreach (var x in entities.Where(x => !x.ParentId.HasValue).OrderBy(o => o.Order).ToList())
            {
                var menu = new MenuDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    Action = x.Action,
                    Controller = x.Controller,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Order = x.Order,
                    ChildMenu = GetChildMenu(entities, x.Id)
                };
                menulst.Add(menu);
            }
            return menulst;
        }
        
        public List<SelectedItemDto> GetDropdown()
        {
            return _repoMenu.GetAllList(x => x.IsMain).OrderBy(o => o.Order).Select(x => new SelectedItemDto()
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();

        }

        public MenuDto GetById(int id)
        {
            var x = _repoMenu.Get(id);
            return new MenuDto
            {
                Action = x.Action,
                Controller = x.Controller,
                Id = x.Id,
                IsActive = x.IsActive,
                IsMain = x.IsMain,
                Name = x.Name,
                Order = x.Order,
                ParentId = x.ParentId
            };
        }

        public bool AddUpdate(MenuDto dto)
        {
            if (dto.Id.HasValue)
            {
                var x = _repoMenu.Get(dto.Id.Value);
                x.Action = dto.Action;
                x.Controller = dto.Controller;
                x.IsActive = dto.IsActive;
                x.IsMain = dto.IsMain;
                x.Name = dto.Name;
                x.Order = dto.Order;
                x.ParentId = dto.ParentId;
                x.UpdatedDate = DateTime.Now;
                _repoMenu.Update(x);
            }
            else
            {
                _repoMenu.Insert(new Data.Models.Menu()
                {
                    Action = dto.Action,
                    Controller = dto.Controller,
                    IsActive = dto.IsActive,
                    IsMain = dto.IsMain,
                    Name = dto.Name,
                    Order = dto.Order,
                    ParentId = dto.ParentId,
                    UpdatedDate = DateTime.Now,
                    CreatedDate = DateTime.Now
                });
            }
            return true;
        }

        public async Task<bool> IsExist(string name, int? id)
        {
            var result = await this._repoMenu.CountAsync(x => x.Id != id && x.Name == name);
            return result > 0;
        }

        private List<MenuDto> GetChildMenu(List<Menu> entities, int parentId)
        {
            var menulst = new List<MenuDto>();
            foreach (var x in entities.Where(x => x.ParentId == parentId).OrderBy(o => o.Order).ToList())
            {
                var menu = new MenuDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    Action = x.Action,
                    Controller = x.Controller,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Order = x.Order,
                    ChildMenu = GetChildMenu(entities, x.Id)
                };
                menulst.Add(menu);
            }
            return menulst;
        }
    }
}

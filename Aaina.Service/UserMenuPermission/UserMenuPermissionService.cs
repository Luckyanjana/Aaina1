using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aaina.Service
{
    public class UserMenuPermissionService : IUserMenuPermissionService
    {
        private IRepository<UserMenuPermission, int> repo;
        private IRepository<Menu, int> repoRoleMenu;
        public UserMenuPermissionService(IRepository<UserMenuPermission, int> repo, IRepository<Menu, int> repoRoleMenu)
        {
            this.repo = repo;
            this.repoRoleMenu = repoRoleMenu;
        }

        public List<UserMenuPermissionDto> GetAll()
        {
            var roleMenuPermissionlist = repo.GetAllList();
            var roleMenuPermissionlst = new List<UserMenuPermissionDto>();
            foreach (var item in roleMenuPermissionlist)
            {
                var rolemenupermission = new UserMenuPermissionDto
                {
                    MenuId = item.MenuId.HasValue ? item.MenuId.Value : 0,
                    UserId = item.UserId.HasValue ? item.UserId.Value : 0,
                    IsAdd = item.IsAdd,
                    IsDelete = item.IsDelete,
                    IsEdit = item.IsEdit,
                    IsList = item.IsList,
                    IsView = item.IsView
                };
                roleMenuPermissionlst.Add(rolemenupermission);
            }
            return roleMenuPermissionlst;
        }

        public void Save(List<UserMenuPermissionDto> roleMenuPermissionlist, int createdBy, int userId)
        {
            var allMenu = repo.GetAllList(x => x.UserId == userId);

            var existingOption = allMenu.Where(x => roleMenuPermissionlist.Any(scdet => scdet.MenuId == x.MenuId)).ToList();
            var deletedOption = allMenu.Where(x => !roleMenuPermissionlist.Any(scdet => scdet.MenuId == x.MenuId)).ToList();
            var insertedOption = roleMenuPermissionlist.Where(x => !allMenu.Any(m => m.MenuId == x.MenuId)).ToList();

            if (deletedOption.Any())
            {

                this.repo.DeleteRange(deletedOption);
            }

            if (existingOption.Any())
            {
                foreach (var e in existingOption)
                {
                    var item = roleMenuPermissionlist.FirstOrDefault(a => a.MenuId == e.MenuId);
                    e.IsAdd = item.IsAdd;
                    e.IsDelete = item.IsDelete;
                    e.IsEdit = item.IsEdit;
                    e.IsList = item.IsList;
                    e.IsView = item.IsView;
                    repo.Update(e);
                }
            }
            if (insertedOption.Any())
            {
                List<UserMenuPermission> addrecords = insertedOption.Select(item => new UserMenuPermission
                {
                    MenuId = item.MenuId,
                    UserId = item.UserId,
                    IsAdd = item.IsAdd,
                    IsDelete = item.IsDelete,
                    IsEdit = item.IsEdit,
                    IsList = item.IsList,
                    IsView = item.IsView,
                    CreatedBy = createdBy
                }).ToList();
                repo.InsertRange(addrecords);
            }           

        }

        public List<MenuPermissionDto> GetByUserIdAll(int userId)
        {
            var entities = repoRoleMenu.GetAllList();
            var permissionEntities = repo.GetAllList(x => x.UserId == userId);
            var menulst = new List<MenuPermissionDto>();
            foreach (var x in entities.Where(x => !x.ParentId.HasValue).OrderBy(o => o.Order).ToList())
            {
                var roleP = permissionEntities.Any(a => a.MenuId == x.Id && a.UserId == userId) ?
                    permissionEntities.FirstOrDefault(a => a.MenuId == x.Id && a.UserId == userId) : new UserMenuPermission();
                var menu = new MenuPermissionDto
                {
                    MenuId = x.Id,
                    RoleId = userId,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    ParentId = x.ParentId,
                    Order = x.Order,
                    IsAdd = roleP.IsAdd,
                    IsList = roleP.IsList,
                    IsEdit = roleP.IsEdit,
                    IsDelete = roleP.IsDelete,
                    IsView = roleP.IsView,
                    ChildMenu = GetChildMenu(entities, permissionEntities, x.Id, userId)
                };
                menulst.Add(menu);
            }
            return menulst;
        }

        private List<MenuPermissionDto> GetChildMenu(List<Menu> entities, List<UserMenuPermission> permissionEntities, int parentId, int userId)
        {
            var menulst = new List<MenuPermissionDto>();
            var list = entities.Where(x => x.ParentId == parentId).OrderBy(o => o.Order).ToList();
            foreach (var x in list)
            {
                var roleP = permissionEntities.Any(a => a.MenuId == x.Id && a.UserId == userId) ?
                    permissionEntities.FirstOrDefault(a => a.MenuId == x.Id && a.UserId == userId) : new UserMenuPermission();

                menulst.Add(new MenuPermissionDto
                {
                    MenuId = x.Id,
                    RoleId = userId,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    ParentId = x.ParentId,
                    Order = x.Order,
                    IsAdd = roleP.IsAdd,
                    IsList = roleP.IsList,
                    IsEdit = roleP.IsEdit,
                    IsDelete = roleP.IsDelete,
                    IsView = roleP.IsView,
                    //ChildMenu = GetChildMenu(entities, permissionEntities, x.Id, roleId)
                });
            }
            return menulst;
        }
        private bool CheckExistsRoleMenuPermission(UserMenuPermissionDto rolemenupermission)
        {
            var findrecord = repo.GetAllList(x => x.UserId == rolemenupermission.UserId && x.MenuId == rolemenupermission.MenuId);
            if (findrecord.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aaina.Service
{
    public class RoleMenuPermissionService : IRoleMenuPermissionService
    {
        private IRepository<RoleMenuPermission, int> repo;
        private IRepository<Menu, int> repoRoleMenu;
        public RoleMenuPermissionService(IRepository<RoleMenuPermission, int> repoRoleMenuPermission, IRepository<Menu, int> repoRoleMenu)
        {
            this.repo = repoRoleMenuPermission;
            this.repoRoleMenu = repoRoleMenu;
        }

        public List<RoleMenuPermissionDto> GetAll()
        {
            var roleMenuPermissionlist = repo.GetAllList();
            var roleMenuPermissionlst = new List<RoleMenuPermissionDto>();
            foreach (var item in roleMenuPermissionlist)
            {
                var rolemenupermission = new RoleMenuPermissionDto
                {
                    MenuId = item.MenuId.HasValue ? item.MenuId.Value : 0,
                    RoleId = item.RoleId.HasValue ? item.RoleId.Value : 0,
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

        public void Save(List<RoleMenuPermissionDto> roleMenuPermissionlist,int createdBy,int roleId)
        {
            var allMenu = repo.GetAllList(x => x.RoleId == roleId);

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
                List<RoleMenuPermission> addrecords = insertedOption.Select(item => new RoleMenuPermission
                {
                    MenuId = item.MenuId,
                    RoleId = item.RoleId,
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

        public List<MenuPermissionDto> GetByRoleIdAll(int roleId)
        {
            var entities = repoRoleMenu.GetAllList();
            var permissionEntities = repo.GetAllList(x => x.RoleId == roleId);
            var menulst = new List<MenuPermissionDto>();
            foreach (var x in entities.Where(x => !x.ParentId.HasValue).OrderBy(o => o.Order).ToList())
            {
                var roleP = permissionEntities.Any(a => a.MenuId == x.Id && a.RoleId == roleId) ?
                    permissionEntities.FirstOrDefault(a => a.MenuId == x.Id && a.RoleId == roleId) : new RoleMenuPermission();
                var menu = new MenuPermissionDto
                {
                    MenuId = x.Id,
                    RoleId = roleId,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    ParentId = x.ParentId,
                    Order = x.Order,
                    IsAdd = roleP.IsAdd,
                    IsList = roleP.IsList,
                    IsEdit = roleP.IsEdit,
                    IsDelete = roleP.IsDelete,
                    IsView = roleP.IsView,
                    ChildMenu = GetChildMenu(entities, permissionEntities, x.Id, roleId)
                };
                menulst.Add(menu);
            }
            return menulst;
        }

        private List<MenuPermissionDto> GetChildMenu(List<Menu> entities, List<RoleMenuPermission> permissionEntities, int parentId, int roleId)
        {
            var menulst = new List<MenuPermissionDto>();
            var list= entities.Where(x => x.ParentId == parentId).OrderBy(o => o.Order).ToList();
            foreach (var x in list)
            {
                var roleP = permissionEntities.Any(a => a.MenuId == x.Id && a.RoleId == roleId) ?
                    permissionEntities.FirstOrDefault(a => a.MenuId == x.Id && a.RoleId == roleId) : new RoleMenuPermission();
                                
                menulst.Add(new MenuPermissionDto
                {
                    MenuId = x.Id,
                    RoleId = roleId,
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
        private bool CheckExistsRoleMenuPermission(RoleMenuPermissionDto rolemenupermission)
        {
            var findrecord = repo.GetAllList(x => x.RoleId == rolemenupermission.RoleId && x.MenuId == rolemenupermission.MenuId);
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

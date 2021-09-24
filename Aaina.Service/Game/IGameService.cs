using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;


namespace Aaina.Service
{
    public interface IGameService
    {
        Task<GameDto> GetById(int id);

        string GetClientName(int id);

        Task<GameDto> GetDetailsId(int id);
        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(GameDto dto);

        List<GameDto> GetAll(int companyId);

        List<GameDto> GetTopParent(int companyId);

        List<GameDto> GetAll(int companyId, int? userId, int gameId);

        GameDto GetFirstGame(int companyId);
        List<SelectedItemDto> GetAllDrop(int? id, int companyId);

        List<SelectedItemDto> GetAllDropParent(int companyId);

        List<SelectedItemDto> GetAllDropSecondParent(int companyId);

        List<SelectedItemDto> GetAllDropByParent(int parentId);

        List<GameGridDto> GetAllByParentId(int? parentId, int companyId);

        List<GameGridDto> GetAllByParentId(int? parentId, int companyId, List<int> gameIds);

        List<int> GetAllIdByParentId(int? parentId, int companyId);

        Task<List<MenuPermissionListDto>> GetMenuStatic(int userId, int gameId);

        Task<List<MenuPermissionListDto>> GetUserMenuStatic(int userId);

        Task<List<GameMenuDto>> GetMenu(int companyId);

        Task<List<GameMenuDto>> GetMenuNLevel(int companyId);

        Task<List<GameMenuDto>> GetAllForChart(int companyId);

        Task<List<GameMenuDto>> GetAllForChart(int parentId, int companyId);

        Task<List<UserGameRole>> GetPlayerGamleRole(int companyId, int userId);

        Task<bool> AddUpdatePlayerGamleRole(int companyId, int userId, List<UserGameRole> dto);
        Task<List<GameUser>> GetGamlePlayer(int companyId);
        void DeleteBy(int id);


        Task<List<WeightagePresetDetailsDto>> GetOnlyGamlePlayer(int companyId, List<int> gameId);

        List<SelectedItemDto> GetGamePlayerList(int gameId);
        MenuDto GetPagePermission(int userId, int roleId, string controllerName, string actionName);
    }
}

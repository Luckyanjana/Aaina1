using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface ITeamService
    {
        Task<TeamDto> GetById(int id);

        Task<TeamDto> GetDetailsId(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(TeamDto dto);

        List<TeamDto> GetAll(int companyId);
        List<TeamDto> GetAll(int companyId, int? userId, int gameId);

        List<TeamDto> GetAllActive(int companyId, int? userId, int gameId);

        List<SelectedItemDto> GetAllDrop(int? id, int companyId);
      //  List<SelectedItemDto> TeamSearchByName(string userName);
        List<SelectedItemDto> GetTeamList(int? id, int companyId);
        string[] TeamPlayerIds(int[] teamIds);
        void DeleteBy(int id);
    }
}

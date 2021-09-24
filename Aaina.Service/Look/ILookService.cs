using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface ILookService
    {

        Task<Tuple<List<GameFeedbackGridDto>,  List<string>,string>> GetGameFeedback(int gId, int? lookid, int? presetId,int? attributeId,int? filterId,DateTime filterFromDate,
            DateTime filterToDate);

        Task<Tuple<List<GameFeedbackGridDto>,  List<string>, string>> GetTeamFeedback(int tId, int? lookid, int? presetId, int? attributeId, int? filterId,DateTime filterFromDate, DateTime filterToDate);

        Task<Tuple<List<GameFeedbackGridDto>,  List<string>, string>> GetUserFeedback(int tId, int? lookid, int? presetId, int? attributeId, int? filterId,DateTime filterFromDate, DateTime filterToDate);

        Task<LookDto> GetById(int id);

        Task<LookDto> GetDetailsId(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(LookDto dto);

        List<LookDto> GetAll(int companyId);

        List<LookDto> GetAll(int companyId, int? userId, int gameId);

        List<LookDto> GetAllByGame(int companyId,int? userId, int gameId);

        List<SelectedItemDto> GetAllDrop(int gid, int companyId, int userId);

        List<SelectedItemDto> GetAllDrop(int gid, int companyId);

        List<SelectedItemDto> GetAllDropPrimission(int gid, int companyId, int userId);

        List<SelectedItemDto> GetAttributeDrop(int lookId);

        List<SelectedItemDto> GetAttributeDropByfilter(int filterId);

        List<SelectedItemDto> GetGroupAllDrop(int? lookId, int? filterId,int? attributeId);

        LookFeebbackDto GetLookFeedbackByLookId(int lookId, int companyId,List<int> gameIds);

        List<GameGridDto> GetLookByLookId(int? lookId);

        List<GameGridDto> GetLookByLookId(int? lookId, List<int> teamIds);

        List<GameGridDto> GetUserByLookId(int? lookId);

        List<GameGridDto> GetUserByLookId(int? lookId, List<int> userIds);

        void DeleteBy(int id);

        DataTable GetCompanyLookNotification(DateTime currentDateTime);

        List<LookPlayersDto> GetCompanyLookParticipantNotification(DateTime currentDateTime);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);


    }
}

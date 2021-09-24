using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IPlayService
    {
        PlayDto GetById(int id);

        Task<int> AddUpdateAsync(PlayDto dto);

        PreSessionAgendaDto GetPlayAction(int id);
        void MoveToday(List<int> ids);
        List<PlayDto> GetAll(int companyId, int typeId, int? gId);

        void UpdateStatus(PlayDelegateDto dto);

        Task AddFeedBack(PlayFeedbackDto dto);
        List<PlayFeedbackDto> GetAllFeedBack(int playid);

        List<PlayDto> GetAll(int companyId, int typeId, bool istoday, int? userId, int? gId);

        Task<bool> IsExist(int companyId, string name, int? id, int typeId);

        List<SelectedItemDto> GetAllParentDrop(int companyId, int typeId, int? gId);

        Task Delete(int id);

        Task Approve(int id);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);

        List<PlayGridDto> GetPlayList(bool isToday, int typeId, int? GameId, int CompanyId, int? userId);

    }
}

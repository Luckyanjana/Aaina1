using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IStatusService
    {
        Task<StatusDto> GetById(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(StatusDto dto);

        List<StatusDto> GetAllByGameId(int companyId, int gameId, int? userId);

        List<StatusDto> GetAllByCompanyId(int companyId,int? userId,int? gameId);

        Task<StatusFeedbackDto> GetFeedbackDetailsById(int id);

        void DeleteById(int id);
        List<StatusFeedbackReminderDto> GetStatusParticipantReminderEventEmail(DateTime currentDateTime, int reminterType);
        Task<int> AddFeedbackAsync(StatusFeedbackPostDto dto);

        Task<int> AddNonStatusAsync(NonStatusDto dto);

        Task<List<SelectedItemDto>> GetParticipentByStatusId(int statusId);
        Task<List<StatusFeedbackDisplayDto>> ViewResult(int id, int? userId);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);
    }
}

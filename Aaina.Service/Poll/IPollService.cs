using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
  public  interface IPollService
    {
        List<PollDto> GetAllByCompanyId(int companyId);

        List<PollDto> GetAllByCompanyId(int companyId, int? userid,int gameId);

        List<PollDto> GetAllByGameId(int companyId, int gameId);

        Task<PollDto> GetById(int id);

        Task<List<SelectedItemDto>> GetParticipantByPollId(int id);
        Task<PollDto> GetDetailsId(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(PollDto dto);

        void DeleteBy(int id);

        Task<PollFeedbackDisplayDto> GetFeedbackById(int id);
        Task<PollFeedbackDisplayDto> ViewResult(int id, int? userId);

        void AddFeedback(PollFeedbackAddDto dto);
        List<StatusFeedbackReminderDto> GetPollReminderMailNotification(DateTime currentDateTime, int reminterType);

    }
}

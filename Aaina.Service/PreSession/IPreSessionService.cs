using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
   public interface IPreSessionService
    {
        Task<int> AddUpdateAsync(PreSessionDto dto);

        Task<PreSessionDto> GetById(int id, DateTime start, DateTime end,int userId);

        List<PreSessionAgendaDto> GetPlayAction(int id, DateTime start, DateTime end, int userId, int gameId);

        Task<List<PreSessionAgendaListDto>> GetList(int id, DateTime start, DateTime end,int userid);

        bool Approve(int id);

        bool DisApprove(int id);

        bool PreSessionUpdateStatus(int companyId, int sessionId, DateTime startDate, DateTime endDate);

        bool UpdateStatus(int companyId, int sessionId, DateTime startDate, DateTime endDate, int statusId, int userId, DateTime? reScheduleDate);

        bool UpdateDelegate(int sessionId, int userId, int delegateId);

        bool Delete(int id);
    }
}

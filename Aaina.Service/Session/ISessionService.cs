using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface ISessionService
    {
        Task<SessionDto> GetById(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(SessionDto dto);

        List<SessionDto> GetAllByGameId(int companyId, int gameId);
        List<SessionDto> GetAllByGameId(int companyId,int? userId, int gameId);
       List<SessionDto> GetAllByCompanyIdAsync(int companyId);
        List<SessionDto> GetAllByCompanyId(int companyId,int? userId,int gameId);
        void DeleteById(int id);
        List<SessionScheduleEventDto> GetCompanySesstionEvent(int companyId, int? gameId, DateTime from, DateTime to);

        List<SessionScheduleEventDto> GetCompanySesstionEvent(int companyId, int? userId, int? gameId, DateTime from, DateTime to);


        DataTable GetCompanySessionReminderEvent(DateTime currentDateTime,int reminterType);

        List<StatusFeedbackReminderDto> GetSessionParticipantReminderEvent(DateTime currentDateTime,int reminterType);

        SessionEventDetails GetSesstionEventDetails(int Id, DateTime from, DateTime to,int? userId);
        Task<SessionDto> GetSessionByUserId(int sessionId, int userId);
        SessionParticipant GetSessionParticipantByUserId(int sessionId, int userId);

        List<SessionParticipant> GetSessionDecisionMaker(int sessionId);
        List<SessionParticipant> GetAllDecisionParticipant(int sessionId);

        List<SessionParticipant> GetSessionParticipantNonMake(int sessionId);

        bool IsDecisionMaker(int userId, int sessionId);
        bool IsCoordinator(int userId, int sessionId);
        void UpdateSessionParticipant(SessionParticipant entity);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);
    }
}

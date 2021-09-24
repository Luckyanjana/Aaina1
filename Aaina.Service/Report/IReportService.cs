using Aaina.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Service
{
    public interface IReportService
    {
        List<ReportDto> GetAllByCompanyId(int companyId, int? userId,int? gameId);
        List<ReportDto> GetAllByGameId(int companyId, int gameId);

        Task<ReportDto> GetById(int id);

        Task<ReportDto> GetDetailsId(int id);

        Task<bool> IsExist(int companyId, string name, int? id);

        Task<int> AddUpdateAsync(ReportDto dto);

        void DeleteBy(int id);

        Task<ReportGiveDto> GetGiveByReportId(int id);

        Task<ReportGiveSaveDto> GetGiveUpdateByReportId(int id);

        Task<List<ReportGiveSaveDto>> GetGiveUpdateByReportRange(int id, DateTime start, DateTime end);

        Task<List<SelectedItemDto>> GetEntityByReportId(int id);
        

        Task<bool> SaveReortGive(ReportGiveSaveDto model);

        Task<bool> UpdateReortGive(ReportGiveSaveDto model);



        List<StatusFeedbackReminderDto> GetReportTemplateReminderMailNotification(DateTime currentDateTime, int reminterType);
        List<SessionScheduleEventDto> GetCompanyReportEvent(int companyId, int? userId, int? gameId, DateTime from, DateTime to);
        Task<ReportGiveResultDto> GetGiveListByReportId(int id);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);
    }
}

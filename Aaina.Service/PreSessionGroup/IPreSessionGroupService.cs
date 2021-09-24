using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IPreSessionGroupService
    {
        Task<int> AddPreSessionGroupAsync(PreSessionGroupDto dto);

        PreSessionGroupDto GetBySessionId(int sessionId, DateTime startDate, DateTime endDate);

        PreSessionGroupDto GetById(int id);

        PreSessionGroupDto GetByIdWithGame(int id);

        PreSessionGroupDto CompleteAndGet(int id);
        int AddGroupChatAsync(PreSessionGroupDetailDto dto);

        List<PreSessionGroupDetailDto> GetChatList(int PresessionId);

        List<SelectedItemDto> GetPresessionProupId(int userid);
    }
}

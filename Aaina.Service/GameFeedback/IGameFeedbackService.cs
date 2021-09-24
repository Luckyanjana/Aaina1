using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IGameFeedbackService
    {
        Task<int> AddUpdateGameAsync(GameFeedbackDto dto);

        Task<int> AddUpdateTeamAsync(GameFeedbackDto dto);

        Task<int> AddUpdateUserAsync(GameFeedbackDto dto);

        Task<GameFeedbackDto> GetGameByLookId(int id, List<int> gameid, int userId);

        Task<GameFeedbackDto> GetTeamByLookId(int id, List<int> teamId, int userId);

        Task<GameFeedbackDto> GetUserByLookId(int id, List<int> userId, int selfId);

        Task<bool> IsDraftExist(int lookId,int userId);

        Task<bool> IsDraftExistTeam(int lookId, int userId);

        Task<bool> IsDraftExistUser(int lookId, int userId);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aaina.Dto;

namespace Aaina.Service
{
    public interface IPostSessionService
    {
        Task<int> AddUpdateAsync(PostSessionDto dto);

        Task<PostSessionDto> GetByForPostId(int id, DateTime start, DateTime end, int userId);
        Task<PostSessionDto> GetById(int id, DateTime start, DateTime end, int userId);
    }
}

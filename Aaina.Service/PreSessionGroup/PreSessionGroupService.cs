using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Service
{
    public class PreSessionGroupService : IPreSessionGroupService
    {
        private IRepository<PreSessionGroup, int> repoPreSessionGroup;
        private IRepository<UserLogin, int> userLoginRepo;
        private IRepository<PreSessionGroupDetails, int> repoPreSessionGroupDetail;
        private IRepository<Session, int> repoSession;

        public PreSessionGroupService(IRepository<PreSessionGroup, int> repoPreSessionGroup, IRepository<PreSessionGroupDetails, int> repoPreSessionGroupDetail,
            IRepository<Session, int> repoSession, IRepository<UserLogin, int> userLoginRepo)
        {
            this.repoPreSessionGroup = repoPreSessionGroup;
            this.repoPreSessionGroupDetail = repoPreSessionGroupDetail;
            this.repoSession = repoSession;
            this.userLoginRepo = userLoginRepo;
        }

        public async Task<int> AddPreSessionGroupAsync(PreSessionGroupDto dto)
        {
            var preSession = repoPreSessionGroup.FirstOrDefault(x => x.SessionId == dto.SessionId && x.StartDate == dto.StartDate && x.EndDate == dto.EndDate);
            if (preSession == null)
            {
                var session = repoSession.Get(dto.SessionId);
                var lookInfo = new PreSessionGroup()
                {
                    SessionId = dto.SessionId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    GroupName = $"{session.Name}_{dto.StartDate.ToString("dd/MM/yyyy")}_{dto.EndDate.ToString("dd/MM/yyyy")}",
                    IsComplete = false
                };

                await repoPreSessionGroup.InsertAsync(lookInfo);
                return lookInfo.Id;
            }
            else
            {
                return preSession.Id;
            }
        }

        public PreSessionGroupDto GetBySessionId(int sessionId, DateTime startDate, DateTime endDate)
        {
            var session = repoPreSessionGroup.FirstOrDefault(x => x.SessionId == sessionId && x.StartDate == startDate && x.EndDate == endDate);

            return session != null ? new PreSessionGroupDto()
            {
                EndDate = session.EndDate,
                GroupName = session.GroupName,
                IsComplete = session.IsComplete,
                SessionId = session.SessionId,
                StartDate = session.StartDate,
                Id = session.Id
            } : new PreSessionGroupDto();
        }

        public PreSessionGroupDto GetById(int id)
        {
            var session = repoPreSessionGroup.Get(id);

            return session != null ? new PreSessionGroupDto()
            {
                EndDate = session.EndDate,
                GroupName = session.GroupName,
                IsComplete = session.IsComplete,
                SessionId = session.SessionId,
                StartDate = session.StartDate,
                Id = session.Id
            } : new PreSessionGroupDto();
        }
        public PreSessionGroupDto GetByIdWithGame(int id)
        {
            var session = repoPreSessionGroup.GetIncludingById(x => x.Id == id, x => x.Include(m => m.Session));

            return session != null ? new PreSessionGroupDto()
            {
                EndDate = session.EndDate,
                GameId = session.Session.GameId.Value,
                GroupName = session.GroupName,
                IsComplete = session.IsComplete,
                SessionId = session.SessionId,
                StartDate = session.StartDate,
                Id = session.Id
            } : new PreSessionGroupDto();
        }

        public PreSessionGroupDto CompleteAndGet(int id)
        {
            var session = repoPreSessionGroup.Get(id);
            session.IsComplete = true;
            repoPreSessionGroup.Update(session);
            return session != null ? new PreSessionGroupDto()
            {
                EndDate = session.EndDate,
                GroupName = session.GroupName,
                IsComplete = session.IsComplete,
                SessionId = session.SessionId,
                StartDate = session.StartDate,
                Id = session.Id
            } : new PreSessionGroupDto();
        }

        public int AddGroupChatAsync(PreSessionGroupDetailDto dto)
        {


            var lookInfo = new PreSessionGroupDetails()
            {
                Message = dto.Message,
                PreSessionGroupId = dto.PreSessionGroupId,
                SendDate = DateTime.Now,
                UserId = dto.UserId
            };

            repoPreSessionGroupDetail.Insert(lookInfo);
            return lookInfo.Id;
        }

        public List<PreSessionGroupDetailDto> GetChatList(int PresessionId)
        {
            return repoPreSessionGroupDetail.GetAllIncluding(x => x.PreSessionGroupId == PresessionId, x => x.Include(m => m.User)).Select(x => new PreSessionGroupDetailDto()
            {
                Message = x.Message,
                UserName = x.User.Fname + " " + x.User.Lname,
                ImagePath = !string.IsNullOrEmpty(x.User.AvatarUrl) ? $"/DYF/{x.User.CompanyId}/EmployeeImages/{x.User.AvatarUrl}" : "/img/Default_avatar.jpg",
                PreSessionGroupId = x.Id,
                SendDate = x.SendDate,
                UserId = x.UserId
            }).ToList();

        }

        public List<SelectedItemDto> GetPresessionProupId(int userid)
        {
            var list = repoPreSessionGroup.GetAllList(x => !x.IsComplete && x.Session.SessionParticipant.Any(x => x.UserId == userid) && x.StartDate <= DateTime.Now &&
            x.EndDate >= DateTime.Now).Select(x => new SelectedItemDto()
            {
                Id = x.Id.ToString(),
                Name = x.GroupName,
                Additional = x.SessionId.ToString()
            }).ToList();

            //var list = repoPreSessionGroup.GetAllList(x => !x.IsComplete && x.Session.SessionParticipant.Any(x => x.UserId == userid)).Select(x => new SelectedItemDto()
            //{
            //    Id = x.Id.ToString(),
            //    Name = x.GroupName,
            //    Additional = x.SessionId.ToString()
            //}).ToList();
            return list;

        }

    }
}


using System.Collections.Generic;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Aaina.Dto;

namespace Aaina.Service
{
    public  interface IUserLoginService
    {
        Task<UserLogin> GetByUserNameOrEmail(string userName);

        Task<UserLogin> GetById(int id);
        UserLogin GetByIdAsync(int id);

        Task<bool> UserNameExist(string userName, int? id);

        Task<int> VerifyEmailAddres(int id);

        Task<bool> EmailExist(string email, int? id);

        Task<bool> BasicDetailsUpdate(int id);

        Task<int> Register(RegisterDto requestDto, string saltKey,int companyId);

        Task<int> UpdateRegister(ProfileDto requestDto);

        void InsertPasswordReset(int id, string key);

        Task<UserLogin> GetPasswordResetByLink(string passwordResetLink);

        Task<int> AddUpdateAsync(UserLoginDto requestDto, string saltKey);

        Task<int> AddUpdateUserProfileAsync(UserProfileDto dto);
        UserProfileDto GetByUserProfileId(int id);

        Task UpdateAsync(UserLogin dbDto);
        List<UserLoginDto> GetAllUsers();
        Task ActiveDeactiveByIdAsync(int id);

        Task UnlockByIdAsync(int id);

        UserLoginDto GetByUserId(int id);
        List<UserLoginDto> GetAllUsersWithoutAdmin();
        List<UserLoginDto> GetByCompanyyId(int companyId);

        List<UserLoginDto> GetAllByCompanyyId(int companyId);

        List<UserLoginDto> GetAllByCompanyyId(int companyId, int gameId);

        List<SelectedItemDto> GetAllByDropByCompanyId(int companyId);

        List<UserLoginDto> GetAllAdminByCompanyyId();

        void DeleteByUserId(int id);
        List<SelectedItemDto> GetAllDrop(int companyId, int? userId);
       // List<SelectedItemDto> UserSearchByName(string userName);
        List<SelectedItemDto> GetTeamPlayers(int companyId);
        List<SelectedItemDto> GetConnectedPlayers(int[] playerIds);

        void UpdateDriveId(int companyId, string driveId);
        List<UserLoginDto> GetUserByCompany(int companyId);

        Task<GridResult> GetPaggedListAsync(GridParameterModel parameters);

    }


}

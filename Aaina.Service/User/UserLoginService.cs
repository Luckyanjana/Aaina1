using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Service
{

    public class UserLoginService : IUserLoginService
    {
        private IRepository<UserLogin, int> repoUser;
        private IRepository<UserProfile, int> repoUserProfile;

        public UserLoginService(IRepository<UserLogin, int> repoUser, IRepository<UserProfile, int> repoUserProfile)
        {
            this.repoUser = repoUser;
            this.repoUserProfile = repoUserProfile;
        }

        public async Task<UserLogin> GetByUserNameOrEmail(string userName)
        {
            var result = await this.repoUser.FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower() || x.Email.ToLower() == userName.ToLower());
            return result;
        }

        public async Task<UserLogin> GetByUserName(string userName)
        {
            var result = await this.repoUser.FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
            return result;
        }
        public async Task<UserLogin> GetById(int id)
        {
            var result = await this.repoUser.GetAsync(id);
            return result;
        }

        public UserLogin GetByIdAsync(int id)
        {
            var result = this.repoUser.Get(id);
            return result;
        }


        public async Task<bool> UserNameExist(string userName, int? id)
        {
            var result = await this.repoUser.CountAsync(x => x.Id != id && x.UserName.ToLower() == userName.ToLower());
            return result > 0;
        }

        public async Task<int> VerifyEmailAddres(int id)
        {
            int response = 0;

            var result = this.repoUser.Get(id);
            if (result != null && !result.IsEmailVerify)
            {
                result.IsEmailVerify = true;
                await this.repoUser.UpdateAsync(result);
                response = 1;
            }
            else
            {
                response = result == null ? -1 : 0;
            }
            return response;
        }

        public async Task<bool> EmailExist(string email, int? id)
        {
            var result = await this.repoUser.CountAsync(x => x.Id != id && x.Email == email);
            return result > 0;
        }

        public async Task<bool> BasicDetailsUpdate(int id)
        {
            var result = await this.repoUser.CountAsync(x => x.Id == id && !string.IsNullOrEmpty(x.Email));
            return result > 0;
        }

        public async Task<int> Register(RegisterDto requestDto, string saltKey, int companyId)
        {

            var userInfo = new UserLogin()
            {
                Fname = requestDto.Fname,
                Lname = requestDto.Lname,
                Mname = requestDto.Mname,
                Email = requestDto.Email,
                UserName = requestDto.UserName,
                SaltKey = saltKey,
                Password = requestDto.Password,
                UserType = requestDto.UserType.Value,
                CompanyId = companyId,
                IsActive = true,
                ModifiedDate = DateTime.Now,
                AddedDate = DateTime.Now,
                PlayerType = requestDto.PlayerType > 0 ? requestDto.PlayerType : (int)PlayersType.Manpower
            };
            await repoUser.InsertAsync(userInfo);
            return userInfo.Id;
        }

        public async Task<int> UpdateRegister(ProfileDto requestDto)
        {
            var user = repoUser.Get(requestDto.Id);
            user.Fname = requestDto.Fname;
            user.Lname = requestDto.Lname;
            user.Mname = requestDto.Mname;
            user.AvatarUrl = requestDto.AvatarUrl;
            user.Address = requestDto.Address;
            user.City = requestDto.City;
            user.Dob = requestDto.Dob;
            user.Gender = requestDto.Gender;
            user.MobileNo = requestDto.MobileNo;
            user.State = requestDto.State;
            user.ModifiedDate = DateTime.Now;
            await repoUser.UpdateAsync(user);
            return requestDto.Id;
        }

        public void InsertPasswordReset(int id, string key)
        {
            var user = repoUser.Get(id);
            user.PasswordResetLink = key;
            user.IsForgotVerified = false;
            user.LinkExpiredDate = DateTime.Now.AddDays(1);
            repoUser.Update(user);

        }

        public async Task<UserLogin> GetPasswordResetByLink(string passwordResetLink)
        {
            var result = await this.repoUser.FirstOrDefaultAsync(x => x.PasswordResetLink == passwordResetLink);
            return result;
        }


        public async Task<int> AddUpdateAsync(UserLoginDto requestDto, string saltKey)
        {
            if (requestDto.Id > 0)
            {
                var user = repoUser.Get(requestDto.Id);
                user.Fname = requestDto.Fname;
                user.Lname = requestDto.Lname;
                user.Mname = requestDto.Mname;
                user.AvatarUrl = requestDto.AvatarUrl;
                user.Address = requestDto.Address;
                user.City = requestDto.City;
                user.Dob = requestDto.Dob;
                user.Gender = requestDto.Gender;
                user.MobileNo = requestDto.MobileNo;
                user.State = requestDto.State;
                user.ModifiedDate = DateTime.Now;
                await repoUser.UpdateAsync(user);
            }
            else
            {

                var userInfo = new UserLogin()
                {
                    Fname = requestDto.Fname,
                    Mname = requestDto.Mname,
                    Lname = requestDto.Lname,
                    Email = requestDto.Email,
                    AvatarUrl = requestDto.AvatarUrl,
                    CompanyId = requestDto.CompanyId,
                    Password = requestDto.Password,
                    SaltKey = requestDto.SaltKey,
                    UserType = requestDto.UserType,
                    Address = requestDto.Address,
                    City = requestDto.City,
                    Dob = requestDto.Dob,
                    Gender = requestDto.Gender,
                    MobileNo = requestDto.MobileNo,
                    IsActive = true,
                    State = requestDto.State,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    PlayerType = (int)PlayersType.Manpower
                };
                await repoUser.InsertAsync(userInfo);
                requestDto.Id = userInfo.Id;
            }

            return requestDto.Id;
        }

        public async Task UpdateAsync(UserLogin dbDto)
        {

            await repoUser.UpdateAsync(dbDto);
        }

        public async Task ActiveDeactiveByIdAsync(int id)
        {
            var user = repoUser.Get(id);
            user.IsActive = !user.IsActive;
            await repoUser.UpdateAsync(user);
        }

        public async Task UnlockByIdAsync(int id)
        {
            var user = repoUser.Get(id);
            //user.IsLocked = false;
            //user.InvalidAttempt = 0;
            //user.LockedDateTime = null;
            await repoUser.UpdateAsync(user);
        }

        public UserLoginDto GetByUserId(int id)
        {
            UserLoginDto dto = new UserLoginDto();
            var user = repoUser.Get(id);
            dto = new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                Password = user.Password,
                SaltKey = user.SaltKey,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo
            };
            return dto;
        }

        public List<UserLoginDto> GetByCompanyyId(int companyId)
        {

            return repoUser.GetAllList(x => x.CompanyId == companyId && x.UserType == (int)UserType.User).Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                PlayerType = user.PlayerType
            }).ToList();
        }

        public List<UserLoginDto> GetAllByCompanyyId(int companyId)
        {
            var userAll = repoUser.GetAllIncluding(x => x.CompanyId == companyId && x.UserType == (int)UserType.User, x => x.Include(m => m.UserProfile));
            return userAll.Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                PlayerType = user.PlayerType,
                Doj = user.UserProfile != null && user.UserProfile.Joining.HasValue ? user.UserProfile.Joining : (DateTime?)null
            }).ToList();
        }

        public List<UserLoginDto> GetAllByCompanyyId(int companyId, int gameId)
        {
            var userAll = repoUser.GetAllIncluding(x => x.CompanyId == companyId && x.GamePlayer.Any(a => a.GameId == gameId) && x.UserType == (int)UserType.User, x => x.Include(m => m.UserProfile));
            return userAll.Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                PlayerType = user.PlayerType,
                Doj = user.UserProfile != null && user.UserProfile.Joining.HasValue ? user.UserProfile.Joining : (DateTime?)null
            }).ToList();
        }

        public List<SelectedItemDto> GetAllByDropByCompanyId(int companyId)
        {
            return repoUser.GetAllIncluding(x => x.CompanyId == companyId && x.UserType == (int)UserType.User, x => x.Include(m => m.GamePlayer).Include(m => m.TeamPlayer)).Select(user => new SelectedItemDto()
            {
                Name = user.Fname + " " + user.Lname,
                Additional = (user.GamePlayer.Any() ? "Game" : " ") + (user.TeamPlayer.Any() ? (user.GamePlayer.Any() ? ", " : "") + "Team" : ""),
                Id = user.Id.ToString(),
            }).ToList();
        }

        public List<SelectedItemDto> GetAllDrop(int companyId, int? userId)
        {
            return repoUser.GetAll(x => x.CompanyId == companyId && (userId.HasValue ? x.Id != userId : true) && x.UserType == (int)UserType.User && x.IsActive).Select(user => new SelectedItemDto()
            {
                Name = user.Fname + " " + user.Lname,
                Id = user.Id.ToString(),
            }).ToList();
        }

        //public List<SelectedItemDto> UserSearchByName(string userName)
        //{
        //    return repoUser.GetAll(x => x.UserName.ToLower().StartsWith(userName.ToLower())).Select(user => new SelectedItemDto()
        //    {
        //        Id = user.Id.ToString(),
        //        Name = user.Fname + " " + user.Lname,
        //        Image = SiteKeys.ImageUrlDomain + (!string.IsNullOrEmpty(user.AvatarUrl) ? user.CompanyId + "/EmployeeImages/" + user.AvatarUrl : "img/avatar.png"),
        //        IsImage = true
        //    }).ToList();
        //}
        public List<SelectedItemDto> GetTeamPlayers(int companyId)
        {
            return repoUser.GetAll(x => x.CompanyId == companyId && x.UserType == (int)UserType.User && x.IsActive).Select(user => new SelectedItemDto()
            {
                Id = user.Id.ToString(),
                Name = user.Fname + " " + user.Lname,
                Image = SiteKeys.ImageUrlDomain + (!string.IsNullOrEmpty(user.AvatarUrl) ? user.CompanyId + "/EmployeeImages/" + user.AvatarUrl : "img/avatar.png"),
                IsImage = true
            }).ToList();
        }
        public List<SelectedItemDto> GetConnectedPlayers(int[] playerIds)
        {
            return repoUser.GetAll(x => playerIds.Contains(x.Id)).Select(user => new SelectedItemDto()
            {
                Name = user.Fname + " " + user.Lname,
            }).ToList();
        }


        public List<UserLoginDto> GetAllAdminByCompanyyId()
        {

            return repoUser.GetAllIncluding(x => x.UserType == (int)UserType.Admin, x => x.Include(m => m.Company)).Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                Company = user.Company.Name,
                Password = user.Password,
                SaltKey = user.SaltKey,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo
            }).ToList();
        }

        public void DeleteByUserId(int id)
        {
            repoUser.Delete(id);
        }

        public UserProfileDto GetByUserProfileId(int id)
        {
            UserProfileDto dto = new UserProfileDto();
            var user = repoUser.GetIncludingById(x => x.Id == id, x => x.Include(m => m.UserProfile));
            var profile = user.UserProfile != null ? user.UserProfile : new UserProfile();
            dto = new UserProfileDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                PlayerType = user.PlayerType,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                EduCert = profile.EduCert,
                ExpCert = profile.ExpCert,
                FatherMobileNo = profile.FatherMobileNo,
                FatherName = profile.FatherName,
                GuardianMobileNo = profile.GuardianMobileNo,
                GuardianName = profile.GuardianName,
                IdProffFile = profile.IdProffFile,
                IdProofType = profile.IdProofType,
                Joining = profile.Joining,
                MotherMobileNo = profile.MotherMobileNo,
                MotherName = profile.MotherName,
                Other = profile.Other,
                PoliceVerification = profile.PoliceVerification

            };
            return dto;
        }

        public async Task<int> AddUpdateUserProfileAsync(UserProfileDto dto)
        {
            if (dto.Id.HasValue)
            {
                var user = repoUser.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.UserProfile));
                var profile = user.UserProfile;
                user.Fname = dto.Fname;
                user.Mname = dto.Mname;
                user.Lname = dto.Lname;
                user.Address = dto.Address;
                user.AvatarUrl = dto.AvatarUrl;
                user.City = dto.City;
                user.Dob = dto.Dob;
                user.Email = dto.Email;
                user.UserName = dto.UserName;
                user.Gender = dto.Gender;
                user.IsActive = dto.IsActive;
                user.MobileNo = dto.MobileNo;
                user.PlayerType = dto.PlayerType;
                if (!string.IsNullOrEmpty(dto.Password) && !string.IsNullOrEmpty(dto.SaltKey))
                {
                    user.Password = dto.Password;
                    user.SaltKey = dto.SaltKey;
                }
                user.State = dto.State;
                user.ModifiedDate = DateTime.Now;
                repoUser.Update(user);

                profile = profile == null ? new UserProfile() : profile;
                profile.EduCert = dto.EduCert;

                profile.ExpCert = dto.ExpCert;
                profile.FatherMobileNo = dto.FatherMobileNo;
                profile.FatherName = dto.FatherName;
                profile.GuardianMobileNo = dto.GuardianMobileNo;
                profile.GuardianName = dto.GuardianName;
                profile.IdProffFile = dto.IdProffFile;
                profile.IdProofType = dto.IdProofType;
                profile.Joining = dto.Joining;
                profile.MotherMobileNo = dto.MotherMobileNo;
                profile.MotherName = dto.MotherName;
                profile.Other = dto.Other;
                profile.PoliceVerification = dto.PoliceVerification;
                if (profile.UserId > 0)
                {
                    await repoUserProfile.UpdateAsync(profile);
                }
                else
                {
                    profile.UserId = user.Id;
                    await repoUserProfile.InsertAsync(profile);
                }
                return dto.Id.Value;
            }
            else
            {
                var userInfo = new UserLogin()
                {
                    CompanyId = dto.CompanyId,
                    Fname = dto.Fname,
                    Mname = dto.Mname,
                    Lname = dto.Lname,
                    Address = dto.Address,
                    AvatarUrl = dto.AvatarUrl,
                    City = dto.City,
                    Dob = dto.Dob,
                    Email = dto.Email,
                    UserName = dto.UserName,
                    Gender = dto.Gender,
                    IsActive = dto.IsActive,
                    MobileNo = dto.MobileNo,
                    Password = dto.Password,
                    SaltKey = dto.SaltKey,
                    State = dto.State,
                    UserType = dto.UserType.Value,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    PlayerType = dto.PlayerType,
                    UserProfile = new UserProfile()
                    {
                        EduCert = dto.EduCert,
                        ExpCert = dto.ExpCert,

                        FatherMobileNo = dto.FatherMobileNo,
                        FatherName = dto.FatherName,
                        GuardianMobileNo = dto.GuardianMobileNo,
                        GuardianName = dto.GuardianName,
                        IdProffFile = dto.IdProffFile,
                        IdProofType = dto.IdProofType,
                        Joining = dto.Joining,
                        MotherMobileNo = dto.MotherMobileNo,
                        MotherName = dto.MotherName,
                        Other = dto.Other,
                        PoliceVerification = dto.PoliceVerification
                    }
                };
                await repoUser.InsertAsync(userInfo);
                return userInfo.Id;
            }

        }

        public List<UserLoginDto> GetAllUsers()
        {
            return repoUser.GetAll().Where(x => x.UserType != (int)Aaina.Common.UserType.SuperAdmin).Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                PlayerType = user.PlayerType,
                Doj = user.UserProfile != null && user.UserProfile.Joining.HasValue ? user.UserProfile.Joining : (DateTime?)null
            }).ToList();
        }

        public List<UserLoginDto> GetAllUsersWithoutAdmin()
        {
            return repoUser.GetAll().Where(x => x.UserType != (int)Aaina.Common.UserType.Admin && x.UserType != (int)Aaina.Common.UserType.SuperAdmin).Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                PlayerType = user.PlayerType,
                Doj = user.UserProfile != null && user.UserProfile.Joining.HasValue ? user.UserProfile.Joining : (DateTime?)null
            }).ToList();
        }

        public void UpdateDriveId(int companyId, string driveId)
        {
            var user = repoUser.Get(companyId);
            user.DriveId = driveId;
            repoUser.Update(user);
        }

        public List<UserLoginDto> GetUserByCompany(int companyId)
        {
            var userAll = repoUser.GetAllIncluding(x => x.CompanyId == companyId && x.IsActive, x => x.Include(m => m.UserProfile));
            return userAll.Select(user => new UserLoginDto()
            {
                Fname = user.Fname,
                Mname = user.Mname,
                Lname = user.Lname,
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                CompanyId = user.CompanyId,
                UserType = user.UserType,
                Id = user.Id,
                State = user.State,
                Address = user.Address,
                City = user.City,
                Dob = user.Dob,
                Gender = user.Gender,
                MobileNo = user.MobileNo,
                IsActive = user.IsActive,
                PlayerType = user.PlayerType,
                Doj = user.UserProfile != null && user.UserProfile.Joining.HasValue ? user.UserProfile.Joining : (DateTime?)null
            }).ToList();
        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {
            IQueryable<UserLogin> query = repoUser.GetAll().Where(x => x.CompanyId == parameters.CompanyId && x.GamePlayer.Any(a=>a.GameId==parameters.GameId) && x.UserType == (int)UserType.User).OrderBy(y => y.Fname);
            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<UserGridDto> data = new List<UserGridDto>();
            foreach (UserLogin item in result.Data)
            {
                data.Add(new UserGridDto()
                {
                    Id = item.Id,
                    Fname = item.Fname,
                    Lname = item.Lname,
                    UserName = item.UserName,
                    PlayerType = ((PlayersType)item.PlayerType).ToString(),
                    IsActive = item.IsActive ? "Active" : "InActive"
                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}

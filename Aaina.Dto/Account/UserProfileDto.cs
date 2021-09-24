using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class UserProfileDto : RegisterDto
    {
        public UserProfileDto()
        {
            this.AllForChart = new List<GameMenuDto>();
            this.AllGame = new List<GameGridDto>();
            this.RoleList = new List<SelectedItemDto>();
            this.GameRoleList = new List<UserGameRole>();
            this.PlayerTypeList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int PlayerType { get; set; }
        public string Mname { get; set; }
        public string SaltKey { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Dob { get; set; }
        public int? Gender { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime? Joining { get; set; }
        public string FatherName { get; set; }
        public string FatherMobileNo { get; set; }
        public string MotherName { get; set; }
        public string MotherMobileNo { get; set; }
        public string GuardianName { get; set; }
        public string GuardianMobileNo { get; set; }
        public int? IdProofType { get; set; }
        public string IdProffFile { get; set; }
        public string EduCert { get; set; }
        public string ExpCert { get; set; }
        public string PoliceVerification { get; set; }
        public string Other { get; set; }
        public List<GameMenuDto> AllForChart { get; set; }
        public List<SelectedItemDto> RoleList { get; set; }
        public List<SelectedItemDto> PlayerTypeList { get; set; }
        public List<GameGridDto> AllGame { get; set; }

        public List<UserGameRole> GameRoleList { get; set; }
    }

    public class UserGameRole
    {
        public int GameId { get; set; }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public bool IsAdded { get; set; }
    }
}

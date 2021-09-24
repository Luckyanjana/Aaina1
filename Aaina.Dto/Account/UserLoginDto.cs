using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class UserLoginDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public int UserType { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsEmailVerify { get; set; }
        public bool IsActive { get; set; }
        public string PasswordResetLink { get; set; }
        public DateTime? LinkExpiredDate { get; set; }
        public bool IsForgotVerified { get; set; }
        public DateTime? Dob { get; set; }
        public string DriveId { get; set; }
        public DateTime? Doj { get; set; }
        public int? Gender { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PlayerType { get; set; }

    }

    public class UserGridDto
    {

        public int Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string PlayerType { get; set; }
        public string UserName { get; set; }
        public string  Doj { get; set; }
        public string IsActive { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? Dob { get; set; }
        public int? Gender { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class UserProfile
    {
        public int UserId { get; set; }
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
        public string DriveIdIdProffFile { get; set; }
        public string DriveIdEduCert { get; set; }
        public string DriveIdExpCert { get; set; }
        public string DriveIdPoliceVerification { get; set; }
        public string DriveIdOther { get; set; }

        public virtual UserLogin User { get; set; }
    }
}

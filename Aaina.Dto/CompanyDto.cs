using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
   public class CompanyDto
    {
        public int Id { get; set; }
        public string DriveId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class CompanyRegisterDto
    {
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class RegisterDto
    {
        public int? UserType { get; set; }
        public int  PlayerType { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Mname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

using Aaina.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.Validation.Account
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("*Required");
            RuleFor(l => l.Password).NotNull().WithMessage("*Required");

        }
    }
}

using Aaina.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.Validation.Account
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Fname).NotEmpty().WithMessage("*Required");
            RuleFor(x => x.Lname).NotEmpty().WithMessage("*Required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("*Required");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("*Required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("*Required");
            RuleFor(l => l.ConfirmPassword).NotNull().WithMessage("*required").Equal(x => x.Password).WithMessage("Confirm Password not matched with Password");

        }
    }
}
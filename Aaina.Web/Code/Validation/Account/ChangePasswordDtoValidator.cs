using Aaina.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.Validation.Account
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("*Required");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("*Required");
            RuleFor(l => l.ConfirmPassword).NotNull().WithMessage("*required").Equal(x => x.NewPassword).WithMessage("Confirm Password not matched with Password");

        }
    }
}

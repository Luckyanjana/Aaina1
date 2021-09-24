using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using FluentValidation;

namespace Aaina.Web.Code.Validation
{
    public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
    {
        public UserProfileDtoValidator()
        {
            RuleFor(x => x.Fname).NotEmpty().WithMessage("First name Required");
            RuleFor(x => x.Lname).NotEmpty().WithMessage("Last name Required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName Required");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("Email Required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password Required");
            RuleFor(l => l.ConfirmPassword).NotNull().WithMessage("ConfirmPassword required").Equal(x => x.Password).WithMessage("Confirm Password not matched with Password");

        }
    }
}

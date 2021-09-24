using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using FluentValidation;

namespace Aaina.Web.Code.Validation
{
    public class LookDtoValidator : AbstractValidator<LookDto>
    {
        public LookDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Required");
            RuleFor(l => l.FromDate).NotEmpty().WithMessage("From Date Required");
            RuleFor(l => l.ToDate).NotEmpty().WithMessage("To Date Required");

        }
    }
}

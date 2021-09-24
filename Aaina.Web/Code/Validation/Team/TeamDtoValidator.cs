using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using FluentValidation;

namespace Aaina.Web.Code.Validation
{
    public class TeamDtoValidator : AbstractValidator<TeamDto>
    {
        public TeamDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Required");
            RuleFor(l => l.Weightage).NotEmpty().WithMessage("Weightage Required");

        }
    }
}

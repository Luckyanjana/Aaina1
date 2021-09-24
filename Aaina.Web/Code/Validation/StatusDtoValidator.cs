using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using FluentValidation;

namespace Aaina.Web.Code.Validation
{
    public class StatusDtoValidator : AbstractValidator<StatusDto>
    {
        public StatusDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Required");
            RuleFor(l => l.GameId).NotEmpty().WithMessage("Game Required");
            RuleFor(l => l.StatusModeId).NotEmpty().WithMessage("Status Mode Required");
        }
    }
}
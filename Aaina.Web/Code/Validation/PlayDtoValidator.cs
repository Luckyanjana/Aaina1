using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using FluentValidation;

namespace Aaina.Web.Code.Validation
{
    public class PlayDtoValidator : AbstractValidator<PlayDto>
    {
        public PlayDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Required");
            RuleFor(l => l.GameId).NotEmpty().WithMessage("Game Required");
            RuleFor(l => l.AccountableId).NotEmpty().WithMessage("Accountable Required");
            RuleFor(l => l.Priority).NotEmpty().WithMessage("Priority Required");
            RuleFor(l => l.Status).NotEmpty().WithMessage("Status Required");
            RuleFor(l => l.AddedOn).NotEmpty().WithMessage("Added On Required");
            RuleFor(l => l.StartDate).NotEmpty().WithMessage("Start date Required");
            RuleFor(l => l.DeadlineDate).NotEmpty().WithMessage("Deadline date Required");
        }
    }
}


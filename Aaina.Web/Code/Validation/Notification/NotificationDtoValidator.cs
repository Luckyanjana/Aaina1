using Aaina.Web.Models.Hubs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.Validation
{
    public class NotificationDtoValidator : AbstractValidator<NotificationModel>
    {
        public NotificationDtoValidator()
        {
            //RuleFor(x => x.PlayerIds).NotEmpty().WithMessage("Player Required");
           // RuleFor(l => l.TeamIds).NotEmpty().WithMessage("Team Required");
            RuleFor(l => l.Region).NotEmpty().WithMessage("Region Required");
            RuleFor(l => l.Message).NotEmpty().WithMessage("Message Required");

        }
    }
}

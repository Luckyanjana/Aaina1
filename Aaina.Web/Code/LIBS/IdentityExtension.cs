using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aaina.Web.Code.LIBS
{
    public static class IdentityExtension
    {
        public static string GetUserProperty(this ClaimsPrincipal user, string claimType)
        {
            if (user.Identity.IsAuthenticated)
            {
                return user.Claims.FirstOrDefault(v => v.Type == claimType)?.Value ?? string.Empty;
            }

            return string.Empty;
        }
    }
}

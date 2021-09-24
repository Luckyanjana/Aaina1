using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Aaina.Web.Code.Attributes
{
    public class CustomAuthorization: AuthorizeAttribute
    {
        private object[] roleTypes;
        public CustomAuthorization(params object[] roleTypes)
        {
            this.roleTypes = roleTypes;
        }

        protected virtual ClaimsPrincipal CurrentUser
        {
            get { return ContextProvider.Current.User; }
        }
    }
}

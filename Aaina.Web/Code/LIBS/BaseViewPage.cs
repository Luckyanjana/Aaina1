using Aaina.Web.Models.Security;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.LIBS
{
    public abstract class BaseViewPage<TModel> : RazorPage<TModel>
    {
        public CustomPrincipal CurrentUser => new CustomPrincipal(ContextProvider.Current.User);
        public CustomMenuPermission CurrentMenuPermission => new CustomMenuPermission(ContextProvider.Current.Items);
    }
    public abstract class BaseViewPage : RazorPage
    {
        public CustomPrincipal CurrentUser => new CustomPrincipal(ContextProvider.Current.User);
        public CustomMenuPermission CurrentMenuPermission => new CustomMenuPermission(ContextProvider.Current.Items);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aaina.Web.Models.Security
{
    public class CustomPrincipal
    {
        private readonly ClaimsPrincipal _principal;

        public CustomPrincipal(ClaimsPrincipal principal)
        {
            _principal = principal;
        }
        public bool IsAuthenticated => (_principal.Identity?.IsAuthenticated) ?? false;
        public int UserId => int.Parse(_principal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value);
        public int CompanyId => int.Parse(_principal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value);        
        public string Name => _principal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
        public string Email => _principal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;
        public int RoleId => int.Parse(_principal.Claims.FirstOrDefault(u => u.Type == "roleId")?.Value);
        public int PlayerType => int.Parse(_principal.Claims.FirstOrDefault(u => u.Type == "playerType")?.Value);
        public string Avatar => _principal.Claims.FirstOrDefault(u => u.Type == "avatar")?.Value;
    }

    public class CustomMenuPermission
    {
        private readonly IDictionary<object, object> _items;

        public CustomMenuPermission(IDictionary<object, object> items)
        {
            _items = items;
        }
        public bool IsList => _items.Any() && _items.ContainsKey("IsList") ? (bool)_items["IsList"] : true;
        public bool IsView => _items.Any() && _items.ContainsKey("IsView") ? (bool)_items["IsView"] : true;
        public bool IsAdd => _items.Any() && _items.ContainsKey("IsAdd") ? (bool)_items["IsAdd"] : true;
        public bool IsEdit => _items.Any() && _items.ContainsKey("IsEdit") ? (bool)_items["IsEdit"] : true;
        public bool IsDelete => _items.Any() && _items.ContainsKey("IsDelete") ? (bool)_items["IsDelete"] : true;
    }


}

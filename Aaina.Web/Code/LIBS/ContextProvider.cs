using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



    public class ContextProvider
    {
        private static Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return _httpContextAccessor.HttpContext;
            }
        }
    }


using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.LIBS
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;        

        public CipherService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input, string Key)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText, string Key)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(cipherText);
        }
    }
}

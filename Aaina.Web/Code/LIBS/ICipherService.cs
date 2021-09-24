using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web.Code.LIBS
{
  public  interface ICipherService
    {
        string Encrypt(string input, string Key);

        string Decrypt(string cipherText, string Key);
    }
}

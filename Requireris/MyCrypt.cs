using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Requireris
{
    public static class MyCrypt
    {
        static byte[] s_aditionalEntropy = { 1, 7, 15, 241, 3, 10, 145, 78, 59, 112, 14, 14 };

        public static byte[] Protect(byte[] data)
        {
            try
            { 
                return ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] UnProtect(byte[] data)
        {
            try
            {
                return ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}

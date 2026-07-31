using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Extensions
{
    public static class SecureStringExtensions
    {
        public static string ConvertToUnsecureString(this SecureString secureString)
        {
            string plainString = string.Empty;

            if (secureString != null)
            {
                IntPtr unmanagedString = IntPtr.Zero;

                try
                {
                    unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                    plainString = Marshal.PtrToStringUni(unmanagedString);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }

            return plainString;
        }
    }
}

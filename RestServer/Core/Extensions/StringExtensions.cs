using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Core.Extensions
{
    using System.Security;

    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string strData)
        {
            if (string.IsNullOrWhiteSpace(strData))
            {
                return null;
            }

            var secureString = new SecureString();
            foreach (var charData in strData.ToCharArray())
            {
                secureString.AppendChar(charData);
            }

            return secureString;
        }
    }
}

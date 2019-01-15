using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Core.Helpers
{
    public static class Utility
    {
        public static string GetCompleteMobileNumber(string isdCode, string mobileNumber)
        {
            return isdCode.Trim() + mobileNumber.Trim();
        }
    }
}

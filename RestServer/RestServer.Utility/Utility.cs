using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Utility
{
    public static class Utility
    {
        public static string FlattenException(this Exception ex)
        {
            if (null == ex.InnerException)
            {
                return ex.Message;
            }
            else
            {
                StringBuilder exceptionMessage = new StringBuilder(ex.Message);
                var innerException = ex.InnerException;
                while (null != innerException)
                {
                    exceptionMessage.Append("---------");
                    exceptionMessage.Append(innerException.Message);
                    innerException = innerException.InnerException;
                }

                return exceptionMessage.ToString();
            }
        }

        public static string GetClassName(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                return Path.GetFileNameWithoutExtension(filePath);
            }

            return filePath;
        }
    }
}

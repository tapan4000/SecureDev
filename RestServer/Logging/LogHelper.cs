using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Logging
{
    public class LogHelper
    {
        public static string CurrentFormattedDateTime => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public static string FlattenException(Exception ex)
        {
            StringBuilder exceptionMessage = new StringBuilder(ex.Message);
            if (ex.InnerException != null)
            {
                exceptionMessage.AppendLine(ex.InnerException.Message);
                if (ex.InnerException.InnerException != null)
                {
                    exceptionMessage.AppendLine(ex.InnerException.InnerException.Message);
                }
            }

            exceptionMessage.AppendLine($"Exception Type: {ex.GetType().FullName}");
            exceptionMessage.AppendLine(ex.StackTrace);
            return exceptionMessage.ToString();
        }
    }
}

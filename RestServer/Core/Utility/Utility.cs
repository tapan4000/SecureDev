namespace RestServer.Core.Utility
{
    using System;
    using System.Text;

    public static class Utility
    {
        public static string FlattenException(this Exception ex)
        {
            if(null == ex.InnerException)
            {
                return ex.Message;
            }
            else
            {
                StringBuilder exceptionMessage = new StringBuilder(ex.Message);
                var innerException = ex.InnerException;
                while(null != innerException)
                {
                    exceptionMessage.Append("---------");
                    exceptionMessage.Append(innerException.Message);
                    innerException = innerException.InnerException;
                }

                return exceptionMessage.ToString();
            }
        }
    }
}

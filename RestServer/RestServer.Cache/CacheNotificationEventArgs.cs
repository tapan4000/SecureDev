using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public sealed class CacheNotificationEventArgs : EventArgs
    {
        private readonly string message;
        private CacheNotificationEventArgs()
        {
        }

        public CacheNotificationEventArgs(string message)
        {
            this.message = message;
        }

        public string Message { get { return this.message; } }
    }
}

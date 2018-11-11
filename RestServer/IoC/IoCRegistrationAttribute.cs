using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class IoCRegistrationAttribute : Attribute
    {
        public IoCRegistrationAttribute(string lifetime):this(null, true, lifetime)
        {
            this.Lifetime = lifetime;
        }

        public IoCRegistrationAttribute(string namePrefix, bool shouldAppendClassName, string lifetime, params string[] interceptions)
        {
            this.NamePrefix = namePrefix;
            this.ShouldAppendClassName = shouldAppendClassName;
            this.Lifetime = lifetime;
            this.Interceptions = interceptions;
        }

        public string Lifetime { get; set; }

        public string NamePrefix { get; set; }

        public bool ShouldAppendClassName { get; internal set; }

        public string[] Interceptions { get; set; }
    }
}

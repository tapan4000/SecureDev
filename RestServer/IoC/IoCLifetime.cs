using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.IoC
{
    public static class IoCLifetime
    {
        public const string Hierarchical = "Hierarchical";

        public const string Transient = "Transient";

        public const string ContainerControlled = "ContainerControlled";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.IoC
{
    public class DependencyParameterOverride
    {
        private string parameterName;

        private object parameterValue;

        public DependencyParameterOverride(string parameterName, object parameterValue)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
        }

        public string ParameterName { get { return this.parameterName; } }

        public object ParameterValue { get { return this.parameterValue; } }
    }
}

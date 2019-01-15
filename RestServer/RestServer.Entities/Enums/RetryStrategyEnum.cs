using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Enums
{
    public enum RetryStrategyEnum
    {
        None = 0,

        Fixed = 1,

        Exponential = 2
    }
}

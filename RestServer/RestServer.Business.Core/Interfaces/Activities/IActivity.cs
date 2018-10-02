using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Activities
{
    public interface IActivity<RequestData, ResponseData> : ITrackable<RequestData, ResponseData>
    {
        // This flag would be determined based on whether the activity is compensatable or not.
        bool IsCompensatable { get; }
    }
}

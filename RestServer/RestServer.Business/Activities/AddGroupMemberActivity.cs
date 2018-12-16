using RestServer.Business.Core.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Activities
{
    public class AddGroupMemberActivity : CompensatableActivityBase<AddGroupRequestData, PopulatedUserBusinessResult>
    {
    }
}

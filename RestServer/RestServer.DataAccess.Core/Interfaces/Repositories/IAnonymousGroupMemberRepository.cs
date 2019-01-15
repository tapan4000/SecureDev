using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface IAnonymousGroupMemberRepository : IRepository<AnonymousGroupMember>
    {
        Task<AnonymousGroupMember> GetExistingAnonymousGroupMemberRecord(string isdCode, string mobileNumber, int groupId);
    }
}

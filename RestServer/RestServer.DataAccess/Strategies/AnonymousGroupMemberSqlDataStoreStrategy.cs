using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;

namespace RestServer.DataAccess.Strategies
{
    public class AnonymousGroupMemberSqlDataStoreStrategy : DataStoreStrategyBase<AnonymousGroupMember>, IAnonymousGroupMemberDataStoreStrategy
    {
        public AnonymousGroupMemberSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<AnonymousGroupMember> GetExistingAnonymousGroupMemberRecord(string isdCode, string mobileNumber, int groupId)
        {
            var existingRecord = (await this.GetData(anonymousGroupMember => anonymousGroupMember.AnonymousUserIsdCode.Equals(isdCode, StringComparison.InvariantCultureIgnoreCase)
                                            && anonymousGroupMember.AnonymousUserMobileNumber.Equals(mobileNumber, StringComparison.InvariantCultureIgnoreCase)
                                            && anonymousGroupMember.GroupId == groupId)).FirstOrDefault();

            return existingRecord;
        }
    }
}

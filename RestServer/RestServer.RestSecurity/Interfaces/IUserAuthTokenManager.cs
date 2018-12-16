using RestServer.RestSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestSecurity.Interfaces
{
    public interface IUserAuthTokenManager
    {
        Task<KeyValuePair<string, DateTime>> GenerateEncodedSignedToken(int userId, string userUniqueId, string applicationUniqueId);

        Task<UserAuthenticationToken> VerifyAndExtractDecodedToken(string encodedSignedToken);
    }
}

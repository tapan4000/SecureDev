using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Interfaces.Managers
{
    public interface IUserManager
    {
        Task<AddUserRequestBusinessResult> AddUser(string isdCode, string mobileNumber, string email, string firstName, string lastName, string passwordHash, string applicationUniqueId);

        Task<BusinessResult> CompleteUserRegistration(int userId, int activationCode);

        Task<BusinessResult> ResendUserRegistrationOtp(int userId);

        Task<LoginUserBusinessResult> LoginUser(string isdCode, string mobileNumber, string passwordHash, string applicationUniqueId);

        Task<LoginUserBusinessResult> LoginUserWithToken(int userId, string applicationUniqueId, string refreshToken, long tokenCreationDateTime);
    }
}

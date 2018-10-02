using RestServer.FrontEndService.ContractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestServer.FrontEndService.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiControllerBase
    {
        [HttpPost]
        [Route]
        public async Task RegisterUser(RegisterUserRequestModel registerRequest)
        {
            await Task.FromResult(true);
        }
    }
}

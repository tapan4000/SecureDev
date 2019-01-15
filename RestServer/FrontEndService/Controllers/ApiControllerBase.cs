using RestServer.Entities.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestServer.FrontEndService.Controllers
{
    public abstract class ApiControllerBase: ApiController
    {
        protected IEventLogger logger;
        protected IUserContext userContext;

        public ApiControllerBase(IEventLogger logger, IUserContext userContext)
        {
            this.logger = logger;
            this.userContext = userContext;
        }
    }
}

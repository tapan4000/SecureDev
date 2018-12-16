using RestServer.Core.Extensions;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace RestServer.FrontEndService.Filters
{
    public class ApplicationValidationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var logger = (IEventLogger)context.Request.GetDependencyScope().GetService(typeof(IEventLogger));
            var workflowContext = (IWorkflowContext)context.Request.GetDependencyScope().GetService(typeof(IWorkflowContext));

            try
            {
                // Generate the workflow Id for a request.
                workflowContext.WorkflowId = Guid.NewGuid().ToString();

                var applicationUniqueId = this.GetHeaderValue(context.Request, "App-Id");
                if (applicationUniqueId.IsEmpty())
                {
                    logger.LogWarning("The application id cannot be empty.");
                    context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    var unitOfWorkFactory = (IUnitOfWorkFactory)context.Request.GetDependencyScope().GetService(typeof(IUnitOfWorkFactory));
                    using (var unitOfWork = unitOfWorkFactory.RestServerUnitOfWork)
                    {
                        var requestingApplication = await unitOfWork.ApplicationRepository.GetApplicationByUniqueId(applicationUniqueId).ConfigureAwait(false);
                        if (null == requestingApplication)
                        {
                            logger.LogWarning($"The application with Id {applicationUniqueId} is not found in the system.");
                            context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
                            return;
                        }
                    }

                    workflowContext.ApplicationUniqueId = applicationUniqueId;
                }

            }
            catch(ArgumentException ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.InternalServerError);
            }

            await this.AuthenticationStepsPostApplicationValidation(context, cancellationToken);
        }

        protected virtual Task AuthenticationStepsPostApplicationValidation(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() => { }, cancellationToken);
        }

        protected string GetHeaderValue(HttpRequestMessage request, string key)
        {
            if(null == request)
            {
                throw new ArgumentException("Request object cannot be null.");
            }

            IEnumerable<string> values = null;
            if(!request.Headers.TryGetValues(key, out values) || !values.Any())
            {
                return null;
            }

            return values.First();
        }
    }
}

using RestServer.Core.Extensions;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.RestSecurity.Interfaces;
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
    public class UserAuthenticationFilter : ApplicationValidationFilter
    {
        protected override async Task AuthenticationStepsPostApplicationValidation(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var logger = (IEventLogger)context.Request.GetDependencyScope().GetService(typeof(IEventLogger));
            var workflowContext = (IWorkflowContext)context.Request.GetDependencyScope().GetService(typeof(IWorkflowContext));
            
            try
            {
                var userAuthToken = this.GetHeaderValue(context.Request, "auth");
                if (userAuthToken.IsEmpty())
                {
                    logger.LogWarning("The auth token cannot be empty.");
                    context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
                    return;
                }

                var authTokenManager = (IUserAuthTokenManager)context.Request.GetDependencyScope().GetService(typeof(IUserAuthTokenManager));
                var decodedAuthToken = await authTokenManager.VerifyAndExtractDecodedToken(userAuthToken);

                if (!decodedAuthToken.ApplicationUniqueId.Equals(workflowContext.ApplicationUniqueId))
                {
                    logger.LogWarning($"The header application id {workflowContext.ApplicationUniqueId} does not match the token application id {decodedAuthToken.ApplicationUniqueId} for user id {decodedAuthToken.UserId}.");
                    context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.Unauthorized);
                    return;
                }

                var unitOfWorkFactory = (IUnitOfWorkFactory)context.Request.GetDependencyScope().GetService(typeof(IUnitOfWorkFactory));
                using (var unitOfWork = unitOfWorkFactory.RestServerUnitOfWork)
                {
                    var expectedUserByUserId = await unitOfWork.UserRepository.GetById(decodedAuthToken.UserId);
                    if(null == expectedUserByUserId)
                    {
                        logger.LogWarning($"The user with Id {decodedAuthToken.UserId} is not found in the system.");
                        context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.Unauthorized);
                        return;
                    }

                    if (!expectedUserByUserId.UserUniqueId.Equals(decodedAuthToken.UserUniqueId))
                    {
                        logger.LogWarning($"The User unique id {decodedAuthToken.UserUniqueId} in token for user id {decodedAuthToken.UserId} does not match the user unique id {expectedUserByUserId.UserUniqueId}.");
                        context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.Unauthorized);
                        return;
                    }
                }

                workflowContext.UserId = decodedAuthToken.UserId;
                workflowContext.UserUniqueId = decodedAuthToken.UserUniqueId;
            }
            catch (ArgumentNullException ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
            }
            catch (ArgumentException ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.BadRequest);
            }
            catch(UnauthorizedAccessException ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                context.ErrorResult = new RestServerHttpActionResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}

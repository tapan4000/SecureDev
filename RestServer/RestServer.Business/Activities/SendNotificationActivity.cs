using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Notification.Interfaces;
using RestServer.Entities.Enums;
using RestServer.Business.Models;
using RestServer.Core.Extensions;

namespace RestServer.Business.Activities
{
    public class SendNotificationActivity : ActivityBase<SendNotificationActivityData, RestrictedBusinessResultBase>
    {
        private INotificationHandler notificationHandler;

        public SendNotificationActivity(IEventLogger logger, INotificationHandler notificationHandler) : base(logger)
        {
            this.notificationHandler = notificationHandler;
        }

        protected async override Task<RestrictedBusinessResultBase> ExecuteAsync(SendNotificationActivityData requestData)
        {
            this.CanIgnoreTrackableFailure = !requestData.CanNotificationFailureCauseFlowFailure;
            var response = new RestrictedBusinessResultBase();
            switch (requestData.NotificationMode)
            {
                case NotificationModeEnum.Email:
                    var emailResult = await this.notificationHandler.SendEmail(requestData.Title, requestData.Message, requestData.RecipientIdentifier);

                    if (!emailResult)
                    {
                        this.logger.LogError($"Failed to send email to recipient :{requestData.RecipientIdentifier}");
                    }

                    this.Result.IsSuccessful = requestData.CanNotificationFailureCauseFlowFailure ? emailResult : true;
                    break;
                case NotificationModeEnum.Sms:
                    var sendSmsResult = await this.notificationHandler.SendSms(requestData.Message, requestData.RecipientIdentifier);

                    if (!sendSmsResult)
                    {
                        this.logger.LogError($"Failed to send SMS to recipient :{requestData.RecipientIdentifier}");
                    }

                    this.Result.IsSuccessful = requestData.CanNotificationFailureCauseFlowFailure ? sendSmsResult : true;
                    break;
                case NotificationModeEnum.PushNotification:
                    break;
            }

            return response;
        }

        protected override bool ValidateRequestData(SendNotificationActivityData requestData)
        {
            if (requestData.NotificationMode == NotificationModeEnum.None)
            {
                this.logger.LogError("Notification mode is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationModeNotProvided);
                return false;
            }

            if (requestData.Message.IsEmpty())
            {
                this.logger.LogError("Notification message is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationMessageNotProvided);
                return false;
            }

            if (requestData.RecipientIdentifier.IsEmpty())
            {
                this.logger.LogError("Notification recipient is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationRecipientNotProvided);
                return false;
            }

            if((requestData.NotificationMode == NotificationModeEnum.Email || requestData.NotificationMode == NotificationModeEnum.PushNotification) && requestData.Title.IsEmpty())
            {
                this.logger.LogError("Notification title is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationTitleNotProvided);
                return false;
            }

            return true;
        }
    }
}

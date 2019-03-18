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
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Business.Models.Response;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class SendNotificationActivity : ActivityBase<SendNotificationActivityData, SendNotificationActivityResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private INotificationHandler notificationHandler;

        public SendNotificationActivity(IActivityFactory activityFactory, IUnitOfWorkFactory unitOfWorkFactory, IEventLogger logger, INotificationHandler notificationHandler) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.notificationHandler = notificationHandler;
        }

        protected async override Task<SendNotificationActivityResult> ExecuteAsync(SendNotificationActivityData requestData)
        {
            this.CanIgnoreTrackableFailure = !requestData.CanNotificationFailureCauseFlowFailure;
            var response = new SendNotificationActivityResult();
            IList<NotificationMessageTemplate> notificationMessageTemplates = null;
            response.IsNotificationSuccessful = true;

            // Fetch the notification message templates based on the notification message type
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                notificationMessageTemplates = await unitOfWork.NotificationMessageTemplateRepository.GetNotificationTemplatesByMessageType((int)requestData.NotificationMessageType).ConfigureAwait(false);
            }

            if (null != notificationMessageTemplates)
            {
                foreach(var notificationRecipient in requestData.Recipients)
                {
                    // For each recipient check if notification preference is present. If yes, send notification only if preference matches. If the notification mode preference
                    // is not present then go ahead and send the notification for all available notification modes.
                    foreach (var notificationMessageTemplate in notificationMessageTemplates)
                    {
                        // Check if the notification preference is present
                        if (null != notificationRecipient.NotificationPreference && notificationMessageTemplate.NotificationModeId != (int)notificationRecipient.NotificationPreference)
                        {
                            // Continue to process the rest of the templates
                            continue;
                        }

                        switch (notificationMessageTemplate.NotificationModeId)
                        {
                            case (int)NotificationModeEnum.Email:
                                if (notificationRecipient.EmailId.IsEmpty())
                                {
                                    this.logger.LogError($"Failed to send email as the email id provided is empty.");
                                    this.Result.IsSuccessful = false;
                                    response.IsNotificationSuccessful = response.IsNotificationSuccessful && false;
                                }
                                else
                                {
                                    notificationMessageTemplate.Subject = this.ReplaceMergeFields(notificationMessageTemplate.Subject, requestData.SubjectMergeFields);
                                    notificationMessageTemplate.Body = this.ReplaceMergeFields(notificationMessageTemplate.Body, requestData.BodyMergeFields);
                                    var emailResult = await this.notificationHandler.SendEmail(notificationMessageTemplate.Subject, notificationMessageTemplate.Body, notificationRecipient.EmailId);

                                    if (!emailResult)
                                    {
                                        this.logger.LogError($"Failed to send email to recipient :{notificationRecipient.EmailId}");
                                    }

                                    response.IsNotificationSuccessful = response.IsNotificationSuccessful && emailResult;
                                    this.Result.IsSuccessful = requestData.CanNotificationFailureCauseFlowFailure ? emailResult : true;
                                }
                                
                                break;
                            case (int)NotificationModeEnum.Sms:
                                if (notificationRecipient.CompleteMobileNumber.IsEmpty())
                                {
                                    this.logger.LogError($"Failed to send sms as the mobile number provided is empty.");
                                    this.Result.IsSuccessful = false;
                                    response.IsNotificationSuccessful = response.IsNotificationSuccessful && false;
                                }
                                else
                                {
                                    notificationMessageTemplate.Body = this.ReplaceMergeFields(notificationMessageTemplate.Body, requestData.BodyMergeFields);
                                    var sendSmsResult = await this.notificationHandler.SendSms(notificationMessageTemplate.Body, notificationRecipient.CompleteMobileNumber);

                                    if (!sendSmsResult)
                                    {
                                        this.logger.LogError($"Failed to send SMS to recipient :{notificationRecipient.CompleteMobileNumber}");
                                    }

                                    response.IsNotificationSuccessful = response.IsNotificationSuccessful && sendSmsResult;
                                    this.Result.IsSuccessful = requestData.CanNotificationFailureCauseFlowFailure ? sendSmsResult : true;
                                }
                                
                                break;
                            case (int)NotificationModeEnum.PushNotification:
                                break;
                        }
                    }
                }
            }

            return response;
        }

        protected override bool ValidateRequestData(SendNotificationActivityData requestData)
        {
            if (null == requestData.Recipients)
            {
                this.logger.LogError("Notification recipients are not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationRecipientNotProvided);
                return false;
            }

            if(requestData.NotificationMessageType == NotificationMessageTypeEnum.None)
            {
                this.logger.LogError("Notification message type is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.NotificationMessageTypeNotProvided);
                return false;
            }

            return true;
        }

        private string ReplaceMergeFields(string template, IList<KeyValuePair<string, string>> mergeFields)
        {
            if(null == mergeFields)
            {
                return template;
            }

            var originalTemplate = template;
            foreach(var mergeField in mergeFields)
            {
                template = template.Replace(mergeField.Key, mergeField.Value);
            }

            if (template.Contains('{'))
            {
                StringBuilder warningMessage = new StringBuilder($"The template '{originalTemplate}' contains a merge field after replacement of keys:");
                foreach(var mergeFieldKey in mergeFields)
                {
                    warningMessage.Append(mergeFieldKey.Key);
                    warningMessage.Append(',');
                }

                this.logger.LogWarning(warningMessage.ToString());
            }

            return template;
        }
    }
}

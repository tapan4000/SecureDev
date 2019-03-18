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
    public class NotificationMessageTemplateSqlDataStoreStrategy : DataStoreStrategyBase<NotificationMessageTemplate>, INotificationMessageTemplateDataStoreStrategy
    {
        public NotificationMessageTemplateSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<IList<NotificationMessageTemplate>> GetNotificationTemplatesByMessageType(int notificationMessageType)
        {
            var notificationTemplates = await this.GetData(notificationTemplate => notificationTemplate.NotificationMessageTypeId == notificationMessageType).ConfigureAwait(false);
            return notificationTemplates;
        }
    }
}

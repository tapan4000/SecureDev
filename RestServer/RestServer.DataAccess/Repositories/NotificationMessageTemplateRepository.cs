using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Cache.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Interfaces.Strategies;

namespace RestServer.DataAccess.Repositories
{
    public class NotificationMessageTemplateRepository : RepositoryBase<NotificationMessageTemplate>, INotificationMessageTemplateRepository
    {
        private INotificationMessageTemplateDataStoreStrategy notificationMessageTemplateStrategy;

        public NotificationMessageTemplateRepository(IDependencyContainer dependencyContainer, INotificationMessageTemplateDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<NotificationMessageTemplate> cacheStrategyHandler, IEventLogger logger) : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.notificationMessageTemplateStrategy = dataStoreStrategy;
        }

        public Task<IList<NotificationMessageTemplate>> GetNotificationTemplatesByMessageType(int notificationMessageType)
        {
            return this.notificationMessageTemplateStrategy.GetNotificationTemplatesByMessageType(notificationMessageType);
        }
    }
}

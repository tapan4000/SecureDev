using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface INotificationMessageTemplateRepository : IRepository<NotificationMessageTemplate>
    {
        Task<IList<NotificationMessageTemplate>> GetNotificationTemplatesByMessageType(int notificationMessageType);
    }
}

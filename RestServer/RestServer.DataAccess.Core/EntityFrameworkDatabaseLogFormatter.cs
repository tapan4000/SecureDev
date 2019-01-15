using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core
{
    public class EntityFrameworkDatabaseLogFormatter : DatabaseLogFormatter
    {
        public EntityFrameworkDatabaseLogFormatter(DbContext context, Action<string> writeAction) : base(context, writeAction)
        {
        }

        public override void LogCommand<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
        }

        public override void LogParameter<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext, DbParameter parameter)
        {
        }

        public override void LogResult<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
        }

        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        public override void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
        }
    }
}

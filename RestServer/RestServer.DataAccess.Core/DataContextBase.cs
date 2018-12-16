using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer.Entities.DataAccess.Core;
using System.Data.Entity;
using RestServer.IoC;
using RestServer.DataAccess.Core.Interfaces;
using System.Linq;
using System.Data.Entity.Validation;

namespace RestServer.DataAccess.Core
{
    [IoCRegistration(IoCLifetime.Hierarchical)]
    public abstract class DataContextBase : DbContext, IDataContext
    {
        public DataContextBase(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IEntityBase> GetModifiedEntities()
        {
            return this.ChangeTracker.Entries<IEntityBase>()
                .Where(entity => entity.State == EntityState.Added || entity.State == EntityState.Modified || entity.State == EntityState.Deleted)
                .Select(entity =>
                {
                    entity.Entity.ObjectState = GetStateFromEntityFrameworkState(entity.State);
                    return entity.Entity;
                }); 
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await this.SaveChangesAsync().ConfigureAwait(false);
            }
            catch(DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}:{e.ErrorMessage}");

                var consolidatedMessage = $"Exception Message: {ex.Message}, Validation Errors: {string.Join(",", errorMessages) }";

                throw new DbEntityValidationException(consolidatedMessage);
            }
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>().AsNoTracking();
        }

        private static ObjectState GetStateFromEntityFrameworkState(EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Detached:
                    return ObjectState.Unchanged;
                case EntityState.Unchanged:
                    return ObjectState.Unchanged;
                case EntityState.Added:
                    return ObjectState.Added;
                case EntityState.Deleted:
                    return ObjectState.Deleted;
                case EntityState.Modified:
                    return ObjectState.Modified;
                default:
                    throw new ArgumentOutOfRangeException("entityState");
            }
        }
    }
}

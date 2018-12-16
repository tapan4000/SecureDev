using Newtonsoft.Json;
using RestServer.Cache.Interfaces;
using RestServer.Logging.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class RedisCacheStrategy<T> : CacheStrategyBase<T>
    {
        private static readonly JsonSerializerSettings JsonSerializerSetting = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly string redisConnectionString;

        private int database;

        private readonly IEventLogger logger;

        private readonly IConnectionMultiplexer connectionMultiplexer;

        public RedisCacheStrategy(string cacheConnectionString, int database, IEventLogger logger)
        {
            this.redisConnectionString = cacheConnectionString;
            this.database = database;
            this.logger = logger;
            this.connectionMultiplexer = this.GetConnectionMultiplexer();
        }

        public override async Task<bool> ClearCacheAsync()
        {
            using(var adminConnection = this.GetAdminConnection())
            {
                var endpoints = adminConnection.GetEndPoints();
                var servers = endpoints.Select(endpoint => adminConnection.GetServer(endpoint));
                foreach(var server in servers)
                {
                    await server.FlushDatabaseAsync(this.database, CommandFlags.HighPriority).ConfigureAwait(false);
                }
            }

            return true;
        }

        public override async Task<bool> DeleteAsync(string key)
        {
            var cacheDb = this.GetDatabase(this.database);
            if (null != cacheDb && !string.IsNullOrWhiteSpace(key))
            {
                return await cacheDb.KeyDeleteAsync(key).ConfigureAwait(false);
            }

            return true;
        }

        public override async Task<bool> DoesKeyExistAsync(string key)
        {
            var cacheDb = this.GetDatabase(this.database);
            if(null != cacheDb && !string.IsNullOrWhiteSpace(key))
            {
                return await cacheDb.KeyExistsAsync(key).ConfigureAwait(false);
            }

            return false;
        }

        public override async Task<T> GetAsync(string key)
        {
            var cacheDb = this.GetDatabase(this.database);
            if (null != cacheDb && !string.IsNullOrWhiteSpace(key))
            {
                var value = await cacheDb.StringGetAsync(key).ConfigureAwait(false);
                return value.IsNull ? default(T) : Deserialize<T>(value);
            }

            return default(T);
        }

        public override async Task<bool> InsertOrUpdateAsync(string key, T entity, TimeSpan? expiry = null)
        {
            var cacheDb = this.GetDatabase(this.database);
            if (null != cacheDb && !string.IsNullOrWhiteSpace(key))
            {
                return await cacheDb.StringSetAsync(key, Serialize(entity), expiry).ConfigureAwait(false);
            }

            return false;
        }

        private static string Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(
                o,
                Formatting.None,
                JsonSerializerSetting);
        }

        private static T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(
                value,
                JsonSerializerSetting);
        }

        private IConnectionMultiplexer GetConnectionMultiplexer()
        {
            var multiplexer = RedisConnectionMultiplexer.Instance.GetConnectionMultiplexer(this.redisConnectionString);
            if(null == multiplexer)
            {
                throw new NullReferenceException("The REDIS connection multiplexer cannot be null. Please ensure that the connection muliplexer has been initialized.");
            }

            return multiplexer;
        }

        private IDatabase GetDatabase(int db)
        {
            if(null != this.connectionMultiplexer && this.connectionMultiplexer.IsConnected)
            {
                return this.connectionMultiplexer.GetDatabase(db);
            }

            return null;
        }

        private ConnectionMultiplexer GetAdminConnection()
        {
            var config = ConfigurationOptions.Parse(this.redisConnectionString);
            config.AllowAdmin = true;
            config.ConnectRetry = 10;
            config.ConnectTimeout = 10000;
            config.SyncTimeout = 10000;

            var connection = ConnectionMultiplexer.Connect(config);
            return connection;
        }
    }
}

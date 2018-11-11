using RestServer.Logging.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface IRedisConnectionMultiplexer
    {
        int MultiplexerPoolSize { get; }

        IConnectionMultiplexer GetConnectionMultiplexer(string redisConnectionString);

        bool Initialize(string[] connectionStrings, int minOnDemandIoThreadCount, IEventLogger logger, int multiplexerPoolSize, bool isSubscribedToExternalChannel, int subscriptionEventMonitorFrequency);

        bool Reset(string[] connectionStrings, int multiplexerPoolSize, bool isSubscribedToExternalChannel, int subscriptionEventMonitorFrequency);
    }
}

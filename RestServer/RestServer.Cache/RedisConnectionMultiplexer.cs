using RestServer.Cache.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace RestServer.Cache
{
    public class RedisConnectionMultiplexer : IRedisConnectionMultiplexer
    {
        private const string SubscriptionChannel = "Generic";

        private int multiplexerPoolSize;

        private static readonly Lazy<IRedisConnectionMultiplexer> IntanceObject = new Lazy<IRedisConnectionMultiplexer>(() => new RedisConnectionMultiplexer());

        private static readonly ConcurrentDictionary<string, CircularList<IConnectionMultiplexer>> ConnectionMultiplexers = new ConcurrentDictionary<string, CircularList<IConnectionMultiplexer>>();

        private static readonly ConcurrentDictionary<RedisChannel, RedisValue> PublishedEventsReceived = new ConcurrentDictionary<RedisChannel, RedisValue>();

        private static readonly object MultiplexerLockObj = new object();

        private int subscriptionEventMonitorFrequency;

        private StringWriter redisConnectLog = new StringWriter(CultureInfo.InvariantCulture);

        private IEventLogger logger;

        /// <summary>
        /// Prevents a default instance of the <see cref="RedisConnectionMultiplexer"/> class from being created.
        /// </summary>
        private RedisConnectionMultiplexer()
        {
        }

        public event EventHandler<CacheNotificationEventArgs> RedisPublishEventReceived;

        public static IRedisConnectionMultiplexer Instance { get { return IntanceObject.Value; } }

        public int MultiplexerPoolSize { get { return this.multiplexerPoolSize; } }

        public bool Initialize(string[] connectionStrings, int minOnDemandIoThreadCount, IEventLogger logger, int multiplexerPoolSize, bool isSubscribedToExternalChannel = true, int subscriptionEventMonitorFrequency = 30)
        {
            this.logger = logger;
            this.multiplexerPoolSize = multiplexerPoolSize;

            if(minOnDemandIoThreadCount > 0)
            {
                this.SetMinOnDemandThreadCount(minOnDemandIoThreadCount);
            }

            int connectionStringIndex = 0;
            bool isInitializationSuccessful = true;
            foreach(var connectionString in connectionStrings)
            {
                try
                {
                    CircularList<IConnectionMultiplexer> connectionMultiplexerList;
                    if(!ConnectionMultiplexers.TryGetValue(connectionString, out connectionMultiplexerList))
                    {
                        connectionMultiplexerList = this.GetConnectionMultiplexerPool(connectionString, isSubscribedToExternalChannel);
                        if (!connectionMultiplexerList.Any())
                        {
                            isInitializationSuccessful = false;
                        }
                        else
                        {
                            if (isSubscribedToExternalChannel)
                            {
                                this.subscriptionEventMonitorFrequency = subscriptionEventMonitorFrequency;
                                Task.Run(async () => await this.ProcessSubscribedEvents().ConfigureAwait(false)).ConfigureAwait(false);
                            }

                            ConnectionMultiplexers.TryAdd(connectionString, connectionMultiplexerList);
                        }
                    }
                }
                catch(Exception ex)
                {
                    isInitializationSuccessful = false;
                    this.logger.LogException($"Error occurred while connecting to REDIS index: {connectionStringIndex}", ex);
                }

                connectionStringIndex++;
            }

            return isInitializationSuccessful;
        }

        public IConnectionMultiplexer GetConnectionMultiplexer(string redisConnectionString)
        {
            IConnectionMultiplexer mux = null;
            try
            {
                CircularList<IConnectionMultiplexer> connectionMultiplexerList;
                if(!ConnectionMultiplexers.TryGetValue(redisConnectionString, out connectionMultiplexerList))
                {
                    if(null != this.logger)
                    {
                        this.logger.LogError("Redis connection multiplexer not found.");
                    }
                }

                if(null != connectionMultiplexerList)
                {
                    // Iterate through the multiplexer list to get a multiplexer that is connected.
                    var muxCount = connectionMultiplexerList.Count;
                    do
                    {
                        mux = connectionMultiplexerList.GetSafeNext();
                    }
                    while (!mux.IsConnected && --muxCount >= 0);
                }
            }
            catch(Exception ex)
            {
                this.logger.LogException("Error occurred while fetching connection multiplexer.", ex);
            }

            return mux;
        }

        public bool Reset(string[] connectionStrings, int multiplexerPoolSize, bool isSubscribedToExternalChannel, int subscriptionEventMonitorFrequency)
        {
            var multiplexers = ConnectionMultiplexers.Where(multiplexer => !connectionStrings.Contains(multiplexer.Key));

            // Close the multiplexers that are not present in the new list, however, do not close the existing multiplexers that are expected to be in the new list
            // to avoid any service degradation.
            this.CloseConnections(multiplexers);
            return this.Initialize(connectionStrings, 0, this.logger, multiplexerPoolSize, isSubscribedToExternalChannel, subscriptionEventMonitorFrequency);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CloseConnections(ConnectionMultiplexers.ToList());
                if(null != this.redisConnectLog && TextWriter.Null != this.redisConnectLog)
                {
                    this.redisConnectLog.Dispose();
                }
            }
        }

        private async Task<bool> ProcessSubscribedEvents()
        {
            foreach(var publishedEvent in PublishedEventsReceived)
            {
                if(null != this.RedisPublishEventReceived)
                {
                    this.logger.LogInformation($"Raising REDIS publish event {publishedEvent.Value} via channel {publishedEvent.Key}.");
                    this.RedisPublishEventReceived(this, new CacheNotificationEventArgs(publishedEvent.Value));
                }

                RedisValue redisValue;
                if(!PublishedEventsReceived.TryRemove(publishedEvent.Key, out redisValue))
                {
                    this.logger.LogError($"Failed to remove publish event {redisValue} via channel {publishedEvent.Key}.");
                }
            }

            // Keep checking for published events periodically.
            await Task.Delay(TimeSpan.FromSeconds(this.subscriptionEventMonitorFrequency)).ContinueWith(async task => await this.ProcessSubscribedEvents().ConfigureAwait(false));
            return true;
        }

        private void CloseConnections(IEnumerable<KeyValuePair<string, CircularList<IConnectionMultiplexer>>> multiplexers)
        {
            if(null != multiplexers)
            {
                foreach (var connectionMultiplexerList in multiplexers)
                {
                    Parallel.ForEach(connectionMultiplexerList.Value.Where(mux => null != mux),
                        connectionMux =>
                        {
                            connectionMux.ConnectionFailed -= this.ConnectionMultiplexerConnectionFailed;
                            connectionMux.ConnectionRestored -= this.ConnectionMultiplexerConnectionRestored;
                            connectionMux.InternalError -= this.ConnectionMultiplexerInternalError;
                            connectionMux.ErrorMessage -= this.ConnectionMultiplexerErrorMessage;
                            connectionMux.Dispose();
                        });

                    // Remove from the list
                    CircularList<IConnectionMultiplexer> multiplexerList;
                    if(!ConnectionMultiplexers.TryRemove(connectionMultiplexerList.Key, out multiplexerList))
                    {
                        this.logger.LogError($"Failed to remove the connection multiplexer from the list.");
                    }
                }
            }
        }

        private CircularList<IConnectionMultiplexer> GetConnectionMultiplexerPool(string connectionString, bool isSubscribedToRedisChannel)
        {
            var configurationOptions = ConfigurationOptions.Parse(connectionString);
            var masterServerAddress = configurationOptions.EndPoints.First().ToString();
            var connectionMultiplexerPool = new CircularList<IConnectionMultiplexer>(this.MultiplexerPoolSize);
            for(int i=0; i< this.MultiplexerPoolSize; i++)
            {
                var connectionid = i + 1;

                try
                {
                    this.redisConnectLog.WriteLine($"Initiating connection: {connectionid}/{this.MultiplexerPoolSize}");
                    this.logger.LogVerbose($"Connecting to redis master");
                    IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions, this.redisConnectLog);
                    this.logger.LogVerbose($"Connected to redis master");
                    connectionMultiplexer.ConnectionFailed += this.ConnectionMultiplexerConnectionFailed;
                    connectionMultiplexer.ConnectionRestored += this.ConnectionMultiplexerConnectionRestored;
                    connectionMultiplexer.InternalError += this.ConnectionMultiplexerInternalError;
                    connectionMultiplexer.ErrorMessage += this.ConnectionMultiplexerErrorMessage;

                    if (isSubscribedToRedisChannel)
                    {
                        // Subscribe to the REDIS channel to receive any events from external triggers.
                        connectionMultiplexer.GetSubscriber().Subscribe(SubscriptionChannel,
                            (redisChannel, redisValue) => this.SubscribedEventReceived(redisChannel, redisValue, connectionid));
                        this.logger.LogVerbose($"Subscribed to channel {SubscriptionChannel} for connection {connectionid}");
                    }

                    connectionMultiplexerPool.Add(connectionMultiplexer);

                    this.logger.LogVerbose(this.redisConnectLog.ToString());
                }
                catch(Exception ex)
                {
                    this.logger.LogInformation(this.redisConnectLog.ToString());
                    this.logger.LogException($"Failed to connect to {masterServerAddress} for connectionid {connectionid}", ex);
                }
            }

            return connectionMultiplexerPool;
        }

        private void SubscribedEventReceived(RedisChannel redisChannel, RedisValue redisValue, int connectionId)
        {
            PublishedEventsReceived[redisChannel] = redisValue;
            this.logger.LogInformation($"REDIS publish event. Received {redisValue} via channel {redisChannel} for connection {connectionId}.");
        }

        private void ConnectionMultiplexerInternalError(object sender, InternalErrorEventArgs e)
        {
            this.logger.LogError($"Connection multiplexer internal error triggered for {e.EndPoint}.");
            if(null != e.Exception)
            {
                this.logger.LogException($"Connection multiplexer internal error triggered for {e.EndPoint}.", e.Exception);
            }
        }

        private void ConnectionMultiplexerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            this.logger.LogError($"Connection multiplexer error message triggered for {e.EndPoint}.");
            if (null != e.Message)
            {
                this.logger.LogError($"Connection multiplexer error message triggered for {e.EndPoint}. Message: {e.Message}");
            }
        }

        private void ConnectionMultiplexerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            this.logger.LogVerbose($"Connection multiplexer connection restored for {e.EndPoint}.");
            if (null != e.Exception)
            {
                this.logger.LogException($"Connection multiplexer connection restored for {e.EndPoint}. Failure type: {e.FailureType}", e.Exception);
            }
        }

        private void ConnectionMultiplexerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            this.logger.LogError($"Connection multiplexer connection failed for {e.EndPoint}.");
            if (null != e.Exception)
            {
                this.logger.LogException($"Connection multiplexer connection failed for {e.EndPoint}. Failure type: {e.FailureType}", e.Exception);
            }
        }

        private void SetMinOnDemandThreadCount(int minOnDemandIoThreadCount)
        {
            int defaultMinOnDemandWorkerThreadCount;
            int defaultMinOnDemandIoThreadCount;

            ThreadPool.GetMinThreads(out defaultMinOnDemandWorkerThreadCount, out defaultMinOnDemandIoThreadCount);

            try
            {
                var evaluatedMinIoThreadCount = minOnDemandIoThreadCount > defaultMinOnDemandIoThreadCount ? minOnDemandIoThreadCount : defaultMinOnDemandIoThreadCount;
                var result = ThreadPool.SetMinThreads(defaultMinOnDemandWorkerThreadCount, evaluatedMinIoThreadCount);
                if (result)
                {
                    this.logger.LogVerbose($"Successfully set the min IO thread count to {evaluatedMinIoThreadCount}");
                }
                else
                {
                    this.logger.LogVerbose($"Failed to set the min IO thread count to {evaluatedMinIoThreadCount}. Existing min thread count is {defaultMinOnDemandIoThreadCount}");
                }
            }
            catch(Exception ex)
            {
                this.logger.LogException("Error occured while setting the min thread count.", ex);
            }
        }

        private class CircularList<T> : List<T>
        {
            private static readonly object LockObj = new object();

            private IEnumerator circularEnumerator;

            public CircularList(int capacity) : base(capacity)
            {
            }

            public T GetSafeNext()
            {
                lock (LockObj)
                {
                    this.circularEnumerator = this.circularEnumerator ?? this.GetEnumerator();
                    if (!this.circularEnumerator.MoveNext())
                    {
                        this.circularEnumerator.Reset();
                        this.circularEnumerator.MoveNext();
                    }

                    return (T)this.circularEnumerator.Current;
                }
            }
        }
    }
}

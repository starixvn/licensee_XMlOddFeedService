namespace Fanex.OddsFeed.Common.Helpers
{
    using System;
    using StackExchange.Redis;

    public static class MyRedisConnectorHelper
    {
        private const int redisTimeOut = 30000;

        private static readonly Lazy<ConnectionMultiplexer> redisConnection = new Lazy
               <ConnectionMultiplexer>(() =>
               {
                   var options = ConfigurationOptions.Parse(System.Configuration.ConfigurationManager.AppSettings["RedisHost"]);
                   options.ConnectTimeout = redisTimeOut;
                   options.SyncTimeout = redisTimeOut;
                   options.AsyncTimeout = redisTimeOut;

                   return ConnectionMultiplexer.
                      Connect(options);
               });

        public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return redisConnection.Value;
            }
        }
    }
}
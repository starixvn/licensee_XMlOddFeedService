using System;
using StackExchange.Redis;

namespace Fanex.OddsFeed.Common.Helpers
{
    public static class MyRedisConnectorHelper
    {
        private static Lazy<ConnectionMultiplexer> _RedisConnection = new Lazy
               <ConnectionMultiplexer>(() =>
               {
                   var options = ConfigurationOptions.Parse(System.Configuration.ConfigurationManager.AppSettings["RedisHost"]);
                   options.ConnectTimeout = 30000;
                   options.SyncTimeout = 30000;
                   options.AsyncTimeout = 30000;

                   return ConnectionMultiplexer.
                      Connect(options);
               });

        public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return _RedisConnection.Value;
            }
        }
    }
}
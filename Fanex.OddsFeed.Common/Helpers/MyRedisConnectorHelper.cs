using StackExchange.Redis;
using System;

namespace Fanex.OddsFeed.Common.Helpers
{
    public static class MyRedisConnectorHelper
    {
        private static Lazy<StackExchange.Redis
         .ConnectionMultiplexer> _RedisConnection = new Lazy
               <StackExchange.Redis.ConnectionMultiplexer>(() =>
               {
                   var options = ConfigurationOptions.Parse(System.Configuration.ConfigurationManager.AppSettings["RedisHost"]);
                   options.ConnectTimeout = 30000;
                   options.SyncTimeout = 30000;
                   options.AsyncTimeout = 30000;

                   return StackExchange.Redis.ConnectionMultiplexer.
                      Connect(options);
               });

        public static StackExchange.Redis.ConnectionMultiplexer
           RedisConnection
        {
            get
            {
                return _RedisConnection.Value;
            }
        }
    }
}

using System;

namespace Fanex.OddsFeed.Common.Helpers
{
    public static class MyRedisConnectorHelper
    {
        private static Lazy<StackExchange.Redis
         .ConnectionMultiplexer> _RedisConnection = new Lazy
               <StackExchange.Redis.ConnectionMultiplexer>(() =>
               {
                   return StackExchange.Redis.ConnectionMultiplexer.
                      Connect(System.Configuration.ConfigurationManager.AppSettings["RedisHost"]);
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

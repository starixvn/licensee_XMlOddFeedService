using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Fanex.OddsFeed.Common.Configurations;
using System;
using System.Collections.Generic;

namespace Fanex.OddsFeed.Common.Caching
{
    public static class ClusterRepository
    {
        private static Cluster _cluster;
        private static readonly object padlock = new object();

        public static IBucket GetBucket(string bucketName)
        {
            Initialize();
            return _cluster.OpenBucket(bucketName);
        }

        public static void Close()
        {
            lock (padlock)
            {
                if (_cluster != null)
                {
                    _cluster.Dispose();
                }
            }
        }

        private static void Initialize()
        {
            lock (padlock)
            {
                if (_cluster == null)
                {
                    var couchBaseClientConfig = new ClientConfiguration
                    {
                        Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
                    };
                    _cluster = new Cluster(couchBaseClientConfig);
                }
            }
        }
    }
}

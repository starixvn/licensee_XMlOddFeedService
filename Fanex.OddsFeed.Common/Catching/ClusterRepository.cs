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

        public static void Initizlize()
        {
            var couchBaseClientConfig = new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
            };

            _cluster = new Cluster(couchBaseClientConfig);
        }

        public static void Close()
        {
            _cluster.Dispose();
        }

        public static IBucket GetBucket(string bucketName)
        {
            return _cluster.OpenBucket(bucketName);
        }
    }
}

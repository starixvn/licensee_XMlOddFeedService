using System;

namespace CouchbaseApp
{
    public class ItemResult
    {
        public int Index { get; set; }

        public TimeSpan LiveDb { get; set; }

        public TimeSpan DeadballDb { get; set; }

        public TimeSpan OutrightDb { get; set; }

        public TimeSpan All { get; set; }
    }
}
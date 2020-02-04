namespace Fanex.OddsFeed.Common.FilterAtribute
{
    using System;

    public class ThrottleInfo
    {
        public int RequestCount { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}

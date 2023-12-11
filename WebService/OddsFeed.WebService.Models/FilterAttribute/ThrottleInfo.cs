namespace OddsFeed.WebService.Models.FilterAttribute
{
    using System;

    public class ThrottleInfo
    {
        public int RequestCount { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
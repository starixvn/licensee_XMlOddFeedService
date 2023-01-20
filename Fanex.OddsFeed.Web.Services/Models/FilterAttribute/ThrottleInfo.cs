namespace Fanex.OddsFeed.Web.Services.Models.FilterAttribute
{
    public class ThrottleInfo
    {
        public int RequestCount { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
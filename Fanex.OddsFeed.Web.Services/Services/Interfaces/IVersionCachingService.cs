namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Models.Cache;

    public interface IVersionCachingService
    {
        CacheVersion GetRightVersionOddsFeed(UserProfile profile);

        int GetRightVersionResultFeed(int delayTimes);
    }
}
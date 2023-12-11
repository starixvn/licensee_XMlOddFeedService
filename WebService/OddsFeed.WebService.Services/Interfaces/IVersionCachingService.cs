namespace OddsFeed.WebService.Services.Interfaces
{
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Models.Cache;

    public interface IVersionCachingService
    {
        CacheVersion GetRightVersionOddsFeed(UserProfile profile);

        int GetRightVersionResultFeed(int delayTimes);
    }
}
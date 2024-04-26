namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Models.Cache;

    public interface IVersionCachingService
    {
        Task<CacheVersion> GetRightVersionOddsFeed(UserProfile profile);

        int GetRightVersionResultFeed(int delayTimes);
    }
}
namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using Fanex.OddsFeed.Common.DTO;

    public interface IVersionCaching
    {
        CacheVersion GetRightVersionOddsFeed(UserProfile profile);

        int GetRightVersionResultFeed(int delayTimes);
    }
}
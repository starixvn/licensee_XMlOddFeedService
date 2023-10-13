namespace Fanex.OddsFeed.AutoJobs.Services
{
    public interface IResultFeedService
    {
        Task Reset();

        Task Update();
    }
}
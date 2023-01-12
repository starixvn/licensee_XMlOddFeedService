namespace Fanex.OddsFeed.AutoJobs.Services.Interfaces
{
    public interface IResultFeedService
    {
        Task Reset();

        Task Update();
    }
}
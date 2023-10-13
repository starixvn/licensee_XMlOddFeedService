namespace Fanex.OddsFeed.AutoJobs.Services
{
    public interface IOddsfeedService
    {
        Task Reset();

        Task Update();
    }
}
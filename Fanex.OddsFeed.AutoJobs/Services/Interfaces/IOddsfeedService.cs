namespace Fanex.OddsFeed.AutoJobs.Services.Interfaces
{
    public interface IOddsfeedService
    {
        Task Reset();

        Task Update();
    }
}
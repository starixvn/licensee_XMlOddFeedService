namespace OddsFeed.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IResultFeedService
    {
        Task Reset();

        Task Update();
    }
}
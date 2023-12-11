namespace OddsFeed.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IOddsfeedService
    {
        Task Reset();

        Task Update();
    }
}
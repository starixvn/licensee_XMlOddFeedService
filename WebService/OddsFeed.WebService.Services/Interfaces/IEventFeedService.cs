namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;

    public interface IEventFeedService
    {
        Task<string> BuildXmlEventAsync(UserProfile userProfile, bool isSample = false, bool isOutright = true);
    }
}
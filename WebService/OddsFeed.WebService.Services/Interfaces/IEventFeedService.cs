namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;

    public interface IEventFeedService
    {
        Task<string> BuildXmlEvent(UserProfile userProfile, bool isSample = false, bool isOutright = true);
    }
}
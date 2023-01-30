namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.DTO;

    public interface IEventFeedService
    {
        Task<string> BuildXmlEvent(UserProfile userProfile, bool isSample = false, bool isOutright = true);
    }
}
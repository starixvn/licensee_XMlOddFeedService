namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.OddsData;

    public interface IResultFeedService
    {
        Task<string> BuildXmlDocument(OddsFeedsModel model);
    }
}
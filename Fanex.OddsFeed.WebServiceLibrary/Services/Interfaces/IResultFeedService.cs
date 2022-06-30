namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public interface IResultFeedService
    {
        string BuildXmlDocument(OddsFeedsModel model);
    }
}
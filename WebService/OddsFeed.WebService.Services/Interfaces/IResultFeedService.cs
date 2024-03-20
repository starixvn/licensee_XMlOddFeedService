namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;
    using OddsFeed.Models.OddsData;

    public interface IResultFeedService
    {
        Task<string> BuildXmlDocumentAsync(OddsFeedsModel model);
    }
}
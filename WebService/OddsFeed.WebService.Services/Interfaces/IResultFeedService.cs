namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;
    using OddsFeed.Models.OddsData;

    public interface IResultFeedService
    {
        Task<string> BuildXmlDocument(OddsFeedsModel model);
    }
}
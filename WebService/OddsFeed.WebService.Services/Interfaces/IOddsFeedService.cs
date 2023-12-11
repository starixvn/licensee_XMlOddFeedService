namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Data;
    using System.Threading.Tasks;
    using OddsFeed.Models.OddsData;

    public interface IOddsFeedService
    {
        Task<string> BuildXmlDocument(OddsFeedsModel model);

        Task<DataTable> GetCountryListAsync();
    }
}
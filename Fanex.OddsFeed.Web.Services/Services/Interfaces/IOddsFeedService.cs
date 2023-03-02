namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using System.Data;
    using Fanex.OddsFeed.Models.OddsData;

    public interface IOddsFeedService
    {
        Task<string> BuildXmlDocument(OddsFeedsModel model);

        Task<DataTable> GetCountryListAsync();
    }
}
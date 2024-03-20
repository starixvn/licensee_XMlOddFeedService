namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.OddsData;

    public interface IOddsFeedService
    {
        Task<string> BuildXmlDocumentAsync(OddsFeedsModel model);

        Task<IEnumerable<Country>> GetCountryListAsync();
    }
}
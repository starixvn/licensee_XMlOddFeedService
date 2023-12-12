namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.OddsData;

    public interface IOddsFeedService
    {
        Task<string> BuildXmlDocument(OddsFeedsModel model);

        Task<IEnumerable<Country>> GetCountryListAsync();
    }
}
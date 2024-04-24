namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using OddsFeed.Models.Matches;

    public interface IResultFeedDataService
    {
        Task<IList<Match>> GetMatchResultDetailsAsync(ICriteria criteria);
    }
}
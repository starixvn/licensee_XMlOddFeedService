namespace OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Outright;

    public interface IOddsDataService
    {
        Task<IList<Match>> GetLiveMatchAsync(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Match>> GetDeadballMatchAsync(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Outright>> GetOutrightAsync();
    }
}
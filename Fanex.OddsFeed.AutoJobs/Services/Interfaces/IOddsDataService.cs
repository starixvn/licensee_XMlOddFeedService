using Fanex.OddsFeed.Models;
using Fanex.OddsFeed.Models.Matches;
using Fanex.OddsFeed.Models.Outright;

namespace Fanex.OddsFeed.AutoJobs.Services.Interfaces
{
    public interface IOddsDataService
    {
        Task<IList<Match>> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Outright> GetOutrights();
    }
}
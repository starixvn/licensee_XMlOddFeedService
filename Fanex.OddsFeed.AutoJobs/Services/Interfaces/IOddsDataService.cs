namespace Fanex.OddsFeed.AutoJobs.Services.Interfaces
{
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Outright;

    public interface IOddsDataService
    {
        Task<IList<Match>> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Match>> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Outright>> GetOutrights();
    }
}
namespace Fanex.OddsFeed.AutoJobs.Services.Interfaces
{
    using Fanex.OddsFeed.AutoJobs.Models;
    using Fanex.OddsFeed.AutoJobs.Models.Matches;
    using Fanex.OddsFeed.AutoJobs.Models.Outright;

    public interface IOddsDataService
    {
        Task<IList<Match>> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Outright> GetOutrights();
    }
}
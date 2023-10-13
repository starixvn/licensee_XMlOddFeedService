namespace Fanex.OddsFeed.AutoJobs.Services
{
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Outright;

    public interface IOddsDataService
    {
        Task<IList<Match>> GetLiveMatchAsync(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Match>> GetDeadballMatchAsync(IList<BetTypeSchema> betTypeSchemas);

        Task<IList<Outright>> GetOutrightAsync();
    }
}
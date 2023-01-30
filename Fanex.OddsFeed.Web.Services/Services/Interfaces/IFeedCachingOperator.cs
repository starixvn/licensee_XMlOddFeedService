namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.OddsData;

    public interface IFeedCachingOperator
    {
        Task<OddsData> GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        Task<IList<BetTypeSchema>> GetBetTypes();

        Task<IList<Match>> GetMatchResultDetails(int delayTimes);
    }
}
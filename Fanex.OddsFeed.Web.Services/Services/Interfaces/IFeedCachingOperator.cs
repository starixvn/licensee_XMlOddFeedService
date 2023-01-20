namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.OddsData;

    public interface IFeedCachingOperator
    {
        OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        IList<BetTypeSchema> GetBetTypes();

        IList<Match> GetMatchResultDetails(int delayTimes);
    }
}
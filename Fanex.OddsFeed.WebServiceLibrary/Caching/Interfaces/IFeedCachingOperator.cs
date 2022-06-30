namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Schemas;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public interface IFeedCachingOperator
    {
        OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        IList<BetTypeSchema> GetBetTypes();

        IList<Match> GetMatchResultDetails(int delayTimes);
    }
}
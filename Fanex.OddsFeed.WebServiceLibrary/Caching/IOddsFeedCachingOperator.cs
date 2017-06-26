namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System.Collections.Generic;
    using Common.Schemas;
    using Fanex.OddsFeed.Common.DTO;
    using Models;

    public interface IOddsFeedCachingOperator
    {
        OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        IList<BetTypeSchema> GetBetTypes();
    }
}

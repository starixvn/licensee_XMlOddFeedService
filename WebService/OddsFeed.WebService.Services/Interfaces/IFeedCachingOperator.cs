namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;

    public interface IFeedCachingOperator
    {
        Task<OddsData> GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        Task<IList<BetTypeSchema>> GetBetTypes();

        Task<IList<Match>> GetMatchResultDetails(int delayTimes);
    }
}
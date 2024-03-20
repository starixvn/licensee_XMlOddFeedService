namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;

    public interface IFeedCachingOperator
    {
        Task<OddsData> GetOddsDataAsync(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch);

        Task<IList<BetTypeSchema>> GetBetTypeAsync();

        Task<IList<Match>> GetMatchResultDetailAsync(int delayTimes);
    }
}
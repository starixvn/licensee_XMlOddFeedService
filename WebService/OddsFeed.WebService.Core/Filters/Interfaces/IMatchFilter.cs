namespace OddsFeed.WebService.Core.Filters
{
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;

    public interface IMatchFilter
    {
        bool FilterMatch(Match match, OddsFeedsModel model);

        bool FilterBetType(MatchOdds matchOdds, OddsFeedsModel model);
    }
}
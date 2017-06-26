namespace Fanex.OddsFeed.WebServiceLibrary.Filters
{
    using Common.DTO;
    using Models;

    public interface IMatchFilter
    {
        bool FilterMatch(Match match, OddsFeedsModel model);

        bool FilterBetType(MatchOdds matchOdds, OddsFeedsModel model);
    }
}

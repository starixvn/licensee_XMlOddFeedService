namespace Fanex.OddsFeed.WebServiceLibrary.Filters
{
    using Common.DTO;
    using Common.Helpers;
    using Models;
    public class MatchFilter : IMatchFilter
    {
        public bool FilterMatch(Match match, OddsFeedsModel model)
        {
            return FilterBySport(model.UserProfile, model.UserQuery, match) &&
                    FilterByLeague(model.UserProfile, model.UserQuery, match) &&
                    FilterByCountry(model.UserProfile, model.UserQuery, match) &&
                    FilterByKeyword(model.UserProfile, match);
        }

        public bool FilterBetType(MatchOdds matchOdds, OddsFeedsModel model)
        {
            return (matchOdds.TimeStamp > model.TimeStamp)
                && FilterByOddsProfile(model.UserProfile, model.IsSample, matchOdds)
                && FilterByOddsFeedFilter(model.UserQuery, matchOdds);
        }

        private static bool FilterByOddsProfile(UserProfile userProfile, bool isSample, MatchOdds matchOdds)
        {
            return isSample 
                || userProfile.BetTypes == null
                || userProfile.BetTypes.Count == 0
                || userProfile.BetTypes.Contains(matchOdds.BetTypeId);
        }

        private static bool FilterByOddsFeedFilter(UserQuery userQuery, MatchOdds matchOdds)
        {
            return userQuery.BetTypes == null
                || userQuery.BetTypes.Count == 0
                || userQuery.BetTypes.Contains(matchOdds.BetTypeId);
        }

        private static bool FilterBySport(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            return (userProfile.Sports == null || userProfile.Sports.Count == 0 || userProfile.Sports.Contains(match.SportId)) 
                && (userQuery.Sports == null || userQuery.Sports.Count == 0 || userQuery.Sports.Contains(match.SportId));
        }

        private static bool FilterByLeague(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            return (userProfile.Leagues == null || userProfile.Leagues.Count == 0 || userProfile.Leagues.Contains(match.LeagueId))
                && (userQuery.Leagues == null || userQuery.Leagues.Count ==0 || StringHelper.ContainInList(userQuery.Leagues, match.LeagueEn));
        }

        private static bool FilterByCountry(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            return (userProfile.Countries == null || userProfile.Countries.Count == 0 || StringHelper.ContainInList(userProfile.Countries, match.CountryCode))
                && (userQuery.Countries == null || userQuery.Countries.Count == 0 || StringHelper.ContainInList(userQuery.Countries, match.CountryCode));
        }

        private static bool FilterByKeyword(UserProfile userProfile, Match match)
        {
            return string.IsNullOrEmpty(userProfile.LeagueKeyword) || StringHelper.Contain(match.LeagueEn, userProfile.LeagueKeyword);
        }
    }
}
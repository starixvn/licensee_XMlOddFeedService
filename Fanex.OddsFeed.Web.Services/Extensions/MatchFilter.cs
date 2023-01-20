namespace Fanex.OddsFeed.Web.Services.Extensions
{
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.OddsData;

    internal static class MatchFilter
    {
        public static bool FilterMatch(Match match, OddsFeedsModel model)
            => FilterBySport(model.UserProfile, model.UserQuery, match) &&
                    FilterByLeague(model.UserProfile, model.UserQuery, match) &&
                    FilterByCountry(model.UserProfile, model.UserQuery, match) &&
                    FilterByKeyword(model.UserProfile, match);

        public static bool FilterBetType(MatchOdds matchOdds, OddsFeedsModel model)
            => (matchOdds.Timestamp > model.Timestamp)
                && FilterByOddsProfile(model.UserProfile, model.IsSample, matchOdds)
                && FilterByOddsFeedFilter(model.UserQuery, matchOdds);

        private static bool FilterByOddsProfile(UserProfile userProfile, bool isSample, MatchOdds matchOdds)
            => isSample
                || userProfile.BetTypes == null
                || userProfile.BetTypes.Count == 0
                || userProfile.BetTypes.Contains(matchOdds.BetTypeId);

        private static bool FilterByOddsFeedFilter(UserQuery userQuery, MatchOdds matchOdds)
            => userQuery.BetTypes == null
                || userQuery.BetTypes.Count == 0
                || userQuery.BetTypes.Contains(matchOdds.BetTypeId);

        private static bool FilterBySport(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            var userProfileCondition = userProfile.Sports == null
                || userProfile.Sports.Count == 0
                || userProfile.Sports.Contains(match.SportId);
            var userQueryCondition = userQuery.Sports == null
                || userQuery.Sports.Count == 0
                || userQuery.Sports.Contains(match.SportId);
            return userProfileCondition && userQueryCondition;
        }

        private static bool FilterByLeague(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            var userProfileCondition = userProfile.Leagues == null
                || userProfile.Leagues.Count == 0
                || userProfile.Leagues.Contains(match.LeagueId);
            var userQueryCondition = userQuery.Leagues == null
                || userQuery.Leagues.Count == 0
                || StringHelper.ContainInList(userQuery.Leagues, match.LeagueEn);
            return userProfileCondition && userQueryCondition;
        }

        private static bool FilterByCountry(UserProfile userProfile, UserQuery userQuery, Match match)
        {
            var userProfileCondition = userProfile.Countries == null
                || userProfile.Countries.Count == 0
                || StringHelper.ContainInList(userProfile.Countries, match.CountryCode);
            var userQueryCondition = userQuery.Countries == null
                || userQuery.Countries.Count == 0
                || StringHelper.ContainInList(userQuery.Countries, match.CountryCode);
            return userProfileCondition && userQueryCondition;
        }

        private static bool FilterByKeyword(UserProfile userProfile, Match match)
            => string.IsNullOrEmpty(userProfile.LeagueKeyword)
                || StringHelper.Contain(match.LeagueEn, userProfile.LeagueKeyword);
    }
}
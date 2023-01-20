namespace Fanex.OddsFeed.Web.Services.Extensions
{
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Models.OddsData;
    using Fanex.OddsFeed.Models.Outright;

    internal static class OutrightFilter
    {
        public static bool FilterOutright(Outright outright, UserProfile userProfile, UserQuery userQuery)
        {
            var sportLeageCountryCondition = FilterBySport(userProfile, userQuery, outright) &&
                   FilterByLeague(userQuery, outright) &&
                   FilterByCountry(userProfile, userQuery, outright);

            return sportLeageCountryCondition &&
                   FilterByKeyWord(userProfile, outright) &&
                   FilterByProfile(userProfile, outright);
        }

        private static bool FilterBySport(UserProfile userProfile, UserQuery userQuery, Outright outright)
        {
            var userProfileCondition = userProfile.Sports == null || userProfile.Sports.Count == 0 || userProfile.Sports.Contains(outright.SportId);
            var userQueryCondition = userQuery.Sports == null || userQuery.Sports.Count == 0 || userQuery.Sports.Contains(outright.SportId);
            return userProfileCondition && userQueryCondition;
        }

        private static bool FilterByLeague(UserQuery userQuery, Outright outright)
            => userQuery.Leagues == null || userQuery.Leagues.Count == 0 || StringHelper.ContainInList(userQuery.Leagues, outright.EventEn);

        private static bool FilterByCountry(UserProfile userProfile, UserQuery userQuery, Outright outright)
        {
            var userProfileCondition = userProfile.Countries == null || userProfile.Countries.Count == 0 || StringHelper.ContainInList(userProfile.Countries, outright.CountryCode);
            var userQueryCondition = userQuery.Countries == null || userQuery.Countries.Count == 0 || StringHelper.ContainInList(userQuery.Countries, outright.CountryCode);
            return userProfileCondition && userQueryCondition;
        }

        private static bool FilterByProfile(UserProfile userProfile, Outright outright)
            => userProfile.Outrights == null || userProfile.Outrights.Count == 0 || userProfile.Outrights.Contains(outright.EventId);

        private static bool FilterByKeyWord(UserProfile userProfile, Outright outright)
            => string.IsNullOrEmpty(userProfile.OutrightsKeyword)
                || StringHelper.Contain(outright.EventEn, userProfile.OutrightsKeyword);
    }
}
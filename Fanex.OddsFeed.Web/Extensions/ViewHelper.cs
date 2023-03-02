namespace Fanex.OddsFeed.Web.Extensions
{
    using Fanex.OddsFeed.Models.DTO;

    public static class ViewHelper
    {
        public static string GetSportId(Sport sport)
        {
            const int sportId = 27;
            // return 27,50 for Cricket
            return sport.SportId == sportId ? "27,50" : sport.SportId.ToString();
        }

        public static string GetBetTypeName(IDictionary<int, BetType> betTypeList, int betTypeId)
        {
            if (betTypeList.ContainsKey(betTypeId))
            {
                return betTypeList[betTypeId].BetTypeName;
            }

            return "b" + betTypeId;
        }
    }
}
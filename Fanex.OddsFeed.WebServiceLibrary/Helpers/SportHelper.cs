using Fanex.OddsFeed.Common.DTO;

namespace Fanex.OddsFeed.WebServiceLibrary.Helpers
{
    public static class SportHelper
    {
        public static string GetSportId(Sport sport)
        {
            const int sportId = 27;
            // return 27,50 for Cricket
            return sport.SportId == sportId ? "27,50" : sport.SportId.ToString();
        }
    }
}
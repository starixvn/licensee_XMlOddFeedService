using Fanex.OddsFeed.Common.DTO;

namespace Fanex.OddsFeed.WebServiceLibrary.Helpers
{
    public static class SportHelper
    {
        public static string GetSportId(Sport sport)
        {
            // return 27,50 for Cricket
            return sport.SportId == 27 ? "27,50" : sport.SportId.ToString();
        }
    }
}

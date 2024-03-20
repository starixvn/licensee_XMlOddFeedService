namespace OddsFeed.Models.OddsData
{
    using System;
    using OddsFeed.Core;
    using OddsFeed.Models.DTO;

    public class OddsFeedsModel
    {
        public UserProfile UserProfile { get; set; }

        public UserQuery UserQuery { get; set; }

        public string BookieName { get; set; }

        public int Timestamp { get; set; }

        public bool IsSample { get; set; }

        public bool IsOutright { get; set; } = true;

        public string Username { get; set; }

        public DateTime? Date { get; set; }
        public string IP { get; set; }

        public bool IsShownOutright
            => UserProfile != null && UserProfile.OddsType.Contains(Constants.OddsTypeOutRight) && (!IsSample || IsOutright);

        public bool IsShownLiveMatch
            => UserProfile != null && UserProfile.OddsType.Contains(Constants.OddsTypeLiveMatch);

        public bool IsShownDeadMatch
            => UserProfile != null && UserProfile.OddsType.Contains(Constants.OddsTypeDeadBallMatch);
    }
}
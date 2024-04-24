namespace OddsFeed.Models.OddsData
{
    using System;
    using OddsFeed.Models.DTO;

    public class OddsFeedsModel
    {
        private const byte OddsTypeOutRight = 8;
        private const byte OddsTypeLiveMatch = 4;
        private const byte OddsTypeDeadBallMatch = 2;

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
            => UserProfile != null && UserProfile.OddsType.Contains(OddsTypeOutRight) && (!IsSample || IsOutright);

        public bool IsShownLiveMatch
            => UserProfile != null && UserProfile.OddsType.Contains(OddsTypeLiveMatch);

        public bool IsShownDeadMatch
            => UserProfile != null && UserProfile.OddsType.Contains(OddsTypeDeadBallMatch);
    }
}
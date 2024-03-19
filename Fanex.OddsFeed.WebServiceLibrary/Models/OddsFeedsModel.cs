namespace Fanex.OddsFeed.WebServiceLibrary.Models
{
    using System;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.DTO;

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
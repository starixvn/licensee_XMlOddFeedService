using System;

namespace Fanex.OddsFeed.Common.DTO
{
    [Serializable]
    public class CacheVersion
    {
        public int CurrentVersionOutright { get; set; }

        public int CurrentVersionLiveMatch { get; set; }

        public int CurrentVersionDeadballMatch { get; set; }
    }
}

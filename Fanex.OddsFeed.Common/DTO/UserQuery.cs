namespace Fanex.OddsFeed.Common.DTO
{
    using global::System.Collections.Generic;

    public class UserQuery
    {
        public IList<int> Sports { get; set; }

        public IList<string> Countries { get; set; }

        public IList<int> BetTypes { get; set; }

        public IList<string> Leagues { get; set; }

        public bool MoreTime { get; set; }
    }
}

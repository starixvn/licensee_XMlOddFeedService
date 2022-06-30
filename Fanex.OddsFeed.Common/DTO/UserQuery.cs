namespace Fanex.OddsFeed.Common.DTO
{
    using System.Collections.Generic;

    public class UserQuery
    {
        public IList<int> Sports { get; set; } = new List<int>();

        public IList<string> Countries { get; set; } = new List<string>();

        public IList<int> BetTypes { get; set; } = new List<int>();

        public IList<string> Leagues { get; set; } = new List<string>();

        public bool MoreTime { get; set; }
    }
}
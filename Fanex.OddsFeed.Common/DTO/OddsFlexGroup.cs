namespace Fanex.OddsFeed.Common.DTO
{
    using System.Collections.Generic;

    public class OddsFlexGroup
    {
        public int OddsId { get; set; }

        public IList<OddsFlex> OddsFlexs { get; set; }
    }
}

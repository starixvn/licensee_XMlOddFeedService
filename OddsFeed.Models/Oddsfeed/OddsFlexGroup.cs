namespace OddsFeed.Models.Oddsfeed
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class OddsFlexGroup
    {
        public int OddsId { get; set; }

        public IList<OddsFlex> OddsFlexs { get; set; }
    }
}
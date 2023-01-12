namespace Fanex.OddsFeed.AutoJobs.Models.Oddsfeed
{
    public class OddsFlexGroup
    {
        public int OddsId { get; set; }

        public IList<OddsFlex> OddsFlexs { get; set; }
    }
}
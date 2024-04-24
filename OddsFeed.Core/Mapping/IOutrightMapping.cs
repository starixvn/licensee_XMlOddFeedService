namespace OddsFeed.Core.Mapping
{
    using System.Collections.Generic;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Outright;

    public interface IOutrightMapping
    {
        List<Outright> GetOutrights(IEnumerable<OutrightOddsData> odds);
    }
}
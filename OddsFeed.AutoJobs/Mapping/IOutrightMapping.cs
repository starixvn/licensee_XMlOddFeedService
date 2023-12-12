namespace OddsFeed.AutoJobs.Mapping
{
    using System.Collections.Generic;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.Models.Outright;

    public interface IOutrightMapping
    {
        List<Outright> GetOutrights(IEnumerable<OutrightOddsData> odds);
    }
}
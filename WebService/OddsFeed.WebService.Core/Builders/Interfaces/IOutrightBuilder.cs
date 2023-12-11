namespace OddsFeed.WebService.Core.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Outright;

    public interface IOutrightBuilder
    {
        void AppendOutrightsNode(
            StringBuilder oddsFeedNode,
            IList<Outright> outrights,
            OddsFeedsModel model);
    }
}
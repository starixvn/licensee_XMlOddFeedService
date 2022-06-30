namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public interface IOutrightBuilder
    {
        void AppendOutrightsNode(
            StringBuilder oddsFeedNode,
            IList<Outright> outrights,
            OddsFeedsModel model);
    }
}
namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using Common.DTO;

    public interface IEventBuilder
    {
        void AppendOutrightsNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Outright> outrights);

        void AppendMatchesNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Match> matches);
    }
}

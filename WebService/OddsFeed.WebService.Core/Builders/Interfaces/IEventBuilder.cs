namespace OddsFeed.WebService.Core.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Outright;

    public interface IEventBuilder
    {
        void AppendOutrightsNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Outright> outrights);

        void AppendMatchesNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Match> matches);
    }
}

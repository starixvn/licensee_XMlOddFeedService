namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public interface IMatchBuilder
    {
        void AppendMatchNodes(
            StringBuilder oddsFeedNode,
            IList<Match> matches,
            OddsFeedsModel model);

        void AppendMatchResultNodes(
            StringBuilder xmlnode,
            IList<Match> matches,
            OddsFeedsModel model);

        UserLanguage GetUserLanguage(UserProfile model);
    }
}
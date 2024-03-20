namespace OddsFeed.WebService.Core.Builders
{
    using System.Collections.Generic;
    using System.Text;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;

    public interface IMatchBuilder
    {
        void AppendMatchNodes(
            StringBuilder oddsFeedNode,
            IList<Match> matches,
            OddsFeedsModel model,
            bool isLiveMatch = false);

        void AppendMatchResultNodes(
            StringBuilder xmlnode,
            IList<Match> matches,
            OddsFeedsModel model);

        UserLanguage GetUserLanguage(UserProfile model);
    }
}
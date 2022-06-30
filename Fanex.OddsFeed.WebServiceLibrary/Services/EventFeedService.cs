namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Linq;
    using System.Text;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Helpers;
    using Fanex.OddsFeed.WebServiceLibrary.Builders;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public class EventFeedService : IEventFeedService
    {
        private readonly UserProfile userProfile;
        private readonly EventBuilder eventBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;

        public EventFeedService(UserProfile userProfile)
        {
            this.userProfile = userProfile;
            eventBuilder = new EventBuilder();
            feedCachingOperator = new FeedCachingOperator();
        }

        public string BuildXmlEvent(bool isSample = false, bool isOutright = true)
        {
            var eventFeedNode = new StringBuilder();
            var data = feedCachingOperator.GetOddsData(new OddsFeedsModel { UserProfile = userProfile }, isOutright, true, true);
            var fullMatches = data.DeadballMatches.Concat(data.LiveMatches);

            var userLanguage = new UserLanguage
            {
                IsSupportedEnglish = StringHelper.ContainInList(userProfile.Languages, "en-US"),
                IsSupportedTraditionalChinese = StringHelper.ContainInList(userProfile.Languages, "zh-TW"),
                IsSupportedSimplifiedChinese = StringHelper.ContainInList(userProfile.Languages, "zh-CN"),
            };

            eventFeedNode.Append("<eventfeed>");

            if (!isSample || isOutright)
            {
                eventBuilder.AppendOutrightsNode(eventFeedNode, userLanguage, data.Outrights);
            }

            eventBuilder.AppendMatchesNode(eventFeedNode, userLanguage, fullMatches);

            eventFeedNode.Append("</eventfeed>");

            return eventFeedNode.ToString();
        }
    }
}
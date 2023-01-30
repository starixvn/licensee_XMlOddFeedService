namespace Fanex.OddsFeed.Web.Services.Services
{
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Models.OddsData;
    using Fanex.OddsFeed.Web.Services.Builders;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

    public class EventFeedService : IEventFeedService
    {
        private readonly IFeedCachingOperator feedCachingOperator;

        public EventFeedService(IFeedCachingOperator feedCachingOperator)
        {
            this.feedCachingOperator = feedCachingOperator;
        }

        public async Task<string> BuildXmlEvent(UserProfile userProfile, bool isSample = false, bool isOutright = true)
        {
            var eventFeedNode = new StringBuilder();
            var data = await feedCachingOperator.GetOddsData(new OddsFeedsModel { UserProfile = userProfile }, isOutright, true, true);
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
                EventBuilder.AppendOutrightsNode(eventFeedNode, userLanguage, data.Outrights);
            }

            EventBuilder.AppendMatchesNode(eventFeedNode, userLanguage, fullMatches);

            eventFeedNode.Append("</eventfeed>");

            return eventFeedNode.ToString();
        }
    }
}
namespace OddsFeed.WebService.Services
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.OddsData;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Services.Interfaces;

    public class EventFeedService : IEventFeedService
    {
        private readonly IEventBuilder eventBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;

        public EventFeedService(
            IEventBuilder eventBuilder,
            IFeedCachingOperator feedCachingOperator)
        {
            this.eventBuilder = eventBuilder;
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
                eventBuilder.AppendOutrightsNode(eventFeedNode, userLanguage, data.Outrights);
            }

            eventBuilder.AppendMatchesNode(eventFeedNode, userLanguage, fullMatches);

            eventFeedNode.Append("</eventfeed>");

            return eventFeedNode.ToString();
        }
    }
}
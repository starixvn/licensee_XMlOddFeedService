namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Linq;
    using System.Text;
    using Builders;
    using Caching;
    using Common.DTO;
    using Common.Helpers;
    using Models;

    public class EventFeedService : IEventFeedService
    {
        private readonly UserProfile _userProfile;
        private readonly EventBuilder _eventBuilder;
        private readonly IOddsFeedCachingOperator _oddsFeedCachingOperator;

        public EventFeedService(UserProfile userProfile)
        {
            _userProfile = userProfile;
            _eventBuilder = new EventBuilder();
            _oddsFeedCachingOperator = new OddsFeedCachingOperator();
        }

        public string BuildXmlEvent(bool isSample = false, bool isOutright = true)
        {
            var eventFeedNode = new StringBuilder();
            var data = _oddsFeedCachingOperator.GetOddsData(new OddsFeedsModel { UserProfile = _userProfile }, isOutright, true, true);
            var fullMatches = data.DeadballMatches.Concat(data.LiveMatches);

            var userLanguage = new UserLanguage
            {
                IsSupportedEnglish = StringHelper.ContainInList(_userProfile.Languages, "en-US"),
                IsSupportedTraditionalChinese = StringHelper.ContainInList(_userProfile.Languages, "zh-TW"),
                IsSupportedSimplifiedChinese = StringHelper.ContainInList(_userProfile.Languages, "zh-CN"),
            };

            eventFeedNode.Append("<eventfeed>");

            if (!isSample || isOutright)
            {
                _eventBuilder.AppendOutrightsNode(eventFeedNode, userLanguage, data.Outrights);
            }

            _eventBuilder.AppendMatchesNode(eventFeedNode, userLanguage, fullMatches);

            eventFeedNode.Append("</eventfeed>");

            return eventFeedNode.ToString();
        }
    }
}
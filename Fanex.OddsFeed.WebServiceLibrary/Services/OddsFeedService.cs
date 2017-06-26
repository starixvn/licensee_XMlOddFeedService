namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Builders;
    using Caching;
    using Common.DTO;
    using Common.Helpers;
    using Filters;
    using Models;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchBuilder _matchBuilder;
        private readonly IOutrightBuilder _outrightBuilder;
        private readonly IOddsFeedCachingOperator _oddsFeedCachingOperator;
        private readonly IMatchFilter _matchFilter;

        public OddsFeedService()
        {
            _matchBuilder = new MatchBuilder();
            _outrightBuilder = new OutrightBuilder();
            _oddsFeedCachingOperator = new OddsFeedCachingOperator();
            _matchFilter = new MatchFilter();
        }

        public string BuildXmlDocument(OddsFeedsModel model)
        {
            var isShownOutright = IsShownOutright(model);
            var isShownLiveMatch = IsShownLiveMatch(model.UserProfile);
            var isShownDeadMatch = IsShownDeadBallMatch(model.UserProfile);

            var oddsData = _oddsFeedCachingOperator.GetOddsData(model, isShownOutright, isShownLiveMatch, isShownDeadMatch);
            var oddsFeedNode = new StringBuilder("<oddsfeed>");
            var userLanguage = GetUserLanguage(model);

            if (isShownOutright)
            {
                _outrightBuilder.AppendOutrightsNode(oddsFeedNode, oddsData.Outrights, model, userLanguage);
            }

            AppendMatches(oddsFeedNode, oddsData.LiveMatches, oddsData.DeadballMatches, model, userLanguage);

            oddsFeedNode.Append("</oddsfeed>");

            return oddsFeedNode.ToString();
        }

        private static UserLanguage GetUserLanguage(OddsFeedsModel model)
        {
            return new UserLanguage
            {
                IsSupportedEnglish = StringHelper.ContainInList(model.UserProfile.Languages, "en-US"),
                IsSupportedTraditionalChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-TW"),
                IsSupportedSimplifiedChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-CN")
            };
        }

        private void AppendMatches(
            StringBuilder oddsFeedNode,
            IList<Match> liveMatches,
            IList<Match> deadMatches,
            OddsFeedsModel model,
            UserLanguage userLanguage)
        {
            AppendBeginMatches(oddsFeedNode, model, GetMatchTimeStamp(model, liveMatches, deadMatches), GetLastUpdate());

            _matchBuilder.AppendMatchNodes(oddsFeedNode, liveMatches, userLanguage, model);
            _matchBuilder.AppendMatchNodes(oddsFeedNode, deadMatches, userLanguage, model);

            oddsFeedNode.Append("</matches>");
        }

        private int GetMatchTimeStamp(OddsFeedsModel model, IList<Match> liveMatches, IList<Match> deadMatches)
        {
            var liveMatchesTimeStamp = 0;
            var deadMatchesTimeStamp = 0;

            if (liveMatches.Any())
            {
                liveMatchesTimeStamp = GetMaxTimeStampMatches(model, liveMatches);
            }

            if (deadMatches.Any())
            {
                deadMatchesTimeStamp = GetMaxTimeStampMatches(model, deadMatches);
            }

            return Math.Max(liveMatchesTimeStamp, deadMatchesTimeStamp);
        }

        private int GetMaxTimeStampMatches(OddsFeedsModel model, IList<Match> matches)
        {
            var matchFilter = matches.Where(m => _matchFilter.FilterMatch(m, model));

            return matchFilter.Any() ? matchFilter.Max(m => GetMaxTimeStampOdds(model, m.MatchOddsList)) : 0;
        }

        private int GetMaxTimeStampOdds(OddsFeedsModel model, IList<MatchOdds> matchOddsList)
        {
            var matchOddsFilter = matchOddsList.Where(o => _matchFilter.FilterBetType(o, model));

            return matchOddsFilter.Any() ? matchOddsFilter.Max(o => o.TimeStamp) : 0;
        }

        private static bool IsShownOutright(OddsFeedsModel model)
        {
            return model.UserProfile.OddsType.Contains(OddsTypeOutRight) && (!model.IsSample || model.IsOutright);
        }

        private static bool IsShownLiveMatch(UserProfile userProfile)
        {
            return userProfile.OddsType != null && userProfile.OddsType.Contains(OddsTypeLiveMatch);
        }

        private static bool IsShownDeadBallMatch(UserProfile userProfile)
        {
            return userProfile.OddsType != null && userProfile.OddsType.Contains(OddsTypeDeadBallMatch);
        }

        private static DateTime GetLastUpdate()
        {
            return DateTime.Now.AddHours(12);
        }

        private static void AppendBeginMatches(
            StringBuilder oddsFeedNode,
            OddsFeedsModel model,
            int matchesTimeStamp,
            DateTime lastUpdate)
        {
            if (matchesTimeStamp > 0)
            {
                oddsFeedNode.Append($"<matches lastupdate=\"{lastUpdate}\" timestamp=\"{matchesTimeStamp}\" oddstype=\"{GetOddsTypeName(model.UserProfile.OddRegion)}\">");
            }
            else
            {
                oddsFeedNode.Append("<matches>");
            }
        }

        private static string GetOddsTypeName(int oddsTypeId)
        {
            var oddsTypeName = string.Empty;

            switch (oddsTypeId)
            {
                case 2:
                    oddsTypeName = "HongKong";
                    break;

                case 3:
                    oddsTypeName = "Decimal";
                    break;

                case 4:
                    oddsTypeName = "Indo";
                    break;

                case 5:
                    oddsTypeName = "US";
                    break;

                default:
                    oddsTypeName = "Malay";
                    break;
            }

            return oddsTypeName;
        }
    }
}
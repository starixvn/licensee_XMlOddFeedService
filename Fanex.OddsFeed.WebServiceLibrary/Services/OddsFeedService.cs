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
    using NLog;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchBuilder _matchBuilder;
        private readonly IOutrightBuilder _outrightBuilder;
        private readonly IOddsFeedCachingOperator _oddsFeedCachingOperator;
        private readonly IMatchFilter _matchFilter;
        private readonly Logger fileLogger = LogManager.GetCurrentClassLogger();

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

            if (model.Username == "1_12_09")
            {
                WriteLogInfo("LiveMatches rows: " + oddsData.LiveMatches.Count);
                WriteLogInfo("DeadballMatches rows: " + oddsData.DeadballMatches.Count);
                WriteLogInfo("Outrights rows: " + oddsData.Outrights.Count);
            }

            return oddsFeedNode.ToString();
        }

        private static UserLanguage GetUserLanguage(OddsFeedsModel model)
        {
            return new UserLanguage
            {
                IsSupportedEnglish = StringHelper.ContainInList(model.UserProfile.Languages, "en-US"),
                IsSupportedTraditionalChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-TW"),
                IsSupportedSimplifiedChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-CN"),
                IsSupportedIndonesia = StringHelper.ContainInList(model.UserProfile.Languages, "id-ID"),
                IsSupportedThai = StringHelper.ContainInList(model.UserProfile.Languages, "th-TH"),
                IsSupportedKorean = StringHelper.ContainInList(model.UserProfile.Languages, "ko-KR"),
                IsSupportedVietnamese = StringHelper.ContainInList(model.UserProfile.Languages, "vi-VN"),
                IsSupportedJapanese = StringHelper.ContainInList(model.UserProfile.Languages, "ja-JP"),
                IsSupportedHindi = StringHelper.ContainInList(model.UserProfile.Languages, "hi-IN")
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
            const int addHour = 12;
            return DateTime.Now.AddHours(addHour);
        }

        private static void AppendBeginMatches(
            StringBuilder oddsFeedNode,
            OddsFeedsModel model,
            int matchesTimeStamp,
            DateTime lastUpdate)
        {
            oddsFeedNode.Append(matchesTimeStamp > 0 ?
                $"<matches lastupdate=\"{lastUpdate}\" timestamp=\"{matchesTimeStamp}\" oddstype=\"{GetOddsTypeName(model.UserProfile.OddRegion)}\">" :
                "<matches>");
        }

        private static string GetOddsTypeName(int oddsTypeId)
        {
            const int hongKongTypeName = 2;
            const int decimalTypeName = 3;
            const int indoTypeName = 4;
            const int usTypeName = 5;
            var oddsTypeName = string.Empty;

            switch (oddsTypeId)
            {
                case hongKongTypeName:
                    oddsTypeName = "HongKong";
                    break;

                case decimalTypeName:
                    oddsTypeName = "Decimal";
                    break;

                case indoTypeName:
                    oddsTypeName = "Indo";
                    break;

                case usTypeName:
                    oddsTypeName = "US";
                    break;

                default:
                    oddsTypeName = "Malay";
                    break;
            }

            return oddsTypeName;
        }

        private void WriteLogInfo(string message)
        {
            fileLogger.Info(message);
        }
    }
}
namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Helpers;
    using Fanex.OddsFeed.WebServiceLibrary.Builders;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using Fanex.OddsFeed.WebServiceLibrary.Criteria;
    using Fanex.OddsFeed.WebServiceLibrary.Filters;
    using Fanex.OddsFeed.WebServiceLibrary.Models;
    using Fanex.OddsFeed.WebServiceLibrary.Repositories;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchBuilder matchBuilder;
        private readonly IOutrightBuilder outrightBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IMatchFilter matchFilter;
        private readonly IOddsDataService oddsDataService;
        private readonly IUserRepository userRepository;
        private readonly IPartnerRepository partnerRepository;

        public OddsFeedService()
        {
            matchBuilder = new MatchBuilder();
            outrightBuilder = new OutrightBuilder();
            feedCachingOperator = new FeedCachingOperator();
            matchFilter = new MatchFilter();
            oddsDataService = new OddsDataService();
            userRepository = new UserRepository();
            partnerRepository = new PartnerRepository();
        }

        public string BuildXmlDocument(OddsFeedsModel model)
        {
            var startTime = DateTimeOffset.Now.DateTime;
            var isShownOutright = IsShownOutright(model);
            var isShownLiveMatch = IsShownLiveMatch(model.UserProfile);
            var isShownDeadMatch = IsShownDeadBallMatch(model.UserProfile);
            OddsData oddsData = feedCachingOperator.GetOddsData(model, isShownOutright, isShownLiveMatch, isShownDeadMatch);

            if (StringHelper.Equals(model.Username, Constants.BetRadarUsername))
            {
                if (isShownLiveMatch)
                {
                    oddsData.LiveMatches = GetLiveMatchesForBetRadar(oddsData.LiveMatches);
                }

                if (isShownDeadMatch)
                {
                    oddsData.DeadballMatches = oddsData.DeadballMatches.Where(x => !x.LeagueEn.ToLowerInvariant().Contains("fantasy")).ToList();
                }
            }

            var oddsFeedNode = new StringBuilder("<oddsfeed>");

            if (isShownOutright)
            {
                outrightBuilder.AppendOutrightsNode(oddsFeedNode, oddsData.Outrights, model);
            }

            AppendMatches(oddsFeedNode, oddsData.LiveMatches, oddsData.DeadballMatches, model);

            oddsFeedNode.Append("</oddsfeed>");

            if (StringHelper.Equals(model.Username, Constants.BetRadarUsername))
            {
                WriteLogRequestOdds(model, oddsData, startTime);
            }

            return oddsFeedNode.ToString();
        }

        private IList<Match> GetLiveMatchesForBetRadar(IList<Match> liveMatches)
        {
            var sports = userRepository.GetSports(Constants.BetRadarUsername);
            var sportIds = string.Join(",", sports.Select(x => x.SportId));
            var matches = oddsDataService.GetLiveMatchesForBetRadar(sportIds);
            foreach (var match in matches)
            {
                if (!liveMatches.Any(x => x.MatchId == match.MatchId))
                {
                    liveMatches.Add(match);
                }
            }

            liveMatches = liveMatches.Where(x => !x.LeagueEn.ToLowerInvariant().Contains("fantasy")).ToList();

            return liveMatches.OrderBy(x => x.MatchId).ToList();
        }

        private void AppendMatches(
            StringBuilder oddsFeedNode,
            IList<Match> liveMatches,
            IList<Match> deadMatches,
            OddsFeedsModel model)
        {
            AppendBeginMatches(oddsFeedNode, model, GetMatchTimestamp(model, liveMatches, deadMatches), GetLastUpdate());

            matchBuilder.AppendMatchNodes(oddsFeedNode, liveMatches, model, true);
            matchBuilder.AppendMatchNodes(oddsFeedNode, deadMatches, model);

            oddsFeedNode.Append("</matches>");
        }

        private int GetMatchTimestamp(OddsFeedsModel model, IList<Match> liveMatches, IList<Match> deadMatches)
        {
            var liveMatchesTimestamp = 0;
            var deadMatchesTimestamp = 0;

            if (liveMatches.Any())
            {
                liveMatchesTimestamp = GetMaxTimestampMatches(model, liveMatches);
            }

            if (deadMatches.Any())
            {
                deadMatchesTimestamp = GetMaxTimestampMatches(model, deadMatches);
            }

            return Math.Max(liveMatchesTimestamp, deadMatchesTimestamp);
        }

        private int GetMaxTimestampMatches(OddsFeedsModel model, IList<Match> matches)
        {
            var tempMatches = matches.Where(m => matchFilter.FilterMatch(m, model));

            return tempMatches.Any() ? tempMatches.Max(m => GetMaxTimestampOdds(model, m.MatchOddsList)) : 0;
        }

        private int GetMaxTimestampOdds(OddsFeedsModel model, IList<MatchOdds> matchOddsList)
        {
            var matchOddsFilter = matchOddsList.Where(o => matchFilter.FilterBetType(o, model));

            return matchOddsFilter.Any() ? matchOddsFilter.Max(o => o.Timestamp) : 0;
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
            int matchesTimestamp,
            DateTime lastUpdate)
        {
            if (matchesTimestamp > 0)
            {
                var oddsType = GetOddsTypeName(model.UserProfile.OddRegion);
                oddsFeedNode.AppendFormat("<matches lastupdate=\"{0}\" timestamp=\"{1}\"{2}>",
                    lastUpdate,
                    matchesTimestamp,
                    !string.IsNullOrWhiteSpace(oddsType) ? $" oddstype=\"{GetOddsTypeName(model.UserProfile.OddRegion)}\"" : string.Empty);
            }
            else
            {
                oddsFeedNode.Append("<matches>");
            }
        }

        private static string GetOddsTypeName(int oddsTypeId)
        {
            const byte MalayOddsType = 1;
            const byte HongKongOddsType = 2;
            const byte DecimalOddsType = 3;
            const byte IndoOddsType = 4;
            const byte USOddsType = 5;
            string oddsTypeName;

            switch (oddsTypeId)
            {
                case MalayOddsType:
                    oddsTypeName = "Malay";
                    break;

                case HongKongOddsType:
                    oddsTypeName = "HongKong";
                    break;

                case DecimalOddsType:
                    oddsTypeName = "Decimal";
                    break;

                case IndoOddsType:
                    oddsTypeName = "Indo";
                    break;

                case USOddsType:
                    oddsTypeName = "US";
                    break;

                default:
                    oddsTypeName = string.Empty;
                    break;
            }

            return oddsTypeName;
        }

        private void WriteLogRequestOdds(OddsFeedsModel model, OddsData oddsData, DateTime startTime)
        {
            var matchIds = string.Join(",", oddsData.LiveMatches.Select(x => x.MatchId));
            var endTime = DateTimeOffset.Now.DateTime;
            var duration = (endTime - startTime).TotalSeconds;
            var criteria = new WriteLogRequestOddsCriteria
            {
                Username = model.Username,
                MatchIds = matchIds,
                IP = model.IP,
                Duration = (int)duration,
                ExecutionTime = startTime
            };
            var thread = new Thread(() => WriteLogRequestOdds(criteria));
            thread.Start();
        }

        private void WriteLogRequestOdds(WriteLogRequestOddsCriteria criteria)
        {
            try
            {
                partnerRepository.WriteLogRequestOdds(criteria);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }
    }
}
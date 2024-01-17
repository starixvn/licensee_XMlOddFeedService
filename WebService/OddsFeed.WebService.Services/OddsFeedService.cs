namespace OddsFeed.WebService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Services.Mapping;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Core.Filters;
    using OddsFeed.WebService.Criteria;
    using OddsFeed.WebService.Services.Interfaces;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchMapping matchMapping;
        private readonly IMatchBuilder matchBuilder;
        private readonly IOutrightBuilder outrightBuilder;
        private readonly IMatchFilter matchFilter;
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IBaseService baseService;

        public OddsFeedService(
            IMatchMapping matchMapping,
            IMatchBuilder matchBuilder,
            IOutrightBuilder outrightBuilder,
            IMatchFilter matchFilter,
            IFeedCachingOperator feedCachingOperator,
            IBaseService baseService)
        {
            this.matchMapping = matchMapping;
            this.matchBuilder = matchBuilder;
            this.outrightBuilder = outrightBuilder;
            this.matchFilter = matchFilter;
            this.feedCachingOperator = feedCachingOperator;
            this.baseService = baseService;
        }

        public async Task<string> BuildXmlDocument(OddsFeedsModel model)
        {
            var isShownOutright = IsShownOutright(model);
            var isShownLiveMatch = IsShownLiveMatch(model.UserProfile);
            var isShownDeadMatch = IsShownDeadBallMatch(model.UserProfile);

            var oddsData = await feedCachingOperator.GetOddsData(model, isShownOutright, isShownLiveMatch, isShownDeadMatch);
            if (!string.IsNullOrWhiteSpace(model.Username) && model.Username.Equals("betradar", StringComparison.CurrentCultureIgnoreCase) && isShownLiveMatch)
            {
                oddsData.LiveMatches = await GetLiveMatchesForBetRadarAsync(oddsData.LiveMatches);
            }

            var oddsFeedNode = new StringBuilder("<oddsfeed>");

            if (isShownOutright)
            {
                outrightBuilder.AppendOutrightsNode(oddsFeedNode, oddsData.Outrights, model);
            }

            AppendMatches(oddsFeedNode, oddsData.LiveMatches, oddsData.DeadballMatches, model);

            oddsFeedNode.Append("</oddsfeed>");

            return oddsFeedNode.ToString();
        }

        public async Task<IEnumerable<Country>> GetCountryListAsync()
            => await baseService.GetDataAsync<Country>(new GetCountryListCriteria());
        
        private async Task<IList<Match>> GetLiveMatchesForBetRadarAsync(IList<Match> liveMatches)
        {
            var criteria = new GetLiveOddsForBetRadarCriteria();
            var matchOdds = await baseService.GetDataAsync<MatchOddsData>(criteria);
            foreach(var matchOdd in matchOdds)
            {
                if (!liveMatches.Any(x => x.MatchId == matchOdd.MatchID))
                {
                    var match = matchMapping.CreateMatch(matchOdd, false);
                    match.MatchOddsList = new List<MatchOdds>();
                    liveMatches.Add(match);
                }
            }

            return liveMatches.OrderBy(x => x.MatchId).ToList();
        }

        private void AppendMatches(
            StringBuilder oddsFeedNode,
            IList<Match> liveMatches,
            IList<Match> deadMatches,
            OddsFeedsModel model)
        {
            AppendBeginMatches(oddsFeedNode, model, GetMatchTimestamp(model, liveMatches, deadMatches), GetLastUpdate());

            matchBuilder.AppendMatchNodes(oddsFeedNode, liveMatches, model);
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
            const byte AddHour = 12;
            return DateTimeOffset.Now.AddHours(AddHour).DateTime;
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
    }
}
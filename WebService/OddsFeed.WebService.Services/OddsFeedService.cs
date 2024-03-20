namespace OddsFeed.WebService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Core;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Services.Mapping;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Core.Filters;
    using OddsFeed.WebService.Criteria;
    using OddsFeed.WebService.Extensions;

    public class OddsFeedService : IOddsFeedService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMatchMapping matchMapping;
        private readonly IMatchBuilder matchBuilder;
        private readonly IOutrightBuilder outrightBuilder;
        private readonly IMatchFilter matchFilter;
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IBaseService baseService;
        private readonly ILogServiceProvider logServiceProvider;

        public OddsFeedService(IServiceProvider serviceProvider)
        {
            dynamicRepository = serviceProvider.GetRequiredService<IDynamicRepository>();
            matchMapping = serviceProvider.GetRequiredService<IMatchMapping>();
            matchBuilder = serviceProvider.GetRequiredService<IMatchBuilder>();
            outrightBuilder = serviceProvider.GetRequiredService<IOutrightBuilder>();
            matchFilter = serviceProvider.GetRequiredService<IMatchFilter>();
            feedCachingOperator = serviceProvider.GetRequiredService<IFeedCachingOperator>();
            baseService = serviceProvider.GetRequiredService<IBaseService>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
        }

        public async Task<string> BuildXmlDocumentAsync(OddsFeedsModel model)
        {
            var startTime = DateTimeOffset.Now.DateTime;

            var oddsData = await feedCachingOperator.GetOddsDataAsync(model, model.IsShownOutright, model.IsShownLiveMatch, model.IsShownDeadMatch);
            if (StringHelper.Equals(model.Username, Constants.BetRadarUsername))
            {
                if(model.IsShownLiveMatch)
                {
                    oddsData.LiveMatches = await GetLiveMatchesForBetRadarAsync(oddsData.LiveMatches);
                }

                if (model.IsShownDeadMatch)
                {
                    oddsData.DeadballMatches = ExcludeFantasyLeague(oddsData.DeadballMatches).ToList();
                }
            }

            var oddsFeedNode = new StringBuilder("<oddsfeed>");

            if (model.IsShownOutright)
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

        public async Task<IEnumerable<Country>> GetCountryListAsync()
            => await baseService.GetDataAsync<Country>(new GetCountryListCriteria());
        
        private async Task<IList<Match>> GetLiveMatchesForBetRadarAsync(IList<Match> liveMatches)
        {
            var sports = await GetSportAsync(Constants.BetRadarUsername);
            var sportIds = string.Join(",", sports.Select(x => x.SportId));
            var criteria = new GetLiveOddsForBetRadarCriteria
            {
                SportIDList = sportIds
            };
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

            return ExcludeFantasyLeague(liveMatches).OrderBy(x => x.MatchId).ToList();
        }

        private async Task<IEnumerable<Sport>> GetSportAsync(string username)
        {
            var criteria = new GetSportByUserNameCriteria
            {
                Username = username
            };
            var sports = await baseService.GetDataAsync<Sport>(criteria);
            return sports.Where(s => s.SportId != Constants.CricketSportTypeId);
        }

        private static IList<Match> ExcludeFantasyLeague(IList<Match> matches)
            => matches.Where(x => !x.LeagueEn.Contains("fantasy", StringComparison.InvariantCultureIgnoreCase)).ToList();

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

        private void WriteLogRequestOdds(OddsFeedsModel model, OddsData oddsData, DateTime startTime)
        {
            var matchIds = new List<int>();

            if (model.IsShownLiveMatch)
            {
                var liveMatches = oddsData.LiveMatches.Where(m => matchFilter.FilterMatch(m, model)).Select(m => m.MatchId).ToList();
                matchIds.AddRange(liveMatches);
            }

            if (model.IsShownDeadMatch)
            {
                var deadballMatches = oddsData.DeadballMatches.Where(m => matchFilter.FilterMatch(m, model)
                    && m.MatchOddsList.Any(odds => matchFilter.FilterBetType(odds, model))).Select(m => m.MatchId).ToList();
                matchIds.AddRange(deadballMatches);
            }

            var endTime = DateTimeOffset.Now.DateTime;
            var duration = (endTime - startTime).TotalSeconds;
            var criteria = new WriteLogRequestOddsCriteria
            {
                Username = model.Username,
                MatchIds = string.Join(",", matchIds),
                IP = model.IP,
                Duration = (int)duration,
                ExecutionTime = startTime
            };
            var thread = new Thread(async () => await WriteLogRequestOddsAsync(criteria));
            thread.Start();
        }

        private async Task WriteLogRequestOddsAsync(WriteLogRequestOddsCriteria criteria)
        {
            try
            {
                await dynamicRepository.ExecuteAsync(criteria);
            }
            catch (Exception ex)
            {
                await logServiceProvider.LogSentryAsync(ex.Message, ex);
            }
        }
    }
}
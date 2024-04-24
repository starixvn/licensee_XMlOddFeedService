namespace OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.AutoJobs.Criteria.OddsData;
    using OddsFeed.Core.Mapping;
    using OddsFeed.Core.Services;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Oddsfeed;
    using OddsFeed.Models.Outright;

    public class OddsDataService : IOddsDataService
    {
        private readonly IBaseService baseService;
        private readonly IMatchMapping matchMapping;
        private readonly IOutrightMapping outrightMapping;

        public OddsDataService(
            IBaseService baseService,
            IMatchMapping matchMapping,
            IOutrightMapping outrightMapping)
        {
            this.baseService = baseService;
            this.matchMapping = matchMapping;
            this.outrightMapping = outrightMapping;
        }

        public async Task<IList<Match>> GetDeadballMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (deadballOdds, oddFlexes) = await baseService.FetchMultipleAsync<MatchOddsData, OddsFlex>(new GetDeadballOddsDataCriteria());
            var oddsFlexGroups = GetOddsFlexGroups(oddFlexes);
            var matches = matchMapping.GetMatches(deadballOdds, betTypeSchemas, oddsFlexGroups, false);
            return matches;
        }

        public async Task<IList<Match>> GetLiveMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (liveOdds, oddFlexes) = await baseService.FetchMultipleAsync<MatchOddsData, OddsFlex>(new GetLiveOddDataCriteria());
            var oddsFlexGroups = GetOddsFlexGroups(oddFlexes);
            var matches = matchMapping.GetMatches(liveOdds, betTypeSchemas, oddsFlexGroups, false);

            return matches;
        }

        public async Task<IList<Outright>> GetOutrightAsync()
        {
            var odds = await baseService.GetDataAsync<OutrightOddsData>(new GetOutrightOddsDataCriteria());
            var outrights = outrightMapping.GetOutrights(odds);
            return outrights;
        }

        private static IEnumerable<OddsFlexGroup> GetOddsFlexGroups(IEnumerable<OddsFlex> oddFlexes)
        {
            var oddsFlexGroups = new List<OddsFlexGroup>();

            foreach (var oddsFlex in oddFlexes)
            {
                var oddsFlexGroup = oddsFlexGroups.Find(odds => odds.OddsId == oddsFlex.OddsId);
                if (oddsFlexGroup == null)
                {
                    oddsFlexGroups.Add(new OddsFlexGroup
                    {
                        OddsId = oddsFlex.OddsId,
                        OddsFlexs = new List<OddsFlex> { oddsFlex }
                    });
                }
                else
                {
                    oddsFlexGroup.OddsFlexs.Add(oddsFlex);
                }
            }

            return oddsFlexGroups;
        }
    }
}
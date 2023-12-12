namespace OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.AutoJobs.Criteria.OddsData;
    using OddsFeed.AutoJobs.Helpers;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.Core.Services;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Outright;
    using OddsFeed.Repository;

    public class OddsDataService : IOddsDataService
    {
        private readonly IDbAccessRepository dbAccessRepository;
        private readonly IBaseService baseService;

        public OddsDataService(
            IDbAccessRepository dbAccessRepository,
            IBaseService baseService)
        {
            this.dbAccessRepository = dbAccessRepository;
            this.baseService = baseService;
        }

        public async Task<IList<Match>> GetDeadballMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (deadballOdds, oddFlexes) = await dbAccessRepository.GetDeadballMatchAsync(new GetDeadballOddsDataCriteria());
            var matches = MappingHelper.MappingMatches(deadballOdds, betTypeSchemas, oddFlexes, false);
            return matches;
        }

        public async Task<IList<Match>> GetLiveMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (dataTable, oddFlexes) = await dbAccessRepository.GetLiveMatchAsync(new GetLiveOddDataCriteria());
            var matches = MappingHelper.MappingMatches(dataTable, betTypeSchemas, oddFlexes, false);

            return matches;
        }

        public async Task<IList<Outright>> GetOutrightAsync()
        {
            var odds = await baseService.GetDataAsync<OutrightOddsData>(new GetOutrightOddsDataCriteria());
            var outrights = MappingHelper.MappingOutrights(odds);
            return outrights;
        }
    }
}
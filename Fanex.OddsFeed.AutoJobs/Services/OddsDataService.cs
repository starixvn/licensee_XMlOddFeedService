namespace Fanex.OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.AutoJobs.Criteria.OddsData;
    using Fanex.OddsFeed.AutoJobs.Helpers;
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Outright;
    using Fanex.OddsFeed.Repository;

    public class OddsDataService : IOddsDataService
    {
        private readonly IDbAccessRepository dbAccessRepository;

        public OddsDataService(IDbAccessRepository dbAccessRepository)
        {
            this.dbAccessRepository = dbAccessRepository;
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
            var dataSet = await dbAccessRepository.GetDataTableAsync(new GetOutrightOddsDataCriteria());
            var outrights = MappingHelper.MappingOutrights(dataSet);
            return outrights;
        }
    }
}
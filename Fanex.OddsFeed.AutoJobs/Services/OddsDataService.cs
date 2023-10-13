namespace Fanex.OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.AutoJobs.Criteria.OddsData;
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Outright;
    using Fanex.OddsFeed.Repository.Helpers;
    using Fanex.OddsFeed.Repository.Interfaces;

    public class OddsDataService : IOddsDataService
    {
        private readonly IDbAccessRepository dbAccessRepository;

        public OddsDataService(IDbAccessRepository dbAccessRepository)
        {
            this.dbAccessRepository = dbAccessRepository;
        }

        public async Task<IList<Match>> GetDeadballMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (deadballOdds, oddFlex) = await dbAccessRepository.GetDeadballMatches(new GetDeadballOddsDataCriteria());
            var matches = MappingHelper.MappingMatches(deadballOdds, betTypeSchemas, oddFlex, false);
            return matches;
        }

        public async Task<IList<Match>> GetLiveMatchAsync(IList<BetTypeSchema> betTypeSchemas)
        {
            var (dataTable, oddFlex) = await dbAccessRepository.GetLiveMatches(new GetLiveOddDataCriteria());
            var matches = MappingHelper.MappingMatches(dataTable, betTypeSchemas, oddFlex, false);

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
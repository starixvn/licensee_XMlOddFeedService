namespace Fanex.OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.AutoJobs.Criteria.OddsData;
    using Fanex.OddsFeed.AutoJobs.Models;
    using Fanex.OddsFeed.AutoJobs.Models.Matches;
    using Fanex.OddsFeed.AutoJobs.Models.Outright;
    using Fanex.OddsFeed.AutoJobs.Repositories;
    using Fanex.OddsFeed.AutoJobs.Services.Interfaces;

    public class OddsDataService : IOddsDataService
    {
        private readonly IOddsDataRepository oddsDataRepository;

        public OddsDataService(IOddsDataRepository oddsDataRepository)
        {
            this.oddsDataRepository = oddsDataRepository;
        }

        public IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            var criteria = new GetDeadballOddsDataCriteria();
            var matches = oddsDataRepository.GetDeadballMatches(criteria, betTypeSchemas);

            return matches;
        }

        public async Task<IList<Match>> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            var criteria = new GetLiveOddDataCriteria();
            var matches = await oddsDataRepository.GetLiveMatches(criteria, betTypeSchemas);

            return matches;
        }

        public IList<Outright> GetOutrights()
        => oddsDataRepository.GetOutrights(new GetOutrightOddsDataCriteria());
    }
}
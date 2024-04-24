namespace OddsFeed.AutoJobs.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.Matches;

    public class ResultFeedDataService : IResultFeedDataService
    {
        private readonly IBaseService baseService;

        public ResultFeedDataService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IList<Match>> GetMatchResultDetailsAsync(ICriteria criteria)
        {
            (var matches, var matchResultDetails) = await baseService.FetchMultipleAsync<Match, MatchResultDetail>(criteria);
            foreach (var match in matches)
            {
                match.MatchResultDetails = matchResultDetails.Where(x => x.MatchId == match.MatchId).ToList();
            }

            return matches.ToList();
        }
    }
}
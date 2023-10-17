namespace Fanex.OddsFeed.AutoJobs.Services
{
    using Fanex.OddsFeed.AutoJobs.Criteria.FeedResult;
    using Fanex.OddsFeed.Repository;
    using Fanex.OddsFeed.Models.Matches;

    public interface IResultFeedDataService
    {
        Task<IList<Match>> GetMatchResultDetailsAsync(DateTime eventDate);
    }

    public class ResultFeedDataService : IResultFeedDataService
    {
        private readonly IDbAccessRepository dbAccessRepository;

        public ResultFeedDataService(IDbAccessRepository dbAccessRepository)
        {
            this.dbAccessRepository = dbAccessRepository;
        }

        public async Task<IList<Match>> GetMatchResultDetailsAsync(DateTime eventDate)
        {
            var criteria = new GetMatchResultDetailsCriteria
            {
                EventDate = eventDate,
            };

            return await dbAccessRepository.GetMatchResultDetailsAsync(criteria);
        }
    }
}
namespace Fanex.OddsFeed.AutoJobs.Services
{
    using Fanex.OddsFeed.AutoJobs.Criteria.FeedResult;
    using Fanex.OddsFeed.AutoJobs.Repositories;
    using Fanex.OddsFeed.Models.Matches;

    public interface IResultFeedDataService
    {
        Task<IList<Match>> GetMatchResultDetails(DateTime eventDate);
    }

    public class ResultFeedDataService : IResultFeedDataService
    {
        private readonly IResultFeedRepository resultFeedRepository;

        public ResultFeedDataService(IResultFeedRepository resultFeedRepository)
        {
            this.resultFeedRepository = resultFeedRepository;
        }

        public async Task<IList<Match>> GetMatchResultDetails(DateTime eventDate)
        {
            var criteria = new GetMatchResultDetailsCriteria
            {
                EventDate = eventDate,
            };

            return await resultFeedRepository.GetMatchResultDetails(criteria);
        }
    }
}
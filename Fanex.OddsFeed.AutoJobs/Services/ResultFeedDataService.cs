namespace Fanex.OddsFeed.AutoJobs.Services
{
    using Fanex.OddsFeed.AutoJobs.Criteria.FeedResult;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Repository.Interfaces;

    public interface IResultFeedDataService
    {
        Task<IList<Match>> GetMatchResultDetails(DateTime eventDate);
    }

    public class ResultFeedDataService : IResultFeedDataService
    {
        private readonly IDbAccessRepository dbAccess;

        public ResultFeedDataService(IDbAccessRepository dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public async Task<IList<Match>> GetMatchResultDetails(DateTime eventDate)
        {
            var criteria = new GetMatchResultDetailsCriteria
            {
                EventDate = eventDate,
            };

            return await dbAccess.GetMatchResultDetails(criteria);
        }
    }
}
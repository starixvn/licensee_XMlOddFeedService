namespace OddsFeed.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.AutoJobs.Criteria.FeedResult;
    using OddsFeed.Models.Matches;
    using OddsFeed.Repository;

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
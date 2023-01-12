namespace Fanex.OddsFeed.AutoJobs.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.AutoJobs.Models.Matches;

    public interface IResultFeedRepository
    {
        Task<IList<Match>> GetMatchResultDetails(ICriteria criteria);
    }

    public class ResultFeedRepository : IResultFeedRepository
    {
        public async Task<IList<Match>> GetMatchResultDetails(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var matches = multi.Read<Match>().ToList();
                    var matchResultDetails = multi.Read<MatchResultDetail>().ToList();

                    foreach (var match in matches)
                    {
                        match.MatchResultDetails = matchResultDetails.Where(x => x.MatchId == match.MatchId).ToList();
                    }

                    return matches;
                }
            }
        }
    }
}
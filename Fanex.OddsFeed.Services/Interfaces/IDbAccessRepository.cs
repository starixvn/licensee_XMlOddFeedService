namespace Fanex.OddsFeed.Repository.Interfaces
{
    using System.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Oddsfeed;

    public interface IDbAccessRepository
    {
        Task<DataTable> GetDataTableAsync(ICriteria criteria);

        Task<DataTable> GetInsertBetDaq(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetLiveMatches(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetDeadballMatches(ICriteria criteria);

        Task<IList<Match>> GetMatchResultDetails(ICriteria criteria);
    }
}
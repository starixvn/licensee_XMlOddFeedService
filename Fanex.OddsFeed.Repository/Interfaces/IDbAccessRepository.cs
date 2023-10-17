namespace Fanex.OddsFeed.Repository
{
    using System.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Oddsfeed;

    public interface IDbAccessRepository
    {
        Task<DataTable> GetDataTableAsync(ICriteria criteria);

        Task<DataTable> GetInsertBetDaqAsync(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetLiveMatchAsync(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetDeadballMatchAsync(ICriteria criteria);

        Task<IList<Match>> GetMatchResultDetailsAsync(ICriteria criteria);
    }
}

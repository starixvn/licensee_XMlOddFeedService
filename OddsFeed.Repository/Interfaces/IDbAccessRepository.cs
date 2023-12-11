namespace OddsFeed.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Oddsfeed;

    public interface IDbAccessRepository
    {
        Task<DataTable> GetDataTableAsync(ICriteria criteria);

        Task<DataTable> GetInsertBetDaqAsync(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetLiveMatchAsync(ICriteria criteria);

        Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetDeadballMatchAsync(ICriteria criteria);

        Task<IList<Match>> GetMatchResultDetailsAsync(ICriteria criteria);
    }
}

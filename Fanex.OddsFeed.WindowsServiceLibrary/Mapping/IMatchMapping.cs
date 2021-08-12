namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using System.Collections.Generic;
    using System.Data;
    using Common.DTO;
    using Common.Schemas;

    public interface IMatchMapping
    {
        IList<Match> GetMatches(
            DataTable dataTable,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive);

        IList<Match> GetMatchesTest(
            DataTable dataTable,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive);
    }
}
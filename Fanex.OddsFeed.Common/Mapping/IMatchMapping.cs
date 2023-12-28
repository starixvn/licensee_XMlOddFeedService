namespace Fanex.OddsFeed.Common.Mapping
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Schemas;

    public interface IMatchMapping
    {
        IList<Match> GetMatches(
            DataTable dataTable,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive);

        Match CreateMatch(DataRow row, bool isLive);
    }
}
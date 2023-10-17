namespace Fanex.OddsFeed.Repository
{
    using System.Data;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Oddsfeed;

    public class DbAccessRepository : IDbAccessRepository
    {
        public async Task<DataTable> GetDataTableAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (DataSet dataSet = await db.ExecuteDataSetAsync(criteria))
                {
                    return dataSet.Tables[0];
                }
            }
        }

        public async Task<DataTable> GetInsertBetDaqAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var dataset = await db.ExecuteDataSetAsync(criteria);
                var indexTable = dataset.Tables.Count - 1;

                return dataset.Tables[indexTable];
            }
        }

        public async Task<IList<Match>> GetMatchResultDetailsAsync(ICriteria criteria)
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

        public async Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetDeadballMatchAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (DataSet dataSet = await db.ExecuteDataSetAsync(criteria))
                {
                    var deadballTables = dataSet.Tables[0].Copy();
                    var oddsFlexTables = dataSet.Tables[1].Copy();
                    var oddsFlexes = GetOddsFlexGroups(oddsFlexTables);
                    return new(deadballTables, oddsFlexes);
                }
            }
        }

        public async Task<(DataTable, IEnumerable<OddsFlexGroup>)> GetLiveMatchAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (DataSet dataSet = await db.ExecuteDataSetAsync(criteria))
                {
                    var liveTables = dataSet.Tables[0].Copy();
                    var oddsFlex = GetOddsFlexGroups(dataSet.Tables[1]);
                    return new(liveTables, oddsFlex);
                }
            }
        }

        private static IEnumerable<OddsFlexGroup> GetOddsFlexGroups(DataTable table)
        {
            var oddsFlexGroups = new List<OddsFlexGroup>();

            foreach (DataRow row in table.Rows)
            {
                var oddsId = Convert.ToInt32(row["oddsid"]);
                var oddsFlex = new OddsFlex
                {
                    BetChoice = row["betChoice"].ToString(),
                    BetType = Convert.ToInt32(row["bettype"]),
                    OddId = oddsId,
                    Odds = Convert.ToDecimal(row["odds"]),
                    OddsInfo = row["OddsInfo"].ToString()
                };
                var oddsFlexGroup = oddsFlexGroups.FirstOrDefault(odds => odds.OddsId == oddsId);

                if (oddsFlexGroup == null)
                {
                    oddsFlexGroups.Add(new OddsFlexGroup
                    {
                        OddsId = oddsId,
                        OddsFlexs = new List<OddsFlex> { oddsFlex }
                    });
                }
                else
                {
                    oddsFlexGroup.OddsFlexs.Add(oddsFlex);
                }
            }

            return oddsFlexGroups;
        }
    }
}
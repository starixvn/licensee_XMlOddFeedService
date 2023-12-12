namespace OddsFeed.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using OddsFeed.Models.Oddsfeed;

    public class DbAccessRepository : IDbAccessRepository
    {
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
                    OddsId = oddsId,
                    Odds = Convert.ToDecimal(row["odds"]),
                    OddsInfo = row["OddsInfo"].ToString()
                };
                var oddsFlexGroup = oddsFlexGroups.Find(odds => odds.OddsId == oddsId);

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
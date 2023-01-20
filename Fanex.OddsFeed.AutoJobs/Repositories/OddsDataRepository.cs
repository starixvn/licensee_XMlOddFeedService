namespace Fanex.OddsFeed.AutoJobs.Repositories
{
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.AutoJobs.Helpers;
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Oddsfeed;
    using Fanex.OddsFeed.Models.Outright;

    public interface IOddsDataRepository
    {
        Task<IList<Match>> GetLiveMatches(ICriteria criteria, IList<BetTypeSchema> betTypeSchemas);

        IList<Match> GetDeadballMatches(ICriteria criteria, IList<BetTypeSchema> betTypeSchemas);

        IList<Outright> GetOutrights(ICriteria criteria);
    }

    public class OddsDataRepository : IOddsDataRepository
    {
        public IList<Match> GetDeadballMatches(ICriteria criteria, IList<BetTypeSchema> betTypeSchemas)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (DataSet dataSet = db.ExecuteDataSet(criteria))
                {
                    var deadballTables = dataSet.Tables[0].Copy();
                    var oddsFlexTables = dataSet.Tables[1].Copy();
                    var oddsFlex = GetOddsFlexGroups(oddsFlexTables);
                    var matches = MappingHelper.MappingMatches(deadballTables, betTypeSchemas, oddsFlex, false);
                    return matches.OrderBy(t => t.MatchId).ToList();
                }
            }
        }

        public async Task<IList<Match>> GetLiveMatches(ICriteria criteria, IList<BetTypeSchema> betTypeSchemas)
        {
            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (DataSet dataSet = await db.ExecuteDataSetAsync(criteria))
                    {
                        var liveTables = dataSet.Tables[0].Copy();
                        var oddsFlex = GetOddsFlexGroups(dataSet.Tables[1]);
                        var matches = MappingHelper.MappingMatches(liveTables, betTypeSchemas, oddsFlex, false);
                        return matches.OrderBy(t => t.MatchId).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<Outright> GetOutrights(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (DataSet dataSet = db.ExecuteDataSet(criteria))
                {
                    var outrightsTable = dataSet.Tables[0];
                    var outrights = MappingHelper.MappingOutrights(outrightsTable);
                    return outrights;
                }
            }
        }

        private IEnumerable<OddsFlexGroup> GetOddsFlexGroups(DataTable table)
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
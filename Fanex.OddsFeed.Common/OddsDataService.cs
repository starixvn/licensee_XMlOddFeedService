namespace Fanex.OddsFeed.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.OddsFeed.Common.Criteria;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Mapping;
    using Fanex.OddsFeed.Common.Schemas;

    public interface IOddsDataService
    {
        IList<Match> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Match> GetLiveMatchesForBetRadar(string sportIds = null);

        IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas);

        IList<Outright> GetOutrights();
    }

    public class OddsDataService : IOddsDataService
    {
        private readonly IMatchMapping matchMapping;
        private readonly IOutrightMapping outrightMapping;

        public OddsDataService()
        {
            matchMapping = new MatchMapping();
            outrightMapping = new OutrightMapping();
        }

        public IList<Match> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            var criteria = new GetLiveOddsDataCriteria();
            using (var liveOdds = DbAccess.GetLiveOddsData(criteria))
            {
                var liveTables = liveOdds.Tables[0].Copy();
                var oddsFlex = GetOddsFlexGroups(liveOdds.Tables[1]);
                var matches = matchMapping.GetMatches(liveTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();
                return matches;
            }
        }

        public IList<Match> GetLiveMatchesForBetRadar(string sportIds = null)
        {
            var criteria = new GetLiveOddsForBetRadarCriteria
            {
                SportIDList = sportIds
            };
            using (var liveOdds = DbAccess.GetLiveOddsData(criteria))
            {
                var liveTables = liveOdds.Tables[0].Copy();

                var matches = new List<Match>();
                foreach (DataRow row in liveTables.Rows)
                {
                    var match = matchMapping.CreateMatch(row, false);
                    if (!matches.Exists(x => x.MatchId == match.MatchId))
                    {
                        match.MatchOddsList = new List<MatchOdds>();
                        matches.Add(match);
                    }
                }

                return matches;
            }
        }

        public IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var deadballOdds = DbAccess.GetDeadballOddsData())
            {
                var deadballTables = deadballOdds.Tables[0].Copy();
                var oddsFlexTables = deadballOdds.Tables[1].Copy();
                var oddsFlex = GetOddsFlexGroups(oddsFlexTables);

                var matches = matchMapping.GetMatches(deadballTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();
                return matches;
            }
        }

        public IList<Outright> GetOutrights()
        {
            using (var outrightOdds = DbAccess.GetOutrightOddsData())
            {
                return outrightMapping.GetOutrights(outrightOdds);
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
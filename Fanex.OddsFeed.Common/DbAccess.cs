namespace Fanex.OddsFeed.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Data;
    using Fanex.OddsFeed.Common.Criteria;
    using Fanex.OddsFeed.Common.DTO;

    public static class DbAccess
    {
        public static DataSet GetLiveOddsData()
        {
            var criteria = new GetLiveOddsDataCriteria();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet();
            }
        }

        public static DataSet GetDeadballOddsData()
        {
            var criteria = new GetDeadballOddsDataCriteria();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet();
            }
        }

        public static DataTable GetOutrightOddsData()
        {
            var criteria = new GetOutrightOddsDataCriteria();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetBetdaqScheduleData(string uspName)
        {
            using (var db = new ObjectDb(uspName))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetBetRadar()
        {
            var criteria = new GetBetRadarOddsCriteria();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetCountryList()
        {
            var criteria = new GetCountryListCriteria();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetInsertBetDaq(string spName, Hashtable spParams)
        {
            using (var db = new ObjectDb(spName))
            {
                var dataset = db.ExecuteDataSet(spParams);
                var indexTable = dataset.Tables.Count - 1;

                return dataset.Tables[indexTable];
            }
        }

        public static IList<Match> GetMatchResultDetails(DateTime eventDate)
        {
            var criteria = new GetMatchResultDetailCriteria
            {
                EventDate = eventDate
            };
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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
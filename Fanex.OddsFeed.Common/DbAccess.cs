namespace Fanex.OddsFeed.Common
{
    using System.Collections;
    using System.Data;
    using Data;
    using Helpers;

    public static class DbAccess
    {
        public static DataSet GetLiveOddsData()
        {
            using (var db = new ObjectDb(SpHelper.RealSp("SP.LiveOdds")))
            {
                return db.ExecuteDataSet();
            }
        }

        public static DataSet GetDeadballOddsData()
        {
            using (var db = new ObjectDb(SpHelper.RealSp("SP.DeadballOdds")))
            {
                return db.ExecuteDataSet();
            }
        }

        public static DataTable GetOutrightOddsData()
        {
            using (var db = new ObjectDb(SpHelper.RealSp("SP.OutrightOdds")))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetBetdaqScheduleData(string uspName)
        {
            using (var db = new ObjectDb(SpHelper.RealSp(uspName)))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetBetRadar()
        {
            using (var db = new ObjectDb(SpHelper.RealSp("SP.BetRadarOddsSel")))
            {
                return db.ExecuteDataSet().Tables[0];
            }
        }

        public static DataTable GetCountryList()
        {
            using (var db = new ObjectDb(SpHelper.RealSp("SP.GetCountryList")))
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
    }
}
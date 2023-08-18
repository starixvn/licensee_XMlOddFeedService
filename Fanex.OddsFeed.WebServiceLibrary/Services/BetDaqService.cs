namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections;
    using Common;
    using Common.Configurations;
    using Extensions;
    using Fanex.Common;

    public class BetDaqService : IBetDaqService
    {
        private const string SeparateRow = "__";
        private const string BasketBall0 = "0";
        private const string BasketBall1 = "1";
        private const string BasketBall2 = "2";
        private const string Portal_BetDaq_Schedule = "Portal_BetDaq_Schedule";

        public string GetSchedule(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return GetBetDaqSchedule();
            }

            if (filter.Equals(BasketBall0) || filter.Equals("s"))
            {
                return GetBetDaqScheduleBasketBall(Portal_BetDaq_Schedule, "IsBasketBall = 0");
            }

            if (filter.Equals(BasketBall1) || filter.Equals("b"))
            {
                return GetBetDaqScheduleBasketBall(Portal_BetDaq_Schedule, "IsBasketBall = 1");
            }

            if (filter.Equals(BasketBall2))
            {
                return GetBetDaqScheduleBasketBall(Portal_BetDaq_Schedule, "IsBasketBall = 2");
            }

            return string.Empty;
        }

        public string GetOddWithUserIsBetdaq(string filter)
        {
            var result = string.Empty;
            using (var oddsFeed_1 = DbAccess.GetBetdaqScheduleData("SP.GetOddsFeed"))
            using (var oddsFeed_2 = DbAccess.GetBetdaqScheduleData("SP.GetOddsFeed_2"))
            {
                if (string.IsNullOrEmpty(filter))
                {
                    result = oddsFeed_1.Select().ToCSV_Odds(SeparateRow);
                    result += oddsFeed_2.Select().ToCSV_Odds_2(SeparateRow);
                }
                else if (filter.Equals(BasketBall0) || filter.Equals("s"))
                {
                    result = oddsFeed_1.Select("IsBasketBall = 0").ToCSV_Odds(SeparateRow);
                    result += oddsFeed_2.Select("IsBasketBall = 0").ToCSV_Odds_2(SeparateRow);
                }
                else if (filter.Equals(BasketBall1) || filter.Equals("b"))
                {
                    result = oddsFeed_1.Select("IsBasketBall = 1").ToCSV_Odds(SeparateRow);
                    result += oddsFeed_2.Select("IsBasketBall = 1").ToCSV_Odds_2(SeparateRow);
                }
                else
                {
                    if (filter.Equals(BasketBall2))
                    {
                        result = oddsFeed_1.Select("IsBasketBall = 2").ToCSV_Odds(SeparateRow);
                        result += oddsFeed_1.Select("IsBasketBall = 2").ToCSV_Odds_2(SeparateRow);
                    }
                }
            }

            return result;
        }

        public int InsertBetDaq(Hashtable spParams, bool isDownTime)
        {
            var spName = isDownTime ? AppSettings.SpTestingUMTime : "SP.InsertBetDaq";
            var result = DbAccess.GetInsertBetDaq(spName, spParams);
            return Converter.ToInt(result.Rows[0]["ErrCode"]);
        }

        private static string GetBetDaqSchedule()
        {
            var result = DbAccess.GetBetdaqScheduleData(Portal_BetDaq_Schedule);
            return result.ToCSV(false, SeparateRow);
        }

        private static string GetBetDaqScheduleBasketBall(string spName, string query)
        {
            var result = DbAccess.GetBetdaqScheduleData(spName);
            return result.Select(query).ToCSV(SeparateRow);
        }
    }
}
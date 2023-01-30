namespace Fanex.OddsFeed.Web.Services.Services
{
    using System.Collections;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Repository.Interfaces;
    using Fanex.OddsFeed.Web.Services.Criteria.BetDaq;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Models;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

    public class BetDaqService : IBetDaqService
    {
        private const string SeparateRow = "__";
        private const string BasketBall0 = "0";
        private const string BasketBall1 = "1";
        private const string BasketBall2 = "2";

        private readonly IDbAccessRepository dbAccess;
        private readonly WebSettings webSettings;

        public BetDaqService(IDbAccessRepository dbAccess, WebSettings webSettings)
        {
            this.dbAccess = dbAccess;
            this.webSettings = webSettings;
        }

        public async Task<string> GetSchedule(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await GetBetDaqSchedule();
            }

            if (filter.Equals(BasketBall0) || filter.Equals("s"))
            {
                return await GetBetDaqScheduleBasketBall("IsBasketBall = 0");
            }

            if (filter.Equals(BasketBall1) || filter.Equals("b"))
            {
                return await GetBetDaqScheduleBasketBall("IsBasketBall = 1");
            }

            if (filter.Equals(BasketBall2))
            {
                return await GetBetDaqScheduleBasketBall("IsBasketBall = 2");
            }

            return string.Empty;
        }

        public async Task<int> InsertBetDaq(Hashtable spParams, bool isDownTime)
        {
            ICriteria criteria = new InsertBetDaqCriteria(isDownTime ? webSettings.SpTestingUMTime : string.Empty);
            var result = await dbAccess.GetInsertBetDaq(criteria);

            return Convert.ToInt32(result.Rows[0]["ErrCode"]);
        }

        public async Task<string> GetOddWithUserIsBetdaq(string filter)
        {
            var result = string.Empty;
            var oddsFeed_1 = await dbAccess.GetDataTableAsync(new GetOddsFeedCriteria());
            var oddsFeed_2 = await dbAccess.GetDataTableAsync(new GetOddsFeed2Criteria());
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

            return result;
        }

        private async Task<string> GetBetDaqSchedule()
        {
            var result = await dbAccess.GetDataTableAsync(new GetBetDaqScheduleCriteria());
            return result.ToCSV(false, SeparateRow);
        }

        private async Task<string> GetBetDaqScheduleBasketBall(string query)
        {
            var result = await dbAccess.GetDataTableAsync(new GetPortalBetDaqScheduleCriteria());
            return result.Select(query).ToCSV(SeparateRow);
        }
    }
}
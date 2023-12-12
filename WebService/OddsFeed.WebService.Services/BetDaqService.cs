namespace OddsFeed.WebService.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using OddsFeed.Core.Services;
    using OddsFeed.WebService.Criteria.BetDaq;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Models.Betdaq;
    using OddsFeed.WebService.Services.Interfaces;

    public class BetDaqService : IBetDaqService
    {
        private const string SeparateRow = "__";
        private const string BasketBall0 = "0";
        private const string BasketBall1 = "1";
        private const string BasketBall2 = "2";

        private readonly IBaseService baseService;
        private readonly WebSettings webSettings;

        public BetDaqService(IBaseService baseService, WebSettings webSettings)
        {
            this.baseService = baseService;
            this.webSettings = webSettings;
        }

        public async Task<string> GetScheduleAsync(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await GetBetDaqScheduleAsync();
            }

            if (filter.Equals(BasketBall0) || filter.Equals("s"))
            {
                return await GetBetDaqScheduleBasketBallAsync(0);
            }

            if (filter.Equals(BasketBall1) || filter.Equals("b"))
            {
                return await GetBetDaqScheduleBasketBallAsync(1);
            }

            if (filter.Equals(BasketBall2))
            {
                return await GetBetDaqScheduleBasketBallAsync(2);
            }

            return string.Empty;
        }

        public async Task<int> InsertBetDaqAsync(InsertBetDaqParam param, bool isDownTime)
        {
            var criteria = new InsertBetDaqCriteria(isDownTime ? webSettings.SpTestingUMTime : string.Empty)
            {
                OddsId = param.OddsId,
                hdp1 = param.hdp1,
                hdp2 = param.hdp2,
                betteam = param.betteam,
                odds = param.odds,
                stake = param.stake,
                betdaqid = param.betdaqid,
                bet_ip = param.bet_ip
            };

            var result = await baseService.GetDataEntityAsync<int>(criteria);

            return result;
        }

        public async Task<string> GetOddWithUserIsBetdaqAsync(string filter)
        {
            var result = string.Empty;
            var oddsFeed_1 = await baseService.GetDataAsync<BetdaqOdd>(new GetOddsFeedCriteria());
            var oddsFeed_2 = await baseService.GetDataAsync<BetdaqOdd>(new GetOddsFeed2Criteria());
            if (string.IsNullOrEmpty(filter))
            {
                result = oddsFeed_1.ToCSV_Odds(SeparateRow);
                result += oddsFeed_2.ToCSV_Odds_2(SeparateRow);
            }
            else if (filter.Equals(BasketBall0) || filter.Equals("s"))
            {
                result = oddsFeed_1.Where(x => x.IsBasketball == 0).ToCSV_Odds(SeparateRow);
                result += oddsFeed_2.Where(x => x.IsBasketball == 0).ToCSV_Odds_2(SeparateRow);
            }
            else if (filter.Equals(BasketBall1) || filter.Equals("b"))
            {
                result = oddsFeed_1.Where(x => x.IsBasketball == 1).ToCSV_Odds(SeparateRow);
                result += oddsFeed_2.Where(x => x.IsBasketball == 1).ToCSV_Odds_2(SeparateRow);
            }
            else
            {
                if (filter.Equals(BasketBall2))
                {
                    result = oddsFeed_1.Where(x => x.IsBasketball == 2).ToCSV_Odds(SeparateRow);
                    result += oddsFeed_1.Where(x => x.IsBasketball == 2).ToCSV_Odds_2(SeparateRow);
                }
            }

            return result;
        }

        private async Task<string> GetBetDaqScheduleAsync()
        {
            var odds = await baseService.GetDataAsync<BetdaqOdd>(new GetBetDaqScheduleCriteria());
            return odds.ToCSV(SeparateRow);
        }

        private async Task<string> GetBetDaqScheduleBasketBallAsync(byte isBasketball)
        {
            var result = await baseService.GetDataAsync<BetdaqOdd>(new GetBetDaqScheduleCriteria());
            return result.Where(x => x.IsBasketball == isBasketball).ToCSV(SeparateRow);
        }
    }
}
namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.Data;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Criteria;

    public class UserService : IUserService
    {
        private readonly IBaseService baseService;

        public UserService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ColumnModel<BetType>> GetBetTypeAsync(string username)
        {
            var criteria = new GetBetTypeByUserNameCriteria
            {
                Username = username
            };
            var betTypes = await baseService.GetDataAsync<BetType>(criteria);
            return GenerateData(betTypes);
        }

        public async Task<string> GetContactEmailAsync(string username)
            => await baseService.GetDataEntityAsync<string>(
                new GetContactEmailByUserNameCriteria
                {
                    Username = username
                });

        public async Task<ColumnModel<Country>> GetCountryAsync(string username)
        {
            var criteria = new GetCountryByUserNameCriteria
            {
                Username = username
            };
            var countries = await baseService.GetDataAsync<Country>(criteria);
            return GenerateData(countries);
        }

        public async Task<ColumnModel<Sport>> GetSportAsync(string username)
        {
            var criteria = new GetSportByUserNameCriteria
            {
                Username = username
            };
            var sports = await baseService.GetDataAsync<Sport>(criteria);
            return GenerateData(sports.Where(s => s.SportId != OddsFeed.Core.Constants.CricketSportTypeId));
        }

        private static ColumnModel<T> GenerateData<T>(IEnumerable<T> list)
        {
            const int amountCondition = 5;
            var numberItems = list.Count();

            if (numberItems <= amountCondition)
            {
                return GetColumns(list, numberItems, 0, 0);
            }
            else
            {
                var divide = numberItems / 3;
                var module = numberItems % 3;
                var sumCol1 = divide;
                var sumCol2 = divide;
                var sumCol3 = divide;

                while (module > 0)
                {
                    sumCol2 += 1;
                    module--;

                    if (module > 0)
                    {
                        sumCol3 += 1;
                        module--;
                    }

                    if (module > 0)
                    {
                        sumCol1 += 1;
                        module--;
                    }
                }

                return GetColumns(list, sumCol1, sumCol2, sumCol3);
            }
        }

        private static ColumnModel<T> GetColumns<T>(IEnumerable<T> list, int sumCol1, int sumCol2, int sumCol3)
        {
            return new ColumnModel<T>
            {
                ColumnOne = list.Take(sumCol1),
                ColumnTwo = list.Skip(sumCol1).Take(sumCol2),
                ColumnThree = list.Skip(sumCol1 + sumCol2).Take(sumCol3)
            };
        }
    }
}
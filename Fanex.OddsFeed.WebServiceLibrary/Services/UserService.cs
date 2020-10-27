namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.DTO;
    using Models;
    using Repositories;

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService()
        {
            _repository = new UserRepository();
        }

        public ColumnModel<BetType> GetBetTypes(string username)
        {
            var bettypes = _repository.GetBetTypes(username).OrderBy(bettype => bettype.BetTypeId);

            return GenerateData(bettypes);
        }

        public ColumnModel<Sport> GetSports(string username)
        {
            const int ignoreSportId = 50;
            return GenerateData(_repository.GetSports(username).Where(s => s.SportId != ignoreSportId));
        }

        public ColumnModel<Country> GetCountries(string username)
        {
            return GenerateData(_repository.GetCountries(username));
        }

        public string GetContactEmail(string username)
        {
            return _repository.GetContactEmail(username).ContactEmail;
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
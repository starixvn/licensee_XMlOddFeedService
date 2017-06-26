namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.DTO;
    using Criteria;

    public class UserRepository : BaseRepository, IUserRepository
    {
        public IEnumerable<BetType> GetBetTypes(string username)
        {
            return GetListOf<BetType>(new GetBetTypeByUserNameCriteria { Username = username });
        }

        public IEnumerable<Sport> GetSports(string username)
        {
            return GetListOf<Sport>(new GetSportByUserNameCriteria { Username = username });
        }

        public IEnumerable<Country> GetCountries(string username)
        {
            return GetListOf<Country>(new GetCountryByUserNameCriteria { Username = username });
        }

        public Contact GetContactEmail(string username)
        {
            return GetListOf<Contact>(new GetContactEmailByUserNameCriteria { Username = username }).First();
        }
    }
}

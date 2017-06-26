namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.Common.DTO;

    public interface IUserRepository
    {
        IEnumerable<BetType> GetBetTypes(string username);

        IEnumerable<Sport> GetSports(string username);

        IEnumerable<Country> GetCountries(string username);

        Contact GetContactEmail(string username);
    }
}

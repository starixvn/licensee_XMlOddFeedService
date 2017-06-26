namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Common.DTO;
    using Models;

    public interface IUserService
    {
        ColumnModel<BetType> GetBetTypes(string username);

        ColumnModel<Sport> GetSports(string username);

        ColumnModel<Country> GetCountries(string username);

        string GetContactEmail(string username);
    }
}

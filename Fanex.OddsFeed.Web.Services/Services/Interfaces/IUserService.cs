namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.Data;
    using Fanex.OddsFeed.Models.DTO;

    public interface IUserService
    {
        Task<ColumnModel<BetType>> GetBetTypes(string username);

        Task<ColumnModel<Sport>> GetSports(string username);

        Task<ColumnModel<Country>> GetCountries(string username);

        Task<string> GetContactEmail(string username);
    }
}
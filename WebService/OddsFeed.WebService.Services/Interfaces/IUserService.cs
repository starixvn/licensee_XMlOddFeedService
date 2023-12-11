namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;
    using OddsFeed.Models.Data;
    using OddsFeed.Models.DTO;

    public interface IUserService
    {
        Task<ColumnModel<BetType>> GetBetTypes(string username);

        Task<ColumnModel<Sport>> GetSports(string username);

        Task<ColumnModel<Country>> GetCountries(string username);

        Task<string> GetContactEmail(string username);
    }
}
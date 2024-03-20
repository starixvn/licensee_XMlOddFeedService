namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;
    using OddsFeed.Models.Data;
    using OddsFeed.Models.DTO;

    public interface IUserService
    {
        Task<ColumnModel<BetType>> GetBetTypeAsync(string username);

        Task<ColumnModel<Sport>> GetSportAsync(string username);

        Task<ColumnModel<Country>> GetCountryAsync(string username);

        Task<string> GetContactEmailAsync(string username);
    }
}
namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;

    public interface IBetTypeService
    {
        Task<string> GetBetTypeNameByIdAsync(int betTypeId);

        Task<IDictionary<int, BetType>> GetBetTypeListAsync();
    }
}
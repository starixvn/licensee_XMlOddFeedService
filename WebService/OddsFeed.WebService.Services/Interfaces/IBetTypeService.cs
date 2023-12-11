namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;

    public interface IBetTypeService
    {
        Task<string> GetBetTypeNameById(int betTypeId);

        Task<IDictionary<int, BetType>> GetBetTypeList();
    }
}
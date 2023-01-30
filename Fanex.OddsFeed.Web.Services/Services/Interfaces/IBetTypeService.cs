namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.DTO;

    public interface IBetTypeService
    {
        Task<string> GetBetTypeNameById(int betTypeId);

        Task<IDictionary<int, BetType>> GetBetTypeList();
    }
}
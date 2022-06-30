namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections.Generic;
    using Common.DTO;

    public interface IBetTypeService
    {
        string GetBetTypeNameById(int betTypeId);

        IDictionary<int, BetType> GetBetTypeList();
    }
}

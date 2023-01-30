namespace Fanex.OddsFeed.Web.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

    public class BetTypeService : IBetTypeService
    {
        private readonly IFeedCachingOperator feedCachingOperator;

        public BetTypeService(IFeedCachingOperator feedCachingOperator)
        {
            this.feedCachingOperator = feedCachingOperator;
        }

        public async Task<IDictionary<int, BetType>> GetBetTypeList()
            => (await feedCachingOperator.GetBetTypes()).ToDictionary(
                    schema => schema.BetTypeId,
                    schema => new BetType
                    {
                        BetTypeName = schema.Tag,
                        IsChildBetType = schema.IsChildBetType
                    });

        public async Task<string> GetBetTypeNameById(int betTypeId)
        {
            var betTypeList = await GetBetTypeList();
            return GetBetTypeName(betTypeList, betTypeId);
        }

        private static string GetBetTypeName(IDictionary<int, BetType> betTypeList, int betTypeId)
            => betTypeList.ContainsKey(betTypeId) ? betTypeList[betTypeId].BetTypeName : "b" + betTypeId;
    }
}
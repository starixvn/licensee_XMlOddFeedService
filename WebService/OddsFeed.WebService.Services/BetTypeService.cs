namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;

    public class BetTypeService : IBetTypeService
    {
        private readonly IFeedCachingOperator feedCachingOperator;

        public BetTypeService(IFeedCachingOperator feedCachingOperator)
        {
            this.feedCachingOperator = feedCachingOperator;
        }

        public async Task<IDictionary<int, BetType>> GetBetTypeListAsync()
            => (await feedCachingOperator.GetBetTypeAsync()).ToDictionary(
                    schema => schema.BetTypeId,
                    schema => new BetType
                    {
                        BetTypeName = schema.Tag,
                        IsChildBetType = schema.IsChildBetType
                    });

        public async Task<string> GetBetTypeNameByIdAsync(int betTypeId)
        {
            var betTypeList = await GetBetTypeListAsync();
            return GetBetTypeName(betTypeList, betTypeId);
        }

        private static string GetBetTypeName(IDictionary<int, BetType> betTypeList, int betTypeId)
            => betTypeList.ContainsKey(betTypeId) ? betTypeList[betTypeId].BetTypeName : "b" + betTypeId;
    }
}
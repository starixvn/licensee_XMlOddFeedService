namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;

    public class BetTypeService : IBetTypeService
    {
        private readonly IFeedCachingOperator feedCachingOperator;

        public BetTypeService()
        {
            feedCachingOperator = new FeedCachingOperator();
        }

        public string GetBetTypeNameById(int betTypeId)
        {
            var betTypeList = GetBetTypeList();

            return BetTypeHelper.GetBetTypeName(betTypeList, betTypeId);
        }

        public IDictionary<int, BetType> GetBetTypeList()
        {
            return feedCachingOperator.GetBetTypes().ToDictionary(
                schema => schema.BetTypeId,
                schema => new BetType { BetTypeName = schema.Tag, IsChildBetType = schema.IsChildBetType });
        }
    }
}
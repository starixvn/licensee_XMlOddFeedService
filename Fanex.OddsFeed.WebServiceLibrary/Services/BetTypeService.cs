namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Caching;
    using Common.DTO;
    using Common.Schemas;
    using Helpers;

    public class BetTypeService : IBetTypeService
    {
        private readonly IOddsFeedCachingOperator _oddsFeedCachingOperator;

        public BetTypeService()
        {
            _oddsFeedCachingOperator = new OddsFeedCachingOperator();
        }

        public string GetBetTypeNameById(int betTypeId)
        {
            var betTypeList = GetBetTypeList();

            return BetTypeHelper.GetBetTypeName(betTypeList, betTypeId);
        }

        public IDictionary<int, BetType> GetBetTypeList()
        {
            return _oddsFeedCachingOperator.GetBetTypes().ToDictionary(
                schema => schema.BetTypeId,
                schema => new BetType { BetTypeName = schema.Tag, IsChildBetType = schema.IsChildBetType });
        }
    }
}

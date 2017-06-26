using Couchbase.Core;
using Fanex.OddsFeed.Common.DTO;
using Fanex.OddsFeed.WebServiceLibrary.Models;

namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    public interface IVersionCaching
    {
        CacheVersion GetRightVersion(ICluster cluster, OddsFeedsModel model);
    }
}

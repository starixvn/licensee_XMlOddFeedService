namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Fanex.OddsFeed.Common.FilterAtribute;

    public interface IRateLimitService
    {
        ThrottleInfo GetThrottleInfo(string ipAdress);

        void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo);

        void AddToBlockList(string ipAdress, DateTime expiredAt);

        bool IsBlocked(string ipAdress);
    }
}
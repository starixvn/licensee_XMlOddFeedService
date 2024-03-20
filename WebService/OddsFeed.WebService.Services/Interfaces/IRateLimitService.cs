namespace OddsFeed.WebService.Services
{
    using System;
    using OddsFeed.WebService.Models.FilterAttribute;

    public interface IRateLimitService
    {
        ThrottleInfo GetThrottleInfo(string ipAdress);

        void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo);

        void AddToBlockList(string ipAdress, DateTime expiredAt);

        bool IsBlocked(string ipAdress);
    }
}
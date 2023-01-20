namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Web.Services.Models.FilterAttribute;

    public interface IRateLimitService
    {
        ThrottleInfo GetThrottleInfo(string ipAdress);

        void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo);

        void AddToBlockList(string ipAdress, DateTime expiredAt);

        bool IsBlocked(string ipAdress);
    }
}
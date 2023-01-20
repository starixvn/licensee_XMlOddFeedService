namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Web.Services.Enum;

    public interface IFeedsService
    {
        Task<string> GetFeedsData(
           FormatType formatType,
           string feedName,
           string username,
           string domain,
           string ipAddress,
           int? fromid);
    }
}
namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Common.Configurations;

    public interface IFeedsService
    {
        string GetFeedsData(
           FormatType formatType,
           string feedName,
           string username,
           string domain,
           string ipAddress,
           int? fromid);
    }
}

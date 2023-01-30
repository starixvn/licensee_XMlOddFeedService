namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    public interface IBetRadarService
    {
        Task<string> GetXmlDocumentAsync();
    }
}
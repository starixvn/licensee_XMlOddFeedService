namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;

    public interface IBetRadarService
    {
        Task<string> GetXmlDocumentAsync();
    }
}
namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IBetRadarService
    {
        Task<string> GetXmlDocumentAsync();
    }
}
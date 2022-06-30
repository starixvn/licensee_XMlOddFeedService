namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Text;
    using Models;

    public interface IOddsFeedService
    {
        string BuildXmlDocument(OddsFeedsModel model);
    }
}

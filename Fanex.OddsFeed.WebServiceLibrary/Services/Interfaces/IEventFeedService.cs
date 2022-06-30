namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Text;

    public interface IEventFeedService
    {
        string BuildXmlEvent(bool isSample = false, bool isOutright = true);
    }
}

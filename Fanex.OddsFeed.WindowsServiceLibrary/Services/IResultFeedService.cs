namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    public interface IResultFeedService
    {
        void Reset();

        bool Update();
    }
}
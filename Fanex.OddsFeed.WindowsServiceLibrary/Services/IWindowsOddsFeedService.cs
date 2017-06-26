namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    public interface IWindowsOddsFeedService
    {
        void Reset();

        bool Update();
    }
}
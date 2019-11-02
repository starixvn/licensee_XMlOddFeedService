namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using System;

    public interface IWindowsOddsFeedService
    {
        void Reset();

        bool Update();

        string GetExceptionMessage(Exception exception);
    }
}
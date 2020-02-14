namespace Fanex.OddsFeed.WindowsService
{
    using Data.Configuration;
    using WindowsServiceLibrary.Services;

    /// <summary>
    /// Class Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            //var servicesToRun = new ServiceBase[]
            //  {
            //          new OddsFeedService()
            //  };

            //ServiceBase.Run(servicesToRun);
            (new WindowsOddsFeedService()).Update();
        }
    }
}
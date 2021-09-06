namespace Fanex.OddsFeed.WindowsService
{
    using System.ServiceProcess;
    using Data.Configuration;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            var servicesToRun = new ServiceBase[]
            {
                new OddsFeedService()
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
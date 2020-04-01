namespace Fanex.OddsFeed.WindowsService
{
    using Data.Configuration;
    using System.ServiceProcess;

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
namespace Fanex.OddsFeed.Web.Services.Models
{
    using Fanex.OddsFeed.Core.Configurations;

    public class WebSettings : AppSettings
    {
        public string SpTestingUMTime { get; set; }
        public string IsDisableUMTime { get; set; }
        public string VirtualPathWebService { get; set; }
        public int LimitedOptions { get; set; }
        public string HighlightSportType { get; set; }
        public string HighlightBetType { get; set; }
        public string BetTypeDescriptionPath { get; set; }
        public Throttler Throttler { get; set; }
    }

    public class Throttler
    {
        public int RequestLimit { get; set; }
        public int TimeOutInSeconds { get; set; }
        public int BlockTimeInSeconds { get; set; }
    }

    public class EmailSettings
    {
    }
}
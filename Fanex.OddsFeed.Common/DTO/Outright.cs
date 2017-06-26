namespace Fanex.OddsFeed.Common.DTO
{
    using System;

    [Serializable]
    public class Outright
    {
        public int EventId { get; set; }

        public string EventEn { get; set; }

        public string EventTw { get; set; }

        public string EventCn { get; set; }

        public DateTime Time { get; set; }

        public int SportId { get; set; }

        public string SportName { get; set; }

        public string CountryCode { get; set; }

        public SafeList<OutrightOdds> OutrightOddsList { get; set; }
    }
}

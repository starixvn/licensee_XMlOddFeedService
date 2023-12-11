namespace OddsFeed.Models.Outright
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Outright
    {
        public int EventId { get; set; }

        public string EventEn { get; set; }

        public string EventTw { get; set; }

        public string EventCn { get; set; }

        public string EventIndonesia { get; set; }

        public string EventThai { get; set; }

        public string EventKorean { get; set; }

        public string EventVietnamese { get; set; }

        public string EventJapanese { get; set; }

        public string EventHindi { get; set; }

        public DateTime Time { get; set; }

        public int SportId { get; set; }

        public string SportName { get; set; }

        public string CountryCode { get; set; }

        public IList<OutrightOdds> OutrightOddsList { get; set; }
    }
}
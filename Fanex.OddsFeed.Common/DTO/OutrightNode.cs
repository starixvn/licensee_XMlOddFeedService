namespace Fanex.OddsFeed.Common.DTO
{
    using System;

    public class OutrightNode
    {
        public int EventId { get; set; }

        public string EventEn { get; set; }

        public string EventCn { get; set; }

        public string EventTw { get; set; }

        public string SportName { get; set; }

        public int OddsId { get; set; }

        public string OddNameEn { get; set; }

        public string OddNameCn { get; set; }

        public string OddNameTw { get; set; }

        public decimal Value { get; set; }

        public DateTime Time { get; set; }
    }
}

namespace Fanex.OddsFeed.Common.DTO
{
    using System;

    [Serializable]
    public class OutrightOdds
    {
        public int OddsId { get; set; }

        public string OddNameEn { get; set; }

        public string OddNameTw { get; set; }

        public string OddNameCn { get; set; }

        public decimal Value { get; set; }
    }
}

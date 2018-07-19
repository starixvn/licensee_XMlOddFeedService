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

        public string OddNameIndonesia { get; set; }

        public string OddNameThai { get; set; }

        public string OddNameKorean { get; set; }

        public string OddNameVietnamese { get; set; }

        public string OddNameJapanese { get; set; }

        public string OddNameHindi { get; set; }

        public decimal Value { get; set; }
    }
}

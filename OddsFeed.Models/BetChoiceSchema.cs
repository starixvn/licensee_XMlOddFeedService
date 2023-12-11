namespace OddsFeed.Models
{
    using System;

    [Serializable]
    public class BetChoiceSchema
    {
        public string MatchAttribute { get; set; }

        public string DataAttribute { get; set; }

        public bool NotConvert { get; set; }

        public bool IsResource { get; set; }

        public bool IsResourceX { get; set; }

        public bool IsResourceY { get; set; }
    }
}
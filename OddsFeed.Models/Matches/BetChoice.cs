namespace OddsFeed.Models.Matches
{
    using System;

    [Serializable]
    public class BetChoice
    {
        public string Code { get; set; }

        public string Value { get; set; }

        public bool NotConvert { get; set; }
    }
}
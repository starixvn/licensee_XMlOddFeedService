namespace OddsFeed.Models.Schemas
{
    using System;

    [Serializable]
    public class BetChoiceDescription
    {
        public string Attribute { get; set; }

        public string Description { get; set; }
    }
}
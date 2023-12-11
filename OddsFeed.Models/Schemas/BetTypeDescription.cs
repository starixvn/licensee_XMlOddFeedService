namespace OddsFeed.Models.Schemas
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class BetTypeDescription
    {
        public int BetTypeId { get; set; }

        public string BetTypeName { get; set; }

        public string Tag { get; set; }

        public IEnumerable<BetChoiceDescription> BetChoices { get; set; }

        public string ExtraDescription { get; set; }
    }
}
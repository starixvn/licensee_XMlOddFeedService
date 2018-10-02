using System;

namespace Fanex.OddsFeed.Common.Schemas
{
    [Serializable]
    public class BetChoiceDescription
    {
        public string Attribute { get; set; }

        public string Description { get; set; }
    }
}

using System.Collections.Generic;

namespace ExcelToJson
{
    public class JasonDescItem
    {
        public int BetTypeId { get; set; }

        public string BetTypeName { get; set; }

        public string Tag { get; set; }

        public List<BetChoiceDesc> BetChoices { get; set; }

        public string ExtraDescription { get; set; }
    }
}

using System.Collections.Generic;

namespace ExcelToJson
{
    public class JsonItem
    {
        public int BettypeId { get; set; }
        public string Tag { get; set; }
        public ReserveItem Reserve { get; set; }

        public bool ShouldSerializeReserve()
        {
            return Reserve != null;
        }

        public List<BetChoice> BetChoices { get; set; }
    }
}

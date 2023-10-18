namespace Fanex.OddsFeed.Models
{
    [Serializable]
    public class BetTypeSchema
    {
        public int BetTypeId { get; set; }

        public string Tag { get; set; }

        public decimal Value { get; set; }

        public ReserveSchema Reserve { get; set; }

        public bool IsOddsFlex { get; set; }

        public bool IsChildBetType { get; set; }

        public IEnumerable<BetChoiceSchema> BetChoices { get; set; }
    }
}
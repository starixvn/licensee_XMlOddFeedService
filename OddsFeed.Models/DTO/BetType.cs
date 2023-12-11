namespace OddsFeed.Models.DTO
{
    public class BetType
    {
        public int BetTypeId { get; set; }

        public string BetTypeName { get; set; }

        public bool IsChildBetType { get; set; }
    }
}
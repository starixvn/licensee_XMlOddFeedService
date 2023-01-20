namespace Fanex.OddsFeed.Models.DTO
{
    public class League
    {
        public int LeagueId { get; set; }

        public string Name_en { get; set; }

        public string Name_cs { get; set; }

        public string Name_ct { get; set; }

        public string SportName { get; set; }

        public string CountryId { get; set; }

        public string StartDate { get; set; }

        public bool IsActive { get; set; }
    }
}

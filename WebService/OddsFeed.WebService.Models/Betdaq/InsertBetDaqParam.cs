namespace OddsFeed.WebService.Models.Betdaq
{
    public class InsertBetDaqParam
    {
        public int OddsId { get; set; }
        public decimal hdp1 { get; set; }
        public decimal hdp2 { get; set; }
        public string betteam { get; set; }
        public decimal odds { get; set; }
        public decimal stake { get; set; }
        public int betdaqid { get; set; }
        public string bet_ip { get; set; }
    }
}
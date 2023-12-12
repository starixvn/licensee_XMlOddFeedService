namespace OddsFeed.WebService.Models.Betdaq
{
    public class BetdaqOdd
    {
        public byte IsBasketball { get; set; }
        public int matchid { get; set; }
        public int oddsid { get; set; }
        public short bettype { get; set; }
        public string KickOffTime { get; set; }

        public string LeagueName { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }

        public float? hdp1 { get; set; }
        public float? hdp2 { get; set; }
        public float? odds1a { get; set; }
        public float? odds2a { get; set; }

        public float com1 { get; set; }
        public float comx { get; set; }
        public float com2 { get; set; }

        public float cs00 { get; set; }
        public float cs11 { get; set; }
        public float cs22 { get; set; }
        public float cs33 { get; set; }
        public float cs44 { get; set; }

        public float cs10 { get; set; }
        public float cs20 { get; set; }
        public float cs30 { get; set; }
        public float cs40 { get; set; }
        public float cs50 { get; set; }

        public float cs01 { get; set; }
        public float cs02 { get; set; }
        public float cs03 { get; set; }
        public float cs04 { get; set; }
        public float cs05 { get; set; }

        public float cs12 { get; set; }
        public float cs13 { get; set; }
        public float cs14 { get; set; }
        public float cs21 { get; set; }
        public float cs31 { get; set; }
        public float cs41 { get; set; }

        public float cs23 { get; set; }
        public float cs24 { get; set; }
        public float cs32 { get; set; }
        public float cs42 { get; set; }

        public float cs34 { get; set; }
        public float cs43 { get; set; }

        public float minbet { get; set; }
        public float maxbet { get; set; }
    }
}
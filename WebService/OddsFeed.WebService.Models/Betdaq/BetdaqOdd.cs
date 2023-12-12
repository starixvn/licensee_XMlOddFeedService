namespace OddsFeed.WebService.Models.Betdaq
{
    public class BetdaqOdd
    {
        public byte IsBasketball { get; set; }
        public int MatchID { get; set; }
        public int OddsID { get; set; }
        public short Bettype { get; set; }
        public string KickOffTime { get; set; }

        public string LeagueName { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }

        public float? Hdp1 { get; set; }
        public float? Hdp2 { get; set; }
        public float? Odds1a { get; set; }
        public float? Odds2a { get; set; }

        public float Com1 { get; set; }
        public float Comx { get; set; }
        public float Com2 { get; set; }

        public float Cs00 { get; set; }
        public float Cs11 { get; set; }
        public float Cs22 { get; set; }
        public float Cs33 { get; set; }
        public float Cs44 { get; set; }

        public float Cs10 { get; set; }
        public float Cs20 { get; set; }
        public float Cs30 { get; set; }
        public float Cs40 { get; set; }
        public float Cs50 { get; set; }

        public float Cs01 { get; set; }
        public float Cs02 { get; set; }
        public float Cs03 { get; set; }
        public float Cs04 { get; set; }
        public float Cs05 { get; set; }

        public float Cs12 { get; set; }
        public float Cs13 { get; set; }
        public float Cs14 { get; set; }
        public float Cs21 { get; set; }
        public float Cs31 { get; set; }
        public float Cs41 { get; set; }

        public float Cs23 { get; set; }
        public float Cs24 { get; set; }
        public float Cs32 { get; set; }
        public float Cs42 { get; set; }

        public float Cs34 { get; set; }
        public float Cs43 { get; set; }

        public float Minbet { get; set; }
        public float Maxbet { get; set; }
    }
}
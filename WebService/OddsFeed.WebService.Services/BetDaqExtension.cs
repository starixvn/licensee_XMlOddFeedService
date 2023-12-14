namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Text;
    using OddsFeed.WebService.Models.Betdaq;

    public static class BetDaqExtension
    {
        private const byte Handicap = 1;
        private const byte OverUnder = 3;
        private const byte CorrectScore = 4;
        private const byte Handicap1H = 7;
        private const byte OverUnder1H = 8;
        private const byte Outright = 10;
        private const byte TotalCorners = 11;
        private const byte HalfTime_FullTime = 16;
        private const string SpecialCharacter = "\u002C";

        public static string ToCSV(this IEnumerable<BetdaqOdd> drs, string separateRow)
        {
            var sb = new StringBuilder();
            foreach (BetdaqOdd row in drs)
            {
                var param = new[]
                {
                    row.MatchID.ToString(),
                    row.KickOffTime,
                    row.LeagueName.Replace(",", SpecialCharacter),
                    row.Home.Replace(",", SpecialCharacter),
                    row.Away.Replace(",", SpecialCharacter) + separateRow
                };

                sb.Append(string.Join(",", param));
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds(this IEnumerable<BetdaqOdd> drs, string separateRow)
        {
            var sb = new StringBuilder();
            string[] param;

            foreach (BetdaqOdd row in drs)
            {
                if (
                    row.Bettype == Handicap
                    || row.Bettype == OverUnder
                    || row.Bettype == Handicap1H
                    || row.Bettype == OverUnder1H)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(),
                        row.OddsID.ToString(),
                        row.Bettype.ToString(),
                        row.Hdp1.ToString(),
                        row.Hdp2.ToString(),
                        row.Odds1a.ToString(),
                        row.Odds2a.ToString(),
                        row.Minbet.ToString(),
                        row.Maxbet + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.Bettype == Outright || row.Bettype == TotalCorners)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(),
                        row.OddsID.ToString(),
                        row.Bettype.ToString(),
                        row.Com1.ToString(),
                        row.Comx.ToString(),
                        row.Com2.ToString(),
                        row.Minbet.ToString(),
                        row.Maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds_2(this IEnumerable<BetdaqOdd> drs, string separateRow)
        {
            var sb = new StringBuilder();
            string[] param;

            foreach (BetdaqOdd row in drs)
            {
                if (row.Bettype == CorrectScore)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(), row.OddsID.ToString(), row.Bettype.ToString(),
                        row.Cs00.ToString(), row.Cs11.ToString(), row.Cs22.ToString(), row.Cs33.ToString(),
                        row.Cs10.ToString(), row.Cs20.ToString(), row.Cs30.ToString(),
                        row.Cs21.ToString(), row.Cs31.ToString(), row.Cs32.ToString(),
                        row.Cs01.ToString(), row.Cs02.ToString(), row.Cs03.ToString(),
                        row.Cs12.ToString(), row.Cs13.ToString(), row.Cs23.ToString(),
                        row.Minbet.ToString(), row.Maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.Bettype == HalfTime_FullTime)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(), row.OddsID.ToString(), row.Bettype.ToString(),
                        row.Cs11.ToString(), row.Cs10.ToString(), row.Cs12.ToString(),
                        row.Cs01.ToString(), row.Cs00.ToString(), row.Cs02.ToString(),
                        row.Cs21.ToString(), row.Cs20.ToString(), row.Cs22.ToString(),
                        row.Minbet.ToString(), row.Maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }
    }
}
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
                    row.matchid.ToString(),
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
                    row.bettype == Handicap
                    || row.bettype == OverUnder
                    || row.bettype == Handicap1H
                    || row.bettype == OverUnder1H)
                {
                    param = new[]
                    {
                        row.matchid.ToString(),
                        row.oddsid.ToString(),
                        row.bettype.ToString(),
                        row.hdp1.ToString(),
                        row.hdp2.ToString(),
                        row.odds1a.ToString(),
                        row.odds2a.ToString(),
                        row.minbet.ToString(),
                        row.maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.bettype == Outright || row.bettype == TotalCorners)
                {
                    param = new[]
                    {
                        row.matchid.ToString(),
                        row.oddsid.ToString(),
                        row.bettype.ToString(),
                        row.com1.ToString(),
                        row.comx.ToString(),
                        row.com2.ToString(),
                        row.minbet.ToString(),
                        row.maxbet.ToString() + separateRow
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
                if (row.bettype == CorrectScore)
                {
                    param = new[]
                    {
                        row.matchid.ToString(), row.oddsid.ToString(), row.bettype.ToString(),
                        row.cs00.ToString(), row.cs11.ToString(), row.cs22.ToString(), row.cs33.ToString(),
                        row.cs10.ToString(), row.cs20.ToString(), row.cs30.ToString(),
                        row.cs21.ToString(), row.cs31.ToString(), row.cs32.ToString(),
                        row.cs01.ToString(), row.cs02.ToString(), row.cs03.ToString(),
                        row.cs12.ToString(), row.cs13.ToString(), row.cs23.ToString(),
                        row.minbet.ToString(), row.maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.bettype == HalfTime_FullTime)
                {
                    param = new[]
                    {
                        row.matchid.ToString(), row.oddsid.ToString(), row.bettype.ToString(),
                        row.cs11.ToString(), row.cs10.ToString(), row.cs12.ToString(),
                        row.cs01.ToString(), row.cs00.ToString(), row.cs02.ToString(),
                        row.cs21.ToString(), row.cs20.ToString(), row.cs22.ToString(),
                        row.minbet.ToString(), row.maxbet.ToString() + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }
    }
}
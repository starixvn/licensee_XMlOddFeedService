namespace OddsFeed.WebService.Services
{
    using System.Collections.Generic;
    using System.Globalization;
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
            foreach (var row in drs)
            {
                var param = new[]
                {
                    row.MatchID.ToString(CultureInfo.InvariantCulture),
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

            foreach (var row in drs)
            {
                if (
                    row.Bettype == Handicap
                    || row.Bettype == OverUnder
                    || row.Bettype == Handicap1H
                    || row.Bettype == OverUnder1H)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(CultureInfo.InvariantCulture),
                        row.OddsID.ToString(CultureInfo.InvariantCulture),
                        row.Bettype.ToString(CultureInfo.InvariantCulture),
                        row.Hdp1?.ToString(CultureInfo.InvariantCulture),
                        row.Hdp2?.ToString(CultureInfo.InvariantCulture),
                        row.Odds1a?.ToString(CultureInfo.InvariantCulture),
                        row.Odds2a?.ToString(CultureInfo.InvariantCulture),
                        row.Minbet.ToString(CultureInfo.InvariantCulture),
                        row.Maxbet.ToString(CultureInfo.InvariantCulture) + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.Bettype == Outright || row.Bettype == TotalCorners)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(CultureInfo.InvariantCulture),
                        row.OddsID.ToString(CultureInfo.InvariantCulture),
                        row.Bettype.ToString(CultureInfo.InvariantCulture),
                        row.Com1.ToString(CultureInfo.InvariantCulture),
                        row.Comx.ToString(CultureInfo.InvariantCulture),
                        row.Com2.ToString(CultureInfo.InvariantCulture),
                        row.Minbet.ToString(CultureInfo.InvariantCulture),
                        row.Maxbet.ToString(CultureInfo.InvariantCulture) + separateRow
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

            foreach (var row in drs)
            {
                if (row.Bettype == CorrectScore)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(CultureInfo.InvariantCulture),
                        row.OddsID.ToString(CultureInfo.InvariantCulture),
                        row.Bettype.ToString(CultureInfo.InvariantCulture),
                        row.Cs00.ToString(CultureInfo.InvariantCulture),
                        row.Cs11.ToString(CultureInfo.InvariantCulture),
                        row.Cs22.ToString(CultureInfo.InvariantCulture),
                        row.Cs33.ToString(CultureInfo.InvariantCulture),
                        row.Cs10.ToString(CultureInfo.InvariantCulture),
                        row.Cs20.ToString(CultureInfo.InvariantCulture),
                        row.Cs30.ToString(CultureInfo.InvariantCulture),
                        row.Cs21.ToString(CultureInfo.InvariantCulture),
                        row.Cs31.ToString(CultureInfo.InvariantCulture),
                        row.Cs32.ToString(CultureInfo.InvariantCulture),
                        row.Cs01.ToString(CultureInfo.InvariantCulture),
                        row.Cs02.ToString(CultureInfo.InvariantCulture),
                        row.Cs03.ToString(CultureInfo.InvariantCulture),
                        row.Cs12.ToString(CultureInfo.InvariantCulture),
                        row.Cs13.ToString(CultureInfo.InvariantCulture),
                        row.Cs23.ToString(CultureInfo.InvariantCulture),
                        row.Minbet.ToString(CultureInfo.InvariantCulture),
                        row.Maxbet.ToString(CultureInfo.InvariantCulture) + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (row.Bettype == HalfTime_FullTime)
                {
                    param = new[]
                    {
                        row.MatchID.ToString(CultureInfo.InvariantCulture),
                        row.OddsID.ToString(CultureInfo.InvariantCulture),
                        row.Bettype.ToString(CultureInfo.InvariantCulture),
                        row.Cs11.ToString(CultureInfo.InvariantCulture),
                        row.Cs10.ToString(CultureInfo.InvariantCulture),
                        row.Cs12.ToString(CultureInfo.InvariantCulture),
                        row.Cs01.ToString(CultureInfo.InvariantCulture),
                        row.Cs00.ToString(CultureInfo.InvariantCulture),
                        row.Cs02.ToString(CultureInfo.InvariantCulture),
                        row.Cs21.ToString(CultureInfo.InvariantCulture),
                        row.Cs20.ToString(CultureInfo.InvariantCulture),
                        row.Cs22.ToString(CultureInfo.InvariantCulture),
                        row.Minbet.ToString(CultureInfo.InvariantCulture),
                        row.Maxbet.ToString(CultureInfo.InvariantCulture) + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }
    }
}
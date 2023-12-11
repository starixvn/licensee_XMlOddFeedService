namespace OddsFeed.WebService.Extensions
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;

    public static class DataExtension
    {
        private const int FirstBetType = 1;
        private const int ThirdBetType = 3;
        private const int ForthBetType = 4;
        private const int SeventhBetType = 7;
        private const int EighthBetType = 8;
        private const int TenthBetType = 10;
        private const int EleventhBetType = 11;
        private const int SixteenthBetType = 16;
        private const string MatchId = "matchid";
        private const string SpecialCharacter = "\u002C";
        private const string OddsId = "oddsid";
        private const string BetType = "bettype";
        private const string MinBet = "minbet";
        private const string MaxBet = "maxbet";

        public static string ToCSV(this DataTable dt, bool isPrintHeader, string separateRow)
        {
            var sb = new StringBuilder();
            if (isPrintHeader)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.Append(string.Join(",", columnNames));
                sb.Append("__");
            }

            foreach (DataRow row in dt.Rows)
            {
                var param = new[]
                {
                    row[MatchId].ToString(), row["kickofftime"].ToString(),
                    row["leaguename"].ToString().Replace(",", SpecialCharacter), row["home"].ToString().Replace(",", SpecialCharacter),
                    row["away"].ToString().Replace(",", SpecialCharacter) + separateRow
                };

                sb.Append(string.Join(",", param));
            }

            return sb.ToString();
        }

        public static string ToCSV(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();
            foreach (DataRow row in drs)
            {
                var param = new[]
                {
                    row[MatchId].ToString(), row["kickofftime"].ToString(),
                    row["leaguename"].ToString().Replace(",", SpecialCharacter), row["home"].ToString().Replace(",", SpecialCharacter),
                    row["away"].ToString().Replace(",", SpecialCharacter) + separateRow
                };

                sb.Append(string.Join(",", param));
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();
            string[] param;

            foreach (DataRow row in drs)
            {
                var betType = Convert.ToInt16(row[BetType]);
                if (
                    betType == FirstBetType
                    || betType == ThirdBetType
                    || betType == SeventhBetType
                    || betType == EighthBetType)
                {
                    param = new[]
                    {
                        row[MatchId].ToString(),
                        row[OddsId].ToString(),
                        row[BetType].ToString(),
                        row["hdp1"].ToString(),
                        row["hdp2"].ToString(),
                        row["odds1a"].ToString(),
                        row["odds2a"].ToString(),
                        row[MinBet].ToString(),
                        row[MaxBet] + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (betType == TenthBetType || betType == EleventhBetType)
                {
                    param = new[]
                    {
                        row[MatchId].ToString(),
                        row[OddsId].ToString(),
                        row[BetType].ToString(),
                        row["com1"].ToString(),
                        row["comX"].ToString(),
                        row["com2"].ToString(),
                        row[MinBet].ToString(),
                        row[MaxBet] + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds_2(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();
            string[] param;

            foreach (DataRow row in drs)
            {
                var betType = Convert.ToInt16(row[BetType]);
                if (betType == ForthBetType)
                {
                    param = new[]
                    {
                        row[MatchId].ToString(), row[OddsId].ToString(), row[BetType].ToString(),
                        row["cs00"].ToString(), row["cs11"].ToString(), row["cs22"].ToString(), row["cs33"].ToString(),
                        row["cs10"].ToString(), row["cs20"].ToString(), row["cs30"].ToString(),
                        row["cs21"].ToString(), row["cs31"].ToString(), row["cs32"].ToString(),
                        row["cs01"].ToString(), row["cs02"].ToString(), row["cs03"].ToString(),
                        row["cs12"].ToString(), row["cs13"].ToString(), row["cs23"].ToString(),
                        row[MinBet].ToString(), row[MaxBet] + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }

                if (betType == SixteenthBetType)
                {
                    param = new[]
                    {
                        row[MatchId].ToString(), row[OddsId].ToString(), row[BetType].ToString(),
                        row["cs11"].ToString(), row["cs10"].ToString(), row["cs12"].ToString(),
                        row["cs01"].ToString(), row["cs00"].ToString(), row["cs02"].ToString(),
                        row["cs21"].ToString(), row["cs20"].ToString(), row["cs22"].ToString(),
                        row[MinBet].ToString(), row[MaxBet] + separateRow
                    };

                    sb.Append(string.Join(",", param));
                }
            }

            return sb.ToString();
        }
    }
}
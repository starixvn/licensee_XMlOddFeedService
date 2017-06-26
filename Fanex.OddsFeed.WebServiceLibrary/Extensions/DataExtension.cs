namespace Fanex.OddsFeed.WebServiceLibrary.Extensions
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;

    public static class DataExtension
    {
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
                var param = new string[]
                                                    {
                                                        row["matchid"].ToString(), row["kickofftime"].ToString(),
                                                        row["leaguename"].ToString().Replace(",", "\u002C"), row["home"].ToString().Replace(",", "\u002C"),
                                                        row["away"].ToString().Replace(",", "\u002C") + separateRow
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
                var param = new string[]
                                                    {
                                                        row["matchid"].ToString(), row["kickofftime"].ToString(),
                                                        row["leaguename"].ToString().Replace(",", "\u002C"), row["home"].ToString().Replace(",", "\u002C"),
                                                        row["away"].ToString().Replace(",", "\u002C") + separateRow
                                                    };

                sb.Append(string.Join(",", param));
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();

            var param = new string[] { };
            foreach (DataRow row in drs)
            {
                switch (Convert.ToInt16(row["bettype"]))
                {
                    case 1:
                    case 3:
                    case 7:
                    case 8:
                        param = new string[]
                                                    {
                                                        row["matchid"].ToString(),
                                                        row["oddsid"].ToString(),
                                                        row["bettype"].ToString(),
                                                        row["hdp1"].ToString(),
                                                        row["hdp2"].ToString(),
                                                        row["odds1a"].ToString(),
                                                        row["odds2a"].ToString(),
                                                        row["minbet"].ToString(),
                                                        row["maxbet"].ToString() + separateRow
                                                    };
                        sb.Append(string.Join(",", param));
                        break;
                    case 10:
                    case 11:
                        param = new string[]
                                                    {
                                                        row["matchid"].ToString(),
                                                        row["oddsid"].ToString(),
                                                        row["bettype"].ToString(),
                                                        row["com1"].ToString(),
                                                        row["comX"].ToString(),
                                                        row["com2"].ToString(),
                                                        row["minbet"].ToString(),
                                                        row["maxbet"].ToString() + separateRow
                                                    };
                        sb.Append(string.Join(",", param));
                        break;
                }
            }

            return sb.ToString();
        }

        public static string ToCSV_Odds_2(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();
            var param = new string[] { };

            foreach (DataRow row in drs)
            {
                switch (Convert.ToInt16(row["bettype"]))
                {
                    case 4:
                        param = new string[]
                                                        {
                                                            row["matchid"].ToString(), row["oddsid"].ToString(), row["bettype"].ToString(),
                                                            row["cs00"].ToString(), row["cs11"].ToString(), row["cs22"].ToString(), row["cs33"].ToString(),
                                                            row["cs10"].ToString(), row["cs20"].ToString(), row["cs30"].ToString(),
                                                            row["cs21"].ToString(), row["cs31"].ToString(), row["cs32"].ToString(),
                                                            row["cs01"].ToString(), row["cs02"].ToString(), row["cs03"].ToString(),
                                                            row["cs12"].ToString(), row["cs13"].ToString(), row["cs23"].ToString(),
                                                            row["minbet"].ToString(), row["maxbet"].ToString() + separateRow
                                                        };
                        sb.Append(string.Join(",", param));
                        break;
                    case 16:
                        param = new string[]
                                                        {
                                                            row["matchid"].ToString(), row["oddsid"].ToString(), row["bettype"].ToString(),
                                                            row["cs11"].ToString(), row["cs10"].ToString(), row["cs12"].ToString(),
                                                            row["cs01"].ToString(), row["cs00"].ToString(), row["cs02"].ToString(),
                                                            row["cs21"].ToString(), row["cs20"].ToString(), row["cs22"].ToString(),
                                                            row["minbet"].ToString(), row["maxbet"].ToString() + separateRow
                                                        };
                        sb.Append(string.Join(",", param));
                        break;
                }
            }

            return sb.ToString();
        }

        public static string FooBetToCSV_Odds(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();
            var param = new string[] { };

            foreach (DataRow row in drs)
            {
                switch (Convert.ToInt16(row["bettype"]))
                {
                    case 1:
                    case 3:
                    case 7:
                    case 8:
                        param = new string[]
                                                    {
                                                        row["matchid"].ToString(),
                                                        row["oddsid"].ToString(),
                                                        row["bettype"].ToString(),
                                                        row["maxbet"].ToString(),
                                                        row["hdp1"].ToString(),
                                                        row["hdp2"].ToString(),
                                                        row["odds1a"].ToString(),
                                                        row["odds2a"].ToString() + separateRow
                                                    };
                        sb.Append(string.Join(",", param));
                        break;
                    case 10:
                    case 11:
                        param = new string[]
                                                    {
                                                        row["matchid"].ToString(),
                                                        row["oddsid"].ToString(),
                                                        row["bettype"].ToString(),
                                                        row["maxbet"].ToString(),
                                                        row["com1"].ToString(),
                                                        row["comX"].ToString(),
                                                        row["com2"].ToString() + separateRow
                                                    };
                        sb.Append(string.Join(",", param));
                        break;
                }
            }

            return sb.ToString();
        }

        public static string FooBetToCSV_Odds_2(this DataRow[] drs, string separateRow)
        {
            var sb = new StringBuilder();

            foreach (DataRow row in drs)
            {
                switch (Convert.ToInt16(row["bettype"]))
                {
                    case 16:
                        var param = new string[]
                                    {
                                        row["matchid"].ToString(), row["oddsid"].ToString(), row["bettype"].ToString(),
                                        row["maxbet"].ToString(),
                                        row["cs11"].ToString(), row["cs10"].ToString(), row["cs12"].ToString(),
                                        row["cs01"].ToString(), row["cs00"].ToString(), row["cs02"].ToString(),
                                        row["cs21"].ToString(), row["cs20"].ToString(), row["cs22"].ToString() + separateRow
                                    };
                        sb.Append(string.Join(",", param));
                        break;
                }
            }

            return sb.ToString();
        }
    }
}

namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Data;
    using System.Security;
    using System.Text;
    using Common.Formatter;
    using Fanex.OddsFeed.Common;

    public class BetRadarService : IBetRadarService
    {
        public string GetXmlDocument()
        {
            var oddsFeedNode = new StringBuilder("<List><!-- Date is based on Malaysian time -->");

            using (var oddsFeedData = DbAccess.GetBetRadar())
            {
                foreach (DataRow row in oddsFeedData.Rows)
                {
                    AppendOddsObject(oddsFeedNode, row);
                }
            }

            oddsFeedNode.Append("</List>");

            return oddsFeedNode.ToString();
        }

        private static void AppendOddsObject(StringBuilder oddsFeedNode, DataRow row)
        {
            oddsFeedNode.Append("<OddsObject type=\"regular\">");
            oddsFeedNode.Append($"<Sport>{SecurityElement.Escape(row["sporttypename"].ToString())}</Sport>");
            oddsFeedNode.Append($"<Tournament>{SecurityElement.Escape(row["leaguename"].ToString())}</Tournament>");
            oddsFeedNode.Append($"<MatchId>{row["matchid"]}</MatchId>");
            oddsFeedNode.Append($"<HomeTeam>{SecurityElement.Escape(row["home"].ToString())}</HomeTeam>");
            oddsFeedNode.Append($"<AwayTeam>{SecurityElement.Escape(row["away"].ToString())}</AwayTeam>");
            oddsFeedNode.Append($"<Date>{row["kickofftime"]}</Date>");

            if (!row.IsNull("OddsID_5"))
            {
                oddsFeedNode.Append($"<!-- Threeway odds -->");
                oddsFeedNode.Append($"<OddsData type=\"3w\"><AC>3</AC>");
                oddsFeedNode.Append($"<HomeOdds>{NumberFormatter.FormatOdds(row["com1_5"])}</HomeOdds>");
                oddsFeedNode.Append($"<DrawOdds>{NumberFormatter.FormatOdds(row["comx_5"])}</DrawOdds>");
                oddsFeedNode.Append($"<AwayOdds>{NumberFormatter.FormatOdds(row["com2_5"])}</AwayOdds>");
                oddsFeedNode.Append("</OddsData>");
            }

            if (!row.IsNull("OddsID_1"))
            {
                oddsFeedNode.Append($"<!-- Handicap odds -->");
                oddsFeedNode.Append($"<OddsData type=\"handicap\"><AC>1</AC>");
                oddsFeedNode.Append($"<Handicap>{row["hdp1_1"]}:{row["hdp2_1"]}</Handicap>");
                oddsFeedNode.Append($"<HomeOdds>{NumberFormatter.FormatOdds(row["odds1hka_1"])}</HomeOdds>");
                oddsFeedNode.Append($"<AwayOdds>{NumberFormatter.FormatOdds(row["odds2hka_1"])}</AwayOdds>");
                oddsFeedNode.Append("</OddsData>");
            }

            if (!row.IsNull("OddsID_3"))
            {
                oddsFeedNode.Append($"<!-- Total odds(over/under) -->");
                oddsFeedNode.Append($"<OddsData type=\"total\"><AC>1</AC>");
                oddsFeedNode.Append($"<Total>{NumberFormatter.FormatOdds(row["Hdp1_3"])}</Total>");
                oddsFeedNode.Append($"<UnderOdds>{NumberFormatter.FormatOdds(row["odds1hka_3"])}</UnderOdds>");
                oddsFeedNode.Append($"<OverOdds>{NumberFormatter.FormatOdds(row["odds2hka_3"])}</OverOdds>");
                oddsFeedNode.Append("</OddsData>");
            }

            oddsFeedNode.Append("</OddsObject>");
        }
    }
}

namespace OddsFeed.WebService.Services
{
    using System.Data;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using OddsFeed.Repository;
    using OddsFeed.WebService.Criteria;
    using OddsFeed.WebService.Services.Interfaces;

    public class BetRadarService : IBetRadarService
    {
        private readonly IDbAccessRepository dbAccess;

        public BetRadarService(IDbAccessRepository dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public async Task<string> GetXmlDocumentAsync()
        {
            var oddsFeedNode = new StringBuilder("<List><!-- Date is based on Malaysian time -->");
            var criteria = new GetBetRadarCriteria();
            using (var oddsFeedData = await dbAccess.GetDataTableAsync(criteria))
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
            const string EndTagOddsData = "</OddsData>";
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
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(row["com1_5"])}</HomeOdds>");
                oddsFeedNode.Append($"<DrawOdds>{FormatOdds(row["comx_5"])}</DrawOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(row["com2_5"])}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (!row.IsNull("OddsID_1"))
            {
                oddsFeedNode.Append($"<!-- Handicap odds -->");
                oddsFeedNode.Append($"<OddsData type=\"handicap\"><AC>1</AC>");
                oddsFeedNode.Append($"<Handicap>{row["hdp1_1"]}:{row["hdp2_1"]}</Handicap>");
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(row["odds1hka_1"])}</HomeOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(row["odds2hka_1"])}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (!row.IsNull("OddsID_3"))
            {
                oddsFeedNode.Append($"<!-- Total odds(over/under) -->");
                oddsFeedNode.Append($"<OddsData type=\"total\"><AC>1</AC>");
                oddsFeedNode.Append($"<Total>{FormatOdds(row["Hdp1_3"])}</Total>");
                oddsFeedNode.Append($"<UnderOdds>{FormatOdds(row["odds1hka_3"])}</UnderOdds>");
                oddsFeedNode.Append($"<OverOdds>{FormatOdds(row["odds2hka_3"])}</OverOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (!row.IsNull("OddsID_501"))
            {
                oddsFeedNode.Append($"<!-- Match Winner odds -->");
                oddsFeedNode.Append($"<OddsData type=\"winner\"><AC>1</AC>");
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(row["odds1hka_501"])}</HomeOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(row["odds2hka_501"])}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            oddsFeedNode.Append("</OddsObject>");
        }

        private static string FormatOdds(object value)
        {
            const string OddsPattern = "F";
            if (!decimal.TryParse(value.ToString(), out decimal result))
            {
                result = 0;
            }

            return result.ToString(OddsPattern);
        }
    }
}
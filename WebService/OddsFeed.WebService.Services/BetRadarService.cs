namespace OddsFeed.WebService.Services
{
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using OddsFeed.Core.Services;
    using OddsFeed.WebService.Criteria;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Services.Interfaces;

    public class BetRadarService : IBetRadarService
    {
        private readonly IBaseService baseService;

        public BetRadarService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<string> GetXmlDocumentAsync()
        {
            var oddsFeedNode = new StringBuilder("<List><!-- Date is based on Malaysian time -->");
            var criteria = new GetBetRadarCriteria();
            var odds = await baseService.GetDataAsync<BetRadarOdd>(criteria);
            foreach (var odd in odds)
            {
                AppendOddsObject(oddsFeedNode, odd);
            }

            oddsFeedNode.Append("</List>");

            return oddsFeedNode.ToString();
        }

        private static void AppendOddsObject(StringBuilder oddsFeedNode, BetRadarOdd odd)
        {
            const string EndTagOddsData = "</OddsData>";
            oddsFeedNode.Append("<OddsObject type=\"regular\">");
            oddsFeedNode.Append($"<Sport>{SecurityElement.Escape(odd.SportTypeName)}</Sport>");
            oddsFeedNode.Append($"<Tournament>{SecurityElement.Escape(odd.LeagueName)}</Tournament>");
            oddsFeedNode.Append($"<MatchId>{odd.MatchId}</MatchId>");
            oddsFeedNode.Append($"<HomeTeam>{SecurityElement.Escape(odd.Home)}</HomeTeam>");
            oddsFeedNode.Append($"<AwayTeam>{SecurityElement.Escape(odd.Away)}</AwayTeam>");
            oddsFeedNode.Append($"<Date>{odd.KickOffTime}</Date>");

            if (odd.OddsID_5.HasValue)
            {
                oddsFeedNode.Append($"<!-- Threeway odds -->");
                oddsFeedNode.Append($"<OddsData type=\"3w\"><AC>3</AC>");
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(odd.Com1_5)}</HomeOdds>");
                oddsFeedNode.Append($"<DrawOdds>{FormatOdds(odd.ComX_5)}</DrawOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(odd.Com2_5)}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (odd.OddsID_1.HasValue)
            {
                oddsFeedNode.Append($"<!-- Handicap odds -->");
                oddsFeedNode.Append($"<OddsData type=\"handicap\"><AC>1</AC>");
                oddsFeedNode.Append($"<Handicap>{odd.Hdp1_1}:{odd.Hdp2_1}</Handicap>");
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(odd.Odds1hka_1)}</HomeOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(odd.Odds2hka_1)}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (odd.OddsID_3.HasValue)
            {
                oddsFeedNode.Append($"<!-- Total odds(over/under) -->");
                oddsFeedNode.Append($"<OddsData type=\"total\"><AC>1</AC>");
                oddsFeedNode.Append($"<Total>{FormatOdds(odd.Hdp1_3)}</Total>");
                oddsFeedNode.Append($"<UnderOdds>{FormatOdds(odd.Odds1hka_3)}</UnderOdds>");
                oddsFeedNode.Append($"<OverOdds>{FormatOdds(odd.Odds2hka_3)}</OverOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            if (odd.OddsID_501.HasValue)
            {
                oddsFeedNode.Append($"<!-- Match Winner odds -->");
                oddsFeedNode.Append($"<OddsData type=\"winner\"><AC>1</AC>");
                oddsFeedNode.Append($"<HomeOdds>{FormatOdds(odd.Odds1hka_501)}</HomeOdds>");
                oddsFeedNode.Append($"<AwayOdds>{FormatOdds(odd.Odds2hka_501)}</AwayOdds>");
                oddsFeedNode.Append(EndTagOddsData);
            }

            oddsFeedNode.Append("</OddsObject>");
        }

        private static string FormatOdds(decimal? value)
        {
            decimal result = value ?? 0;

            return result.ToString("F");
        }
    }
}
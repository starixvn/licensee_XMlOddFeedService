namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Common.DTO;

    public class EventBuilder : IEventBuilder
    {
        public void AppendOutrightsNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Outright> outrights)
        {
            oddsFeedNode.Append("<outrights>");

            foreach (var outright in outrights)
            {
                AppendEventNode(oddsFeedNode, userLanguage, outright);
            }

            oddsFeedNode.Append("</outrights>");
        }

        public void AppendMatchesNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, IEnumerable<Match> matches)
        {
            var leagues = matches.GroupBy(league => league.LeagueId);

            oddsFeedNode.Append("<matches>");

            foreach (var league in leagues)
            {
                AppendLeagueNode(oddsFeedNode, userLanguage, league.Key, league.AsEnumerable());
            }

            oddsFeedNode.Append("</matches>");
        }

        private static void AppendLeagueNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, int leagueId, IEnumerable<Match> matches)
        {
            var firstMatch = matches.First();

            AppendBeginLeague(oddsFeedNode, userLanguage, leagueId, firstMatch);

            foreach (var match in matches)
            {
                oddsFeedNode.Append($"<match id=\"{match.MatchId}\" ");

                AppendHomeTeam(oddsFeedNode, userLanguage, match);
                AppendAwayTeam(oddsFeedNode, userLanguage, match);

                oddsFeedNode.Append($"mt=\"{match.KickOfTime}\" running=\"{match.IsLive}\"/>");
            }

            oddsFeedNode.Append("</league>");
        }

        private static void AppendBeginLeague(StringBuilder oddsFeedNode, UserLanguage userLanguage, int leagueId, Match match)
        {
            oddsFeedNode.Append($"<league id=\"{leagueId}\" ");

            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"ge=\"{SecurityElement.Escape(match.LeagueEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"gt=\"{SecurityElement.Escape(match.LeagueTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"gs=\"{SecurityElement.Escape(match.LeagueCn)}\" ");
            }

            oddsFeedNode.Append($"sport=\"{SecurityElement.Escape(match.SportName)}\">");
        }

        private static void AppendHomeTeam(StringBuilder oddsFeedNode, UserLanguage userLanguage, Match match)
        {
            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"he=\"{SecurityElement.Escape(match.HomeEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"ht=\"{SecurityElement.Escape(match.HomeTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"hs=\"{SecurityElement.Escape(match.HomeCn)}\" ");
            }
        }

        private static void AppendAwayTeam(StringBuilder oddsFeedNode, UserLanguage userLanguage, Match match)
        {
            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"ae=\"{SecurityElement.Escape(match.AwayEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"at=\"{SecurityElement.Escape(match.AwayTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"as=\"{SecurityElement.Escape(match.AwayCn)}\" ");
            }
        }

        private static void AppendEventNode(StringBuilder oddsFeedNode, UserLanguage userLanguage, Outright outright)
        {
            AppendBeginEvent(oddsFeedNode, userLanguage, outright);

            foreach (OutrightOdds outrightOdds in outright.OutrightOddsList)
            {
                AppendEventTeam(oddsFeedNode, userLanguage, outrightOdds);
            }

            oddsFeedNode.Append("</event>");
        }

        private static void AppendBeginEvent(StringBuilder oddsFeedNode, UserLanguage userLanguage, Outright outright)
        {
            oddsFeedNode.Append($"<event id=\"{outright.EventId}\" ");

            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"ge=\"{SecurityElement.Escape(outright.EventEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"gt=\"{SecurityElement.Escape(outright.EventTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"gs=\"{SecurityElement.Escape(outright.EventCn)}\" ");
            }

            oddsFeedNode.Append($"mt=\"{outright.Time}\" sport=\"{SecurityElement.Escape(outright.SportName)}\">");
        }

        private static void AppendEventTeam(StringBuilder oddsFeedNode, UserLanguage userLanguage, OutrightOdds outrightOdds)
        {
            oddsFeedNode.Append($"<team ");

            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"te=\"{SecurityElement.Escape(outrightOdds.OddNameEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"tt=\"{SecurityElement.Escape(outrightOdds.OddNameTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"ts=\"{SecurityElement.Escape(outrightOdds.OddNameCn)}\" ");
            }

            oddsFeedNode.Append($"/>");
        }
    }
}
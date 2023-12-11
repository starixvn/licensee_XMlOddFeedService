namespace OddsFeed.WebService.Core.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Outright;
    using OddsFeed.WebService.Extensions;

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

            oddsFeedNode.Append($"<league id=\"{leagueId}\" ");
            MatchBuilder.AppendLeague(oddsFeedNode, firstMatch, userLanguage);

            foreach (var match in matches)
            {
                oddsFeedNode.Append($"<match id=\"{match.MatchId}\" ");

                MatchBuilder.AppendHomeTeam(oddsFeedNode, match, userLanguage);
                MatchBuilder.AppendAwayTeam(oddsFeedNode, match, userLanguage);

                oddsFeedNode.Append($"mt=\"{match.KickOffTime}\" running=\"{match.IsLive}\"/>");
            }

            oddsFeedNode.Append("</league>");
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
                oddsFeedNode.Append($"gt=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventTw, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"gs=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventCn, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"gb=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventIndonesia, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ga=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventThai, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"gk=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventKorean, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"gv=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventVietnamese, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"gj=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventJapanese, outright.EventEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"gh=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventHindi, outright.EventEn))}\" ");
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
                oddsFeedNode.Append($"tt=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameTw, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"ts=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameCn, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"tb=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameIndonesia, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ta=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameThai, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"tk=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameKorean, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"tv=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameVietnamese, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"tj=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameJapanese, outrightOdds.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"th=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOdds.OddNameHindi, outrightOdds.OddNameEn))}\" ");
            }

            oddsFeedNode.Append($"/>");
        }
    }
}
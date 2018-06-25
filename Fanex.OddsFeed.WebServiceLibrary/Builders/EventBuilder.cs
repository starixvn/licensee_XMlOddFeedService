namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using Common.DTO;
    using Common.Helpers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;

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
                oddsFeedNode.Append($"gt=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueTw, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"gs=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueCn, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"gb=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueIndonesia, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ga=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueThai, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"gk=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueKorean, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"gv=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueVietnamese, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"gj=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueJapanese, match.LeagueEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"gh=\"{SecurityElement.Escape(StringHelper.SelectName(match.LeagueHindi, match.LeagueEn))}\" ");
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
                oddsFeedNode.Append($"ht=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeTw, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"hs=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeCn, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"hb=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeIndonesia, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ha=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeThai, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"hk=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeKorean, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"hv=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeVietnamese, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"hj=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeJapanese, match.HomeEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"hh=\"{SecurityElement.Escape(StringHelper.SelectName(match.HomeHindi, match.HomeEn))}\" ");
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
                oddsFeedNode.Append($"at=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayTw, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"as=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayCn, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"ab=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayIndonesia, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"aa=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayThai, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"ak=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayKorean, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"av=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayVietnamese, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"aj=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayJapanese, match.AwayEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"ah=\"{SecurityElement.Escape(StringHelper.SelectName(match.AwayHindi, match.AwayEn))}\" ");
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
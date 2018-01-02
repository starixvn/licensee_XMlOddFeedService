namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using Common.Converters;
    using Common.DTO;
    using Filters;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;

    public class MatchBuilder : IMatchBuilder
    {
        private const int LiveStatus = 1;
        private const int DeadBallStatus = 0;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchFilter _matchFilter;

        public MatchBuilder()
        {
            _matchFilter = new MatchFilter();
        }

        public void AppendMatchNodes(
            StringBuilder oddsFeedNode,
            IList<Match> matches,
            UserLanguage userLanguage,
            OddsFeedsModel model)
        {
            if (model.IsSample)
            {
                AppendSampleMatchNode(oddsFeedNode, matches, userLanguage, model);
            }
            else
            {
                foreach (var match in matches.Where(m => _matchFilter.FilterMatch(m, model)))
                {
                    var matchOddsList = match.MatchOddsList.Where(odds => _matchFilter.FilterBetType(odds, model));

                    if (matchOddsList.Any())
                    {
                        AppendMatchNode(oddsFeedNode, match, matchOddsList, userLanguage, model);
                    }
                }
            }
        }

        private void AppendSampleMatchNode(
            StringBuilder oddsFeedNode,
            IList<Match> matches,
            UserLanguage userLanguage,
            OddsFeedsModel model)
        {
            if (matches.Any())
            {
                AppendMatchNode(oddsFeedNode, matches.FirstOrDefault(), matches.FirstOrDefault().MatchOddsList, userLanguage, model);
            }
        }

        private void AppendMatchNode(
            StringBuilder oddsFeedNode,
            Match match,
            IEnumerable<MatchOdds> matchOddsList,
            UserLanguage userLanguage,
            OddsFeedsModel model)
        {
            AppendBeginMatch(oddsFeedNode, match, userLanguage);
            AppendBookieNode(oddsFeedNode, match, matchOddsList, model);
            oddsFeedNode.Append("</match>");
        }

        private static void AppendBeginMatch(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
        {
            var running = match.IsLive ? 1 : 0;

            oddsFeedNode.Append($"<match id=\"{match.MatchId}\" ");

            AppendLeague(oddsFeedNode, match, userLanguage);
            AppendHomeTeam(oddsFeedNode, match, userLanguage);
            AppendAwayTeam(oddsFeedNode, match, userLanguage);

            oddsFeedNode.Append($"mt=\"{match.KickOfTime}\" sport=\"{SecurityElement.Escape(match.SportName)}\" running=\"{running}\">");
        }

        private static void AppendLeague(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
        {
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

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"gb=\"{SecurityElement.Escape(match.LeagueIndonesia)}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ga=\"{SecurityElement.Escape(match.LeagueThai)}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"gk=\"{SecurityElement.Escape(match.LeagueKorean)}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"gv=\"{SecurityElement.Escape(match.LeagueVietnamese)}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"gj=\"{SecurityElement.Escape(match.LeagueJapanese)}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"gh=\"{SecurityElement.Escape(match.LeagueHindi)}\" ");
            }
        }

        private static void AppendHomeTeam(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
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

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"hb=\"{SecurityElement.Escape(match.HomeIndonesia)}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ha=\"{SecurityElement.Escape(match.HomeThai)}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"hk=\"{SecurityElement.Escape(match.HomeKorean)}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"hv=\"{SecurityElement.Escape(match.HomeVietnamese)}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"hj=\"{SecurityElement.Escape(match.HomeJapanese)}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"hh=\"{SecurityElement.Escape(match.HomeHindi)}\" ");
            }
        }

        private static void AppendAwayTeam(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
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

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"ab=\"{SecurityElement.Escape(match.AwayIndonesia)}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"aa=\"{SecurityElement.Escape(match.AwayThai)}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"ak=\"{SecurityElement.Escape(match.AwayKorean)}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"av=\"{SecurityElement.Escape(match.AwayVietnamese)}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"aj=\"{SecurityElement.Escape(match.AwayJapanese)}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"ah=\"{SecurityElement.Escape(match.AwayHindi)}\" ");
            }
        }

        private void AppendBookieNode(StringBuilder matchesNode, Match match, IEnumerable<MatchOdds> matchOddsList, OddsFeedsModel model)
        {
            matchesNode.Append($"<bookie name=\"{model.BookieName}\" simp=\"{model.BookieName}\" trad=\"{model.BookieName}\">");
            AppendOddNodeList(matchesNode, matchOddsList, model.UserProfile.OddRegion);
            matchesNode.Append("</bookie>");
        }

        private void AppendOddNodeList(StringBuilder oddsFeedNode, IEnumerable<MatchOdds> matchOddsNodes, int oddsRegion)
        {
            foreach (var matchOddsNode in matchOddsNodes)
            {
                AppendOdd(oddsFeedNode, matchOddsNode, oddsRegion);
            }
        }

        private static void AppendOdd(StringBuilder oddsFeedNode, MatchOdds matchOddsNode, int oddsRegion)
        {
            oddsFeedNode.Append($"<odd t=\"{matchOddsNode.Tag}\"");

            foreach (var betChoice in matchOddsNode.BetChoices)
            {
                oddsFeedNode.Append($" {betChoice.Code}=\"{ConvertOdds(oddsRegion, betChoice)}\"");
            }

            oddsFeedNode.Append($" time=\"{matchOddsNode.Time}\" timestamp=\"{matchOddsNode.TimeStamp}\" />");
        }

        private static string ConvertOdds(int oddsRegion, BetChoice betChoice)
        {
            return betChoice.NotConvert ? betChoice.Value : OddsConverter.ConvertOdds(oddsRegion, Convert.ToDecimal(betChoice.Value)).ToString();
        }
    }
}
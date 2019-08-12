namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Common.Converters;
    using Common.DTO;
    using Common.Helpers;
    using Filters;
    using Models;

    public class MatchBuilder : IMatchBuilder
    {
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

        private static void AppendSampleMatchNode(
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

        private static void AppendMatchNode(
            StringBuilder oddsFeedNode,
            Match match,
            IEnumerable<MatchOdds> matchOddsList,
            UserLanguage userLanguage,
            OddsFeedsModel model)
        {
            AppendBeginMatch(oddsFeedNode, match, userLanguage, model);
            AppendBookieNode(oddsFeedNode, matchOddsList, model);
            oddsFeedNode.Append("</match>");
        }

        private static void AppendBeginMatch(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage, OddsFeedsModel model)
        {
            var running = match.IsLive ? 1 : 0;

            oddsFeedNode.Append($"<match id=\"{match.MatchId}\" ");

            AppendLeague(oddsFeedNode, match, userLanguage);
            AppendHomeTeam(oddsFeedNode, match, userLanguage);
            AppendAwayTeam(oddsFeedNode, match, userLanguage);

            var moreTimeStr = model.UserQuery.MoreTime ? $"gst=\"{String.Format("{0:G}", match.GlobalShowtime)}\" st=\"{String.Format("{0:G}", match.Showtime) }\"" : string.Empty;
            oddsFeedNode.Append($"mt=\"{String.Format("{0:G}", match.KickOfTime)}\" {moreTimeStr} sport=\"{SecurityElement.Escape(match.SportName)}\" running=\"{running}\">");
        }

        private static void AppendLeague(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
        {
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
        }

        private static void AppendHomeTeam(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
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

        private static void AppendAwayTeam(StringBuilder oddsFeedNode, Match match, UserLanguage userLanguage)
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

        private static void AppendBookieNode(StringBuilder matchesNode, IEnumerable<MatchOdds> matchOddsList, OddsFeedsModel model)
        {
            matchesNode.Append($"<bookie name=\"{model.BookieName}\" simp=\"{model.BookieName}\" trad=\"{model.BookieName}\">");
            AppendOddNodeList(matchesNode, matchOddsList, model.UserProfile.OddRegion);
            matchesNode.Append("</bookie>");
        }

        private static void AppendOddNodeList(StringBuilder oddsFeedNode, IEnumerable<MatchOdds> matchOddsNodes, int oddsRegion)
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
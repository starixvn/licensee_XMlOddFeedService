namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Common.Converters;
    using Common.DTO;
    using Filters;
    using Models;

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
            if(matches.Any())
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
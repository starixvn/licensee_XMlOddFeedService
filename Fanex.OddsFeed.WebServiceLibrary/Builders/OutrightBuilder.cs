namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Common.Converters;
    using Common.DTO;
    using Common.Helpers;
    using Filters;
    using Models;

    public class OutrightBuilder : IOutrightBuilder
    {
        private readonly IMatchBuilder matchBuilder;
        private readonly OutrightFilter outrightFilter;

        public OutrightBuilder()
        {
            matchBuilder = new MatchBuilder();
            outrightFilter = new OutrightFilter();
        }

        public void AppendOutrightsNode(
            StringBuilder oddsFeedNode,
            IList<Outright> outrights,
            OddsFeedsModel model)
        {
            var userLanguage = matchBuilder.GetUserLanguage(model.UserProfile);
            oddsFeedNode.Append("<outrights>");

            if (IsSampleOutright(model.IsOutright, model.IsSample))
            {
                var firstOutright = outrights.FirstOrDefault();

                if (firstOutright != null)
                {
                    AppendEventNode(oddsFeedNode, model.UserProfile, userLanguage, firstOutright);
                }
            }

            if (FilterBetType(model.UserQuery, model.IsSample))
            {
                foreach (var outright in outrights)
                {
                    if (outrightFilter.FilterOutright(outright, model.UserProfile, model.UserQuery))
                    {
                        AppendEventNode(oddsFeedNode, model.UserProfile, userLanguage, outright);
                    }
                }
            }

            oddsFeedNode.Append("</outrights>");
        }

        private static void AppendEventNode(StringBuilder oddsFeedNode, UserProfile userProfile, UserLanguage userLanguage, Outright outright)
        {
            oddsFeedNode.Append($"<event id=\"{outright.EventId}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedEnglish, $"ge=\"{SecurityElement.Escape(outright.EventEn)}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedTraditionalChinese, $"gt=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventTw, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedSimplifiedChinese, $"gs=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventCn, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedIndonesia, $"gb=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventIndonesia, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedThai, $"ga=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventThai, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedKorean, $"gk=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventKorean, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedVietnamese, $"gv=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventVietnamese, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedJapanese, $"gj=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventJapanese, outright.EventEn))}\" ");

            AppendEventNodeSingleCondition(oddsFeedNode, userLanguage.IsSupportedHindi, $"gh=\"{SecurityElement.Escape(StringHelper.SelectName(outright.EventHindi, outright.EventEn))}\" ");

            oddsFeedNode.Append($"mt=\"{outright.Time}\" sport=\"{SecurityElement.Escape(outright.SportName)}\">");

            foreach (OutrightOdds outrightOddsNode in outright.OutrightOddsList)
            {
                AppendOdd(oddsFeedNode, userProfile, userLanguage, outrightOddsNode);
            }

            oddsFeedNode.Append("</event>");
        }

        private static void AppendEventNodeSingleCondition(StringBuilder builder, bool condition, string value)
        {
            if (condition)
            {
                builder.Append(value);
            }
        }

        private static void AppendOdd(
            StringBuilder oddsFeedNode,
            UserProfile userProfile,
            UserLanguage userLanguage,
            OutrightOdds outrightOddsNode)
        {
            oddsFeedNode.AppendFormat($"<odd id=\"{outrightOddsNode.OddsId}\" ");

            if (userLanguage.IsSupportedEnglish)
            {
                oddsFeedNode.Append($"te=\"{SecurityElement.Escape(outrightOddsNode.OddNameEn)}\" ");
            }

            if (userLanguage.IsSupportedTraditionalChinese)
            {
                oddsFeedNode.Append($"tt=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameTw, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"ts=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameCn, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"tb=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameIndonesia, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ta=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameThai, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"tk=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameKorean, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"tv=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameVietnamese, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"tj=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameJapanese, outrightOddsNode.OddNameEn))}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"th=\"{SecurityElement.Escape(StringHelper.SelectName(outrightOddsNode.OddNameHindi, outrightOddsNode.OddNameEn))}\" ");
            }

            oddsFeedNode.AppendFormat($"o=\"{OddsConverter.ConvertOdds(userProfile.OddRegion, outrightOddsNode.Value)}\" />");
        }

        private static bool IsSampleOutright(bool isOutright, bool isSample)
        {
            return isSample && isOutright;
        }

        public static bool FilterBetType(UserQuery userQuery, bool isSample)
        {
            const int defaultBetType = 10;
            var notNullCondition = userQuery.BetTypes != null && userQuery.BetTypes.Contains(defaultBetType);
            return !isSample
                && (userQuery.BetTypes == null
                || userQuery.BetTypes.Count == 0
                || notNullCondition);
        }
    }
}
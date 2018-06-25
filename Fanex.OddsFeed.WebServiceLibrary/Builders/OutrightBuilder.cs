namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using Common.Converters;
    using Common.DTO;
    using Common.Helpers;
    using Filters;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;

    public class OutrightBuilder : IOutrightBuilder
    {
        private OutrightFilter _outrightFilter;

        public OutrightBuilder()
        {
            _outrightFilter = new OutrightFilter();
        }

        public void AppendOutrightsNode(
            StringBuilder oddsFeedNode,
            IList<Outright> outrights,
            OddsFeedsModel model,
            UserLanguage userLanguage)
        {
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
                    if (_outrightFilter.FilterOutright(outright, model.UserProfile, model.UserQuery))
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

            foreach (OutrightOdds outrightOddsNode in outright.OutrightOddsList)
            {
                AppendOdd(oddsFeedNode, userProfile, userLanguage, outrightOddsNode);
            }

            oddsFeedNode.Append("</event>");
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
            return !isSample
                && (userQuery.BetTypes == null
                || userQuery.BetTypes.Count == 0
                || (userQuery.BetTypes != null
                && userQuery.BetTypes.Contains(10)));
        }
    }
}
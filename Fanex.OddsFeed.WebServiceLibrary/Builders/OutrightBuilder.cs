namespace Fanex.OddsFeed.WebServiceLibrary.Builders
{
    using Common.Converters;
    using Common.DTO;
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
                oddsFeedNode.Append($"gt=\"{SecurityElement.Escape(outright.EventTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"gs=\"{SecurityElement.Escape(outright.EventCn)}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"gb=\"{SecurityElement.Escape(outright.EventIndonesia)}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ga=\"{SecurityElement.Escape(outright.EventThai)}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"gk=\"{SecurityElement.Escape(outright.EventKorean)}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"gv=\"{SecurityElement.Escape(outright.EventVietnamese)}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"gj=\"{SecurityElement.Escape(outright.EventJapanese)}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"gh=\"{SecurityElement.Escape(outright.EventHindi)}\" ");
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
                oddsFeedNode.Append($"tt=\"{SecurityElement.Escape(outrightOddsNode.OddNameTw)}\" ");
            }

            if (userLanguage.IsSupportedSimplifiedChinese)
            {
                oddsFeedNode.Append($"ts=\"{SecurityElement.Escape(outrightOddsNode.OddNameCn)}\" ");
            }

            if (userLanguage.IsSupportedIndonesia)
            {
                oddsFeedNode.Append($"tb=\"{SecurityElement.Escape(outrightOddsNode.OddNameIndonesia)}\" ");
            }

            if (userLanguage.IsSupportedThai)
            {
                oddsFeedNode.Append($"ta=\"{SecurityElement.Escape(outrightOddsNode.OddNameThai)}\" ");
            }

            if (userLanguage.IsSupportedKorean)
            {
                oddsFeedNode.Append($"tk=\"{SecurityElement.Escape(outrightOddsNode.OddNameKorean)}\" ");
            }

            if (userLanguage.IsSupportedVietnamese)
            {
                oddsFeedNode.Append($"tv=\"{SecurityElement.Escape(outrightOddsNode.OddNameVietnamese)}\" ");
            }

            if (userLanguage.IsSupportedJapanese)
            {
                oddsFeedNode.Append($"tj=\"{SecurityElement.Escape(outrightOddsNode.OddNameJapanese)}\" ");
            }

            if (userLanguage.IsSupportedHindi)
            {
                oddsFeedNode.Append($"th=\"{SecurityElement.Escape(outrightOddsNode.OddNameHindi)}\" ");
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
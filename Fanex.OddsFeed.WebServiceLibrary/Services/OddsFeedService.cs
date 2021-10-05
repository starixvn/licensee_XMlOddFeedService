namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Builders;
    using Caching;
    using Common.DTO;
    using Common.Helpers;
    using Fanex.OddsFeed.Common;
    using Filters;
    using Models;
    using NLog;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchBuilder _matchBuilder;
        private readonly IOutrightBuilder _outrightBuilder;
        private readonly IOddsFeedCachingOperator _oddsFeedCachingOperator;
        private readonly IMatchFilter _matchFilter;

        public OddsFeedService()
        {
            _matchBuilder = new MatchBuilder();
            _outrightBuilder = new OutrightBuilder();
            _oddsFeedCachingOperator = new OddsFeedCachingOperator();
            _matchFilter = new MatchFilter();
        }

        public string BuildXmlDocument(OddsFeedsModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) && model.Username.Equals("betradar", StringComparison.CurrentCultureIgnoreCase))
            {
                return BuildXmlDocumentForBetRadar(model);
            }

            var isShownOutright = IsShownOutright(model);
            var isShownLiveMatch = IsShownLiveMatch(model.UserProfile);
            var isShownDeadMatch = IsShownDeadBallMatch(model.UserProfile);

            var oddsData = _oddsFeedCachingOperator.GetOddsData(model, isShownOutright, isShownLiveMatch, isShownDeadMatch);
            var oddsFeedNode = new StringBuilder("<oddsfeed>");
            var userLanguage = GetUserLanguage(model);

            if (isShownOutright)
            {
                _outrightBuilder.AppendOutrightsNode(oddsFeedNode, oddsData.Outrights, model, userLanguage);
            }

            AppendMatches(oddsFeedNode, oddsData.LiveMatches, oddsData.DeadballMatches, model, userLanguage);

            oddsFeedNode.Append("</oddsfeed>");

            return oddsFeedNode.ToString();
        }

        private string BuildXmlDocumentForBetRadar(OddsFeedsModel model)
        {
            var matchList = new List<Match>();
            using (var oddsFeedData = DbAccess.GetBetRadar())
            {
                foreach (DataRow row in oddsFeedData.Rows)
                {
                    var match = CreateMatch(row);
                    if (match != null)
                    {
                        if (!matchList.Any(x => x.MatchId == match.MatchId))
                        {
                            match.MatchOddsList = new List<MatchOdds>();
                            matchList.Add(match);
                        }

                        Match tempMatch = matchList.First(x => x.MatchId == match.MatchId);
                        GetHandicapOdds(tempMatch, row);
                        GetOverUnderOdds(tempMatch, row);
                        GetThreewayOdds(tempMatch, row);
                        GetMatchWinnerOdds(tempMatch, row);
                    }
                }
            }

            var oddsFeedNode = new StringBuilder("<oddsfeed>");
            model.UserProfile = new UserProfile();
            var userLanguage = new UserLanguage
            {
                IsSupportedEnglish = true
            };
            AppendMatches(oddsFeedNode, new List<Match>(), matchList, model, userLanguage);
            oddsFeedNode.Append("</oddsfeed>");
            return oddsFeedNode.ToString();
        }

        private static Match CreateMatch(DataRow row)
        {
            return new Match
            {
                MatchId = Convert.ToInt32(row["MatchID"]),
                SportName = row["SportTypeName"].ToString(),
                SportId = Convert.ToInt32(row["SportType"]),
                KickOfTime = Convert.ToDateTime(row["KickOffTime"].ToString()),
                HomeEn = row["Home"].ToString(),
                AwayEn = row["Away"].ToString(),
                LeagueId = Convert.ToInt32(row["LeagueID"]),
                LeagueEn = row["LeagueName"].ToString()
            };
        }

        private static void GetHandicapOdds(Match match, DataRow row)
        {
            if (!row.IsNull("OddsID_1"))
            {
                match.MatchOddsList.Add(new MatchOdds
                {
                    MatchId = Convert.ToInt32(row["MatchID"]),
                    Tag = "ah",
                    Time = Convert.ToDateTime(row["ChangeTime_1"]),
                    TimeStamp = Convert.ToInt32(row["TimeStamp_1"]),
                    BetTypeId = 1,
                    BetChoices = new List<BetChoice>
                    {
                        new BetChoice
                        {
                            Code = "h",
                            Value = ConvertOddsToString(GetOddsValue(row["hdp2_1"]) - GetOddsValue(row["hdp1_1"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "o",
                            Value = ConvertOddsToString(GetOddsValue(row["odds1hka_1"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "u",
                            Value = ConvertOddsToString(GetOddsValue(row["odds2hka_1"])),
                            NotConvert = true
                        }
                    }
                });
            }
        }

        private static void GetOverUnderOdds(Match match, DataRow row)
        {
            if (!row.IsNull("OddsID_3"))
            {
                const byte OverUnderBetTypeId = 3;
                match.MatchOddsList.Add(new MatchOdds
                {
                    MatchId = Convert.ToInt32(row["MatchID"]),
                    Tag = "ou",
                    Time = Convert.ToDateTime(row["ChangeTime_3"]),
                    TimeStamp = Convert.ToInt32(row["TimeStamp_3"]),
                    BetTypeId = OverUnderBetTypeId,
                    BetChoices = new List<BetChoice>
                    {
                        new BetChoice
                        {
                            Code = "h",
                            Value = ConvertOddsToString(GetOddsValue(row["Hdp1_3"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "o",
                            Value = ConvertOddsToString(GetOddsValue(row["odds1hka_3"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "u",
                            Value = ConvertOddsToString(GetOddsValue(row["odds2hka_3"])),
                            NotConvert = true
                        }
                    }
                });
            }
        }

        private static void GetThreewayOdds(Match match, DataRow row)
        {
            if (!row.IsNull("OddsID_5"))
            {
                const byte ThreewayBetTypeId = 5;
                match.MatchOddsList.Add(new MatchOdds
                {
                    MatchId = Convert.ToInt32(row["MatchID"]),
                    Tag = "st",
                    Time = Convert.ToDateTime(row["ChangeTime_5"]),
                    TimeStamp = Convert.ToInt32(row["TimeStamp_5"]),
                    BetTypeId = ThreewayBetTypeId,
                    BetChoices = new List<BetChoice>
                    {
                        new BetChoice
                        {
                            Code = "o1",
                            Value = ConvertOddsToString(GetOddsValue(row["com1_5"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "o2",
                            Value = ConvertOddsToString(GetOddsValue(row["comx_5"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "o3",
                            Value = ConvertOddsToString(GetOddsValue(row["com2_5"])),
                            NotConvert = true
                        }
                    }
                });
            }
        }

        private static void GetMatchWinnerOdds(Match match, DataRow row)
        {
            if (!row.IsNull("OddsID_501"))
            {
                const short MatchWinnerBetTypeId = 501;
                match.MatchOddsList.Add(new MatchOdds
                {
                    MatchId = Convert.ToInt32(row["MatchID"]),
                    Tag = "ck_mw",
                    Time = Convert.ToDateTime(row["ChangeTime_501"]),
                    TimeStamp = Convert.ToInt32(row["TimeStamp_501"]),
                    BetTypeId = MatchWinnerBetTypeId,
                    BetChoices = new List<BetChoice>
                    {
                        new BetChoice
                        {
                            Code = "h",
                            Value = ConvertOddsToString(GetOddsValue(row["odds1hka_501"])),
                            NotConvert = true
                        },
                        new BetChoice
                        {
                            Code = "a",
                            Value = ConvertOddsToString(GetOddsValue(row["odds2hka_501"])),
                            NotConvert = true
                        }
                    }
                });
            }
        }

        private static decimal GetOddsValue(object value)
        {
            object oddValue;
            oddValue = value is DBNull ? 0 : value;

            return Convert.ToDecimal(oddValue);
        }

        private static string ConvertOddsToString(decimal odds)
        {
            return string.Format("{0:0.#########}", odds);
        }

        private static UserLanguage GetUserLanguage(OddsFeedsModel model)
        {
            return new UserLanguage
            {
                IsSupportedEnglish = StringHelper.ContainInList(model.UserProfile.Languages, "en-US"),
                IsSupportedTraditionalChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-TW"),
                IsSupportedSimplifiedChinese = StringHelper.ContainInList(model.UserProfile.Languages, "zh-CN"),
                IsSupportedIndonesia = StringHelper.ContainInList(model.UserProfile.Languages, "id-ID"),
                IsSupportedThai = StringHelper.ContainInList(model.UserProfile.Languages, "th-TH"),
                IsSupportedKorean = StringHelper.ContainInList(model.UserProfile.Languages, "ko-KR"),
                IsSupportedVietnamese = StringHelper.ContainInList(model.UserProfile.Languages, "vi-VN"),
                IsSupportedJapanese = StringHelper.ContainInList(model.UserProfile.Languages, "ja-JP"),
                IsSupportedHindi = StringHelper.ContainInList(model.UserProfile.Languages, "hi-IN")
            };
        }

        private void AppendMatches(
            StringBuilder oddsFeedNode,
            IList<Match> liveMatches,
            IList<Match> deadMatches,
            OddsFeedsModel model,
            UserLanguage userLanguage)
        {
            AppendBeginMatches(oddsFeedNode, model, GetMatchTimeStamp(model, liveMatches, deadMatches), GetLastUpdate());

            _matchBuilder.AppendMatchNodes(oddsFeedNode, liveMatches, userLanguage, model);
            _matchBuilder.AppendMatchNodes(oddsFeedNode, deadMatches, userLanguage, model);

            oddsFeedNode.Append("</matches>");
        }

        private int GetMatchTimeStamp(OddsFeedsModel model, IList<Match> liveMatches, IList<Match> deadMatches)
        {
            var liveMatchesTimeStamp = 0;
            var deadMatchesTimeStamp = 0;

            if (liveMatches.Any())
            {
                liveMatchesTimeStamp = GetMaxTimeStampMatches(model, liveMatches);
            }

            if (deadMatches.Any())
            {
                deadMatchesTimeStamp = GetMaxTimeStampMatches(model, deadMatches);
            }

            return Math.Max(liveMatchesTimeStamp, deadMatchesTimeStamp);
        }

        private int GetMaxTimeStampMatches(OddsFeedsModel model, IList<Match> matches)
        {
            var matchFilter = matches.Where(m => _matchFilter.FilterMatch(m, model));

            return matchFilter.Any() ? matchFilter.Max(m => GetMaxTimeStampOdds(model, m.MatchOddsList)) : 0;
        }

        private int GetMaxTimeStampOdds(OddsFeedsModel model, IList<MatchOdds> matchOddsList)
        {
            var matchOddsFilter = matchOddsList.Where(o => _matchFilter.FilterBetType(o, model));

            return matchOddsFilter.Any() ? matchOddsFilter.Max(o => o.TimeStamp) : 0;
        }

        private static bool IsShownOutright(OddsFeedsModel model)
        {
            return model.UserProfile.OddsType.Contains(OddsTypeOutRight) && (!model.IsSample || model.IsOutright);
        }

        private static bool IsShownLiveMatch(UserProfile userProfile)
        {
            return userProfile.OddsType != null && userProfile.OddsType.Contains(OddsTypeLiveMatch);
        }

        private static bool IsShownDeadBallMatch(UserProfile userProfile)
        {
            return userProfile.OddsType != null && userProfile.OddsType.Contains(OddsTypeDeadBallMatch);
        }

        private static DateTime GetLastUpdate()
        {
            const int addHour = 12;
            return DateTime.Now.AddHours(addHour);
        }

        private static void AppendBeginMatches(
            StringBuilder oddsFeedNode,
            OddsFeedsModel model,
            int matchesTimeStamp,
            DateTime lastUpdate)
        {
            if (matchesTimeStamp > 0)
            {
                var oddsType = GetOddsTypeName(model.UserProfile.OddRegion);
                oddsFeedNode.AppendFormat("<matches lastupdate=\"{0}\" timestamp=\"{1}\"{2}>",
                    lastUpdate,
                    matchesTimeStamp,
                    !string.IsNullOrWhiteSpace(oddsType) ? $" oddstype=\"{GetOddsTypeName(model.UserProfile.OddRegion)}\"" : string.Empty);
            }
            else
            {
                oddsFeedNode.Append("<matches>");
            }
        }

        private static string GetOddsTypeName(int oddsTypeId)
        {
            const byte MalayOddsType = 1;
            const byte HongKongOddsType = 2;
            const byte DecimalOddsType = 3;
            const byte IndoOddsType = 4;
            const byte USOddsType = 5;
            string oddsTypeName;

            switch (oddsTypeId)
            {
                case MalayOddsType:
                    oddsTypeName = "Malay";
                    break;

                case HongKongOddsType:
                    oddsTypeName = "HongKong";
                    break;

                case DecimalOddsType:
                    oddsTypeName = "Decimal";
                    break;

                case IndoOddsType:
                    oddsTypeName = "Indo";
                    break;

                case USOddsType:
                    oddsTypeName = "US";
                    break;

                default:
                    oddsTypeName = string.Empty;
                    break;
            }

            return oddsTypeName;
        }
    }
}
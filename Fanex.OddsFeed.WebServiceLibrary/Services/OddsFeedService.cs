namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Builders;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using Fanex.OddsFeed.WebServiceLibrary.Filters;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public class OddsFeedService : IOddsFeedService
    {
        private const int OddsTypeOutRight = 8;
        private const int OddsTypeLiveMatch = 4;
        private const int OddsTypeDeadBallMatch = 2;

        private readonly IMatchBuilder matchBuilder;
        private readonly IOutrightBuilder outrightBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IMatchFilter matchFilter;

        public OddsFeedService()
        {
            matchBuilder = new MatchBuilder();
            outrightBuilder = new OutrightBuilder();
            feedCachingOperator = new FeedCachingOperator();
            matchFilter = new MatchFilter();
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

            var oddsData = feedCachingOperator.GetOddsData(model, isShownOutright, isShownLiveMatch, isShownDeadMatch);
            var oddsFeedNode = new StringBuilder("<oddsfeed>");

            if (isShownOutright)
            {
                outrightBuilder.AppendOutrightsNode(oddsFeedNode, oddsData.Outrights, model);
            }

            AppendMatches(oddsFeedNode, oddsData.LiveMatches, oddsData.DeadballMatches, model);

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
            model.UserProfile = new UserProfile
            {
                Languages = new List<string> { "en-US" }
            };
            AppendMatches(oddsFeedNode, new List<Match>(), matchList, model);
            oddsFeedNode.Append("</oddsfeed>");
            return oddsFeedNode.ToString();
        }

        private static Match CreateMatch(DataRow row)
        {
            return new Match
            {
                MatchId = GetMatchId(row),
                SportName = row["SportTypeName"].ToString(),
                SportId = Convert.ToInt32(row["SportType"]),
                KickOffTime = Convert.ToDateTime(row["KickOffTime"].ToString()),
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
                    MatchId = GetMatchId(row),
                    Tag = "ah",
                    Time = Convert.ToDateTime(row["ChangeTime_1"]),
                    Timestamp = Convert.ToInt32(row["Timestamp_1"]),
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
                    MatchId = GetMatchId(row),
                    Tag = "ou",
                    Time = Convert.ToDateTime(row["ChangeTime_3"]),
                    Timestamp = Convert.ToInt32(row["Timestamp_3"]),
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
                    MatchId = GetMatchId(row),
                    Tag = "st",
                    Time = Convert.ToDateTime(row["ChangeTime_5"]),
                    Timestamp = Convert.ToInt32(row["Timestamp_5"]),
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
                    MatchId = GetMatchId(row),
                    Tag = "ck_mw",
                    Time = Convert.ToDateTime(row["ChangeTime_501"]),
                    Timestamp = Convert.ToInt32(row["Timestamp_501"]),
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

        private static int GetMatchId(DataRow row)
        {
            return Convert.ToInt32(row["MatchID"]);
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

        private void AppendMatches(
            StringBuilder oddsFeedNode,
            IList<Match> liveMatches,
            IList<Match> deadMatches,
            OddsFeedsModel model)
        {
            AppendBeginMatches(oddsFeedNode, model, GetMatchTimestamp(model, liveMatches, deadMatches), GetLastUpdate());

            matchBuilder.AppendMatchNodes(oddsFeedNode, liveMatches, model);
            matchBuilder.AppendMatchNodes(oddsFeedNode, deadMatches, model);

            oddsFeedNode.Append("</matches>");
        }

        private int GetMatchTimestamp(OddsFeedsModel model, IList<Match> liveMatches, IList<Match> deadMatches)
        {
            var liveMatchesTimestamp = 0;
            var deadMatchesTimestamp = 0;

            if (liveMatches.Any())
            {
                liveMatchesTimestamp = GetMaxTimestampMatches(model, liveMatches);
            }

            if (deadMatches.Any())
            {
                deadMatchesTimestamp = GetMaxTimestampMatches(model, deadMatches);
            }

            return Math.Max(liveMatchesTimestamp, deadMatchesTimestamp);
        }

        private int GetMaxTimestampMatches(OddsFeedsModel model, IList<Match> matches)
        {
            var tempMatches = matches.Where(m => matchFilter.FilterMatch(m, model));

            return tempMatches.Any() ? tempMatches.Max(m => GetMaxTimestampOdds(model, m.MatchOddsList)) : 0;
        }

        private int GetMaxTimestampOdds(OddsFeedsModel model, IList<MatchOdds> matchOddsList)
        {
            var matchOddsFilter = matchOddsList.Where(o => matchFilter.FilterBetType(o, model));

            return matchOddsFilter.Any() ? matchOddsFilter.Max(o => o.Timestamp) : 0;
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
            int matchesTimestamp,
            DateTime lastUpdate)
        {
            if (matchesTimestamp > 0)
            {
                var oddsType = GetOddsTypeName(model.UserProfile.OddRegion);
                oddsFeedNode.AppendFormat("<matches lastupdate=\"{0}\" timestamp=\"{1}\"{2}>",
                    lastUpdate,
                    matchesTimestamp,
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
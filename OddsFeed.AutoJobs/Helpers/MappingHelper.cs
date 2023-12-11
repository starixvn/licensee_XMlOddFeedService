namespace OddsFeed.AutoJobs.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Oddsfeed;
    using OddsFeed.Models.Outright;
    using OddsFeed.Core.Extensions;

    public static class MappingHelper
    {
        public static IList<Match> MappingMatches(
            DataTable dataTable,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive)
        {
            var matchList = new List<Match>();

            var tempMatch = new Match();
            foreach (DataRow row in dataTable.Rows)
            {
                var match = CreateMatch(row, isLive);
                if (match != null)
                {
                    if (tempMatch.MatchId != match.MatchId)
                    {
                        tempMatch = match;
                        match.MatchOddsList = new List<MatchOdds>();
                        matchList.Add(match);
                    }

                    var matchOdds = GetMatchOdds(schemas, row, oddsFlexs);
                    if (matchOdds != null)
                    {
                        tempMatch.MatchOddsList.Add(matchOdds);
                    }
                }
            }

            return matchList;
        }

        #region Mapping Matches Private

        private static Match CreateMatch(DataRow row, bool isLive)
            => new Match
            {
                MatchId = Convert.ToInt32(row["MatchID"]),
                SportName = row["SportTypeName"].ToString(),
                SportId = Convert.ToInt32(row["SportType"]),
                KickOffTime = row["KickOffTime"] != DBNull.Value ? Convert.ToDateTime(row["KickOffTime"].ToString()) : null,
                GlobalShowtime = row["GlobalShowtime"] != DBNull.Value ? Convert.ToDateTime(row["GlobalShowtime"].ToString()) : null,
                Showtime = GetShowtime(row["GlobalShowtime"].ToString(), row["Showtime"].ToString()),
                HomeEn = row["Home"].ToString(),
                HomeTw = row["HomeC_UNI"].ToString(),
                HomeCn = row["HomeCN"].ToString(),
                HomeIndonesia = row["HomeId"].ToString(),
                HomeThai = row["HomeTh"].ToString(),
                HomeKorean = row["HomeKo"].ToString(),
                HomeVietnamese = row["HomeVn"].ToString(),
                HomeJapanese = row["HomeJp"].ToString(),
                HomeHindi = row["HomeHi"].ToString(),
                AwayEn = row["Away"].ToString(),
                AwayTw = row["AwayC_UNI"].ToString(),
                AwayCn = row["AwayCN"].ToString(),
                AwayIndonesia = row["AwayId"].ToString(),
                AwayThai = row["AwayTh"].ToString(),
                AwayKorean = row["AwayKo"].ToString(),
                AwayVietnamese = row["AwayVn"].ToString(),
                AwayJapanese = row["AwayJp"].ToString(),
                AwayHindi = row["AwayHi"].ToString(),
                LeagueId = Convert.ToInt32(row["LeagueID"]),
                LeagueEn = row["LeagueName"].ToString(),
                LeagueTw = row["LeagueNameC_UNI"].ToString(),
                LeagueCn = row["LeagueNameCN"].ToString(),
                LeagueIndonesia = row["leaguenameId"].ToString(),
                LeagueThai = row["leaguenameTh"].ToString(),
                LeagueKorean = row["leaguenameKo"].ToString(),
                LeagueVietnamese = row["leaguenameVn"].ToString(),
                LeagueJapanese = row["leaguenameJp"].ToString(),
                LeagueHindi = row["leaguenameHi"].ToString(),
                CountryCode = row["CountryCode"].ToString(),
                IsLive = isLive
            };

        private static DateTime? GetShowtime(string globalShowtime, string showtime)
        {
            if (string.IsNullOrEmpty(globalShowtime))
            {
                return null;
            }

            if (string.IsNullOrEmpty(showtime))
            {
                return DateTime.MinValue;
            }

            DateTime gsTime = Convert.ToDateTime(globalShowtime);
            DateTime sTime;
            var canParseShowtime = DateTime.TryParseExact(
                showtime,
                "MM/dd hh:mmtt",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out sTime);

            return canParseShowtime
                ? new DateTime(gsTime.Year, sTime.Month, sTime.Day, sTime.Hour, sTime.Minute, sTime.Second)
                : gsTime;
        }

        private static MatchOdds GetMatchOdds(IEnumerable<BetTypeSchema> schemas, DataRow row, IEnumerable<OddsFlexGroup> oddsFlexs)
        {
            var schema = FindBetTypeSchema(Convert.ToInt32(row["BetType"]), schemas);

            if (schema == null)
            {
                return null;
            }

            return new MatchOdds
            {
                MatchId = Convert.ToInt32(row["MatchID"]),
                Tag = schema.Tag,
                Time = row["ChangeTime"] != DBNull.Value ? Convert.ToDateTime(row["ChangeTime"]) : null,
                Timestamp = Convert.ToInt32(row["Timestamp"]),
                BetTypeId = Convert.ToInt32(row["BetType"]),
                BetChoices = schema.IsOddsFlex
                                          ? GetOddsFlexBetChoices(schema, row, oddsFlexs)
                                          : GetNormalBetChoices(schema, row)
            };
        }

        private static IList<BetChoice> GetNormalBetChoices(BetTypeSchema schema, DataRow row)
            => schema.Reserve == null ? MapBetChoices(schema.BetChoices, row) : MapBetChoicesReserve(schema, row);

        private static IList<BetChoice> GetOddsFlexBetChoices(BetTypeSchema schema, DataRow row, IEnumerable<OddsFlexGroup> oddFlexGroups)
        {
            var betChoices = new List<BetChoice>();
            var oddsId = Convert.ToInt32(row["oddsid"]);
            var oddsFlexGroup = oddFlexGroups.FirstOrDefault(odds => odds.OddsId == oddsId);

            if (oddsFlexGroup == null)
            {
                return betChoices;
            }

            foreach (var betChoiceSchema in schema.BetChoices)
            {
                var oddsFlex = oddsFlexGroup.OddsFlexs.FirstOrDefault(odds => odds.BetChoice.Equals(betChoiceSchema.DataAttribute));

                if (oddsFlex != null)
                {
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = ConvertMalayToDecimalOddsString(oddsFlex.Odds)
                    });
                }
            }

            return betChoices;
        }

        private static IList<BetChoice> MapBetChoices(IEnumerable<BetChoiceSchema> betChoiceSchemas, DataRow row)
        {
            var betChoices = new List<BetChoice>();

            foreach (var betChoiceSchema in betChoiceSchemas)
            {
                var dataAttribute = GetDataAttribute(betChoiceSchema);

                if (betChoiceSchema.IsResource)
                {
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = Convert.ToString(row[dataAttribute]),
                        NotConvert = true
                    });
                }
                else if (betChoiceSchema.IsResourceX || betChoiceSchema.IsResourceY)
                {
                    const int maxResouceIdAmount = 4;
                    const int subStringLength = 2;
                    var resourceId = Convert.ToString(row["resourceID"]);
                    var index = betChoiceSchema.IsResourceX ? 0 : 2;
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = (resourceId.Count() >= maxResouceIdAmount) ? resourceId.Substring(index, subStringLength) : string.Empty,
                        NotConvert = true
                    });
                }
                else
                {
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = GetOddsValue(betChoiceSchema, row[dataAttribute]),
                        NotConvert = betChoiceSchema.NotConvert
                    });
                }
            }

            return betChoices;
        }

        private static IList<BetChoice> MapBetChoicesReserve(BetTypeSchema betTypeSchema, DataRow row)
        {
            var betChoices = new List<BetChoice>();
            var betChoice1 = betTypeSchema.BetChoices.ElementAt(1);
            var betChoice2 = betTypeSchema.BetChoices.ElementAt(2);

            var hdp1 = Convert.ToDecimal(row[betTypeSchema.Reserve.FirstHdp]);
            var hdp2 = Convert.ToDecimal(row[betTypeSchema.Reserve.SecondHdp]);

            betChoices.Add(new BetChoice
            {
                Code = betTypeSchema.BetChoices.ElementAt(0).MatchAttribute,
                Value = ConvertOddsToString(hdp2 - hdp1),
                NotConvert = true
            });

            betChoices.Add(new BetChoice
            {
                Code = betChoice1.MatchAttribute,
                Value = ConvertMalayToDecimalOddsString(Convert.ToDecimal(row[GetDataAttribute(betChoice1)]))
            });

            betChoices.Add(new BetChoice
            {
                Code = betChoice2.MatchAttribute,
                Value = ConvertMalayToDecimalOddsString(Convert.ToDecimal(row[GetDataAttribute(betChoice2)]))
            });

            AssignResourceOdds(betChoices, betTypeSchema, row);

            return betChoices;
        }

        private static void AssignResourceOdds(IList<BetChoice> betChoices, BetTypeSchema betTypeSchema, DataRow row)
        {
            var resourceBetChoice = GetResourceBetChoice(betTypeSchema.BetChoices);

            if (resourceBetChoice != null)
            {
                betChoices.Add(new BetChoice
                {
                    Code = resourceBetChoice.MatchAttribute,
                    Value = Convert.ToString(row["resourceID"]),
                    NotConvert = true
                });
            }
        }

        private static string GetDataAttribute(BetChoiceSchema betChoiceSchema)
            => string.IsNullOrEmpty(betChoiceSchema.DataAttribute)
                            ? betChoiceSchema.MatchAttribute
                            : betChoiceSchema.DataAttribute;

        private static string GetOddsValue(BetChoiceSchema betChoiceSchema, object value)
        {
            object oddValue;
            oddValue = value is DBNull ? 0 : value;

            if (betChoiceSchema.NotConvert)
            {
                return ConvertOddsToString(Convert.ToDecimal(oddValue));
            }

            return ConvertMalayToDecimalOddsString(Convert.ToDecimal(oddValue));
        }

        private static BetTypeSchema FindBetTypeSchema(int betTypeId, IEnumerable<BetTypeSchema> schemas)
            => schemas.FirstOrDefault(schema => schema.BetTypeId == betTypeId);

        private static BetChoiceSchema GetResourceBetChoice(IEnumerable<BetChoiceSchema> betChoiceSchemas)
            => betChoiceSchemas.FirstOrDefault(i => i.IsResource);

        private static string ConvertOddsToString(decimal odds)
            => string.Format("{0:0.#########}", odds);

        private static string ConvertMalayToDecimalOddsString(decimal odds)
            => string.Format("{0:0.#########}", OddsConverter.ToDecimalOddsFromMalayOdds(Convert.ToDecimal(odds)));

        #endregion Mapping Matches Private

        public static List<Outright> MappingOutrights(DataTable? dataTable)
        {
            if (dataTable == null)
            {
                return new List<Outright>();
            }

            Partitioner<DataRow> partionerList = Partitioner.Create(dataTable.AsEnumerable().ToList(), true);

            var outrights = from row in partionerList.AsParallel()
                            group row by row["LeagueID"].ToString() into g
                            select new Outright
                            {
                                EventId = Convert.ToInt32(g.Key),
                                EventEn = GetEventEn(g),
                                EventTw = GetEventTw(g),
                                EventCn = GetEventCn(g),
                                EventIndonesia = GetEventIndonesia(g),
                                EventThai = GetEventThai(g),
                                EventKorean = GetEventKorean(g),
                                EventVietnamese = GetEventVietnamese(g),
                                EventJapanese = GetEventJapanese(g),
                                EventHindi = GetEventHindi(g),
                                SportId = Convert.ToInt32(g.FirstOrDefault()?["SportID"]),
                                SportName = g.FirstOrDefault()?["LeagueNameTC"].ToString(),
                                CountryCode = g.FirstOrDefault()?["CountryCode"].ToString(),
                                Time = Convert.ToDateTime(g.FirstOrDefault()?["EventDate"].ToString()),
                                OutrightOddsList = new List<OutrightOdds>(g.Select(i => GetEventOdds(i)))
                            };

            return outrights.ToList();
        }

        #region Mapping Outrights Private

        private static string GetEventEn(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueName"].ToString();
        }

        private static string GetEventTw(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueNameC_UNI"].ToString();
        }

        private static string GetEventCn(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueNameCN"].ToString();
        }

        private static string GetEventIndonesia(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameId"].ToString();
        }

        private static string GetEventThai(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameTh"].ToString();
        }

        private static string GetEventKorean(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameKo"].ToString();
        }

        private static string GetEventVietnamese(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameVn"].ToString();
        }

        private static string GetEventJapanese(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameJp"].ToString();
        }

        private static string GetEventHindi(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameHi"].ToString();
        }

        private static OutrightOdds GetEventOdds(DataRow row)
        {
            return new OutrightOdds
            {
                OddsId = Convert.ToInt32(row["OrID"]),
                OddNameEn = row["TeamName"].ToString(),
                OddNameTw = row["TeamNameC_UNI"].ToString(),
                OddNameCn = row["TeamNameCN"].ToString(),
                OddNameIndonesia = row["teamnameId"].ToString(),
                OddNameThai = row["teamnameTh"].ToString(),
                OddNameKorean = row["teamnameKo"].ToString(),
                OddNameVietnamese = row["teamnameVn"].ToString(),
                OddNameJapanese = row["teamnameJp"].ToString(),
                OddNameHindi = row["teamnameHi"].ToString(),
                Value = Convert.ToDecimal(row["Odds"])
            };
        }

        #endregion Mapping Outrights Private
    }
}
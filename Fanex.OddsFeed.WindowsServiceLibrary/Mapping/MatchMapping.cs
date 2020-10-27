namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using Common.Converters;
    using Common.DTO;
    using Common.Schemas;

    public class MatchMapping : IMatchMapping
    {
        public IList<Match> GetMatches(
            DataTable dataTable,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive)
        {
            var matchList = new SafeList<Match>();
            var matchOddsList = new SafeList<MatchOdds>();

            var partionerList = Partitioner.Create(dataTable.AsEnumerable().ToList(), true);

            var tempRows = from row in partionerList.AsParallel() select row;

            tempRows.ForAll((row) =>
            {
                var matchOdds = GetMatchOdds(schemas, row, oddsFlexs);
                var match = CreateMatch(row, isLive);
                if (matchOdds != null)
                {
                    matchOddsList.Add(matchOdds);
                }

                if (match != null)
                {
                    matchList.Add(match);
                }
            });

            var matchListResult = new SafeList<Match>();
            var partitionerMatchList = Partitioner.Create(matchList, true);
            var matchListById = from row in partitionerMatchList.AsParallel()
                                group row by row.MatchId into g
                                select new { MatchId = g.Key, FirstMatch = g.FirstOrDefault() };

            matchListById.ForAll((row) =>
            {
                var temp = row.FirstMatch;
                temp.MatchOddsList = matchOddsList.AsParallel().Where(i => i.MatchId == row.MatchId).ToList();
                matchListResult.Add(temp);
            });

            return matchListResult;
        }

        private static Match CreateMatch(DataRow row, bool isLive)
        {
            return new Match
            {
                MatchId = Convert.ToInt32(row["MatchID"]),
                SportName = row["SportTypeName"].ToString(),
                SportId = Convert.ToInt32(row["SportType"]),
                KickOfTime = Convert.ToDateTime(row["KickOffTime"].ToString()),
                GlobalShowtime = Convert.ToDateTime(row["GlobalShowtime"].ToString()),
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
        }

        private static DateTime GetShowtime(string globalShowtime, string showtime)
        {
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
                Time = Convert.ToDateTime(row["ChangeTime"]),
                TimeStamp = Convert.ToInt32(row["TimeStamp"]),
                BetTypeId = Convert.ToInt32(row["BetType"]),
                BetChoices = schema.IsOddsFlex
                                          ? GetOddsFlexBetChoices(schema, row, oddsFlexs)
                                          : GetNormalBetChoices(schema, row)
            };
        }

        private static IList<BetChoice> GetNormalBetChoices(BetTypeSchema schema, DataRow row)
        {
            return schema.Reserve == null ? MapBetChoices(schema.BetChoices, row) : MapBetChoicesReserve(schema, row);
        }

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
        {
            return string.IsNullOrEmpty(betChoiceSchema.DataAttribute)
                            ? betChoiceSchema.MatchAttribute
                            : betChoiceSchema.DataAttribute;
        }

        private static string GetOddsValue(BetChoiceSchema betChoiceSchema, object value)
        {
            if (betChoiceSchema.NotConvert)
            {
                return ConvertOddsToString(Convert.ToDecimal(value));
            }

            return ConvertMalayToDecimalOddsString(Convert.ToDecimal(value));
        }

        private static BetTypeSchema FindBetTypeSchema(int betTypeId, IEnumerable<BetTypeSchema> schemas)
        {
            return schemas.FirstOrDefault(schema => schema.BetTypeId == betTypeId);
        }

        private static BetChoiceSchema GetResourceBetChoice(IEnumerable<BetChoiceSchema> betChoiceSchemas)
        {
            return betChoiceSchemas.FirstOrDefault(i => i.IsResource);
        }

        private static string ConvertOddsToString(decimal odds)
        {
            return string.Format("{0:0.#########}", odds);
        }

        private static string ConvertMalayToDecimalOddsString(decimal odds)
        {
            return string.Format("{0:0.#########}", OddsConverter.ToDecimalOddsFromMalayOdds(Convert.ToDecimal(odds)));
        }
    }
}
namespace OddsFeed.Core.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using OddsFeed.Core.Extensions;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Oddsfeed;

    public class MatchMapping : IMatchMapping
    {
        public IList<Match> GetMatches(
            IEnumerable<MatchOddsData> odds,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive)
        {
            var matchList = new List<Match>();

            var tempMatch = new Match();
            foreach (var row in odds)
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

        public Match CreateMatch(MatchOddsData row, bool isLive)
            => new Match
            {
                MatchId = row.MatchID,
                SportName = row.SportTypeName,
                SportId = row.SportType,
                KickOffTime = row.KickOffTime,
                GlobalShowtime = row.GlobalShowtime,
                Showtime = GetShowtime(row.GlobalShowtime, row.Showtime),
                HomeEn = row.Home,
                HomeTw = row.HomeC_UNI,
                HomeCn = row.HomeCN,
                HomeIndonesia = row.HomeId,
                HomeThai = row.HomeTh,
                HomeKorean = row.HomeKo,
                HomeVietnamese = row.HomeVn,
                HomeJapanese = row.HomeJp,
                HomeHindi = row.HomeHi,
                AwayEn = row.Away,
                AwayTw = row.AwayC_UNI,
                AwayCn = row.AwayCN,
                AwayIndonesia = row.AwayId,
                AwayThai = row.AwayTh,
                AwayKorean = row.AwayKo,
                AwayVietnamese = row.AwayVn,
                AwayJapanese = row.AwayJp,
                AwayHindi = row.AwayHi,
                LeagueId = row.LeagueID,
                LeagueEn = row.LeagueName,
                LeagueTw = row.LeagueNameC_UNI,
                LeagueCn = row.LeagueNameCN,
                LeagueIndonesia = row.LeagueNameId,
                LeagueThai = row.LeagueNameTh,
                LeagueKorean = row.LeagueNameKo,
                LeagueVietnamese = row.LeagueNameVn,
                LeagueJapanese = row.LeagueNameJp,
                LeagueHindi = row.LeagueNameHi,
                CountryCode = row.CountryCode,
                IsLive = isLive
            };

        private static DateTime GetShowtime(DateTime globalShowtime, string showtime)
        {
            var canParseShowtime = DateTime.TryParseExact(
                showtime,
                "MM/dd hh:mmtt",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime sTime);

            return canParseShowtime
                ? new DateTimeOffset(globalShowtime.Year, sTime.Month, sTime.Day, sTime.Hour, sTime.Minute, sTime.Second, TimeSpan.Zero).DateTime
                : globalShowtime;
        }

        private static MatchOdds GetMatchOdds(IEnumerable<BetTypeSchema> schemas, MatchOddsData row, IEnumerable<OddsFlexGroup> oddsFlexs)
        {
            var schema = FindBetTypeSchema(row.BetType, schemas);

            if (schema == null)
            {
                return null;
            }

            return new MatchOdds
            {
                MatchId = row.MatchID,
                Tag = schema.Tag,
                Time = row.ChangeTime,
                Timestamp = row.Timestamp,
                BetTypeId = row.BetType,
                BetChoices = schema.IsOddsFlex
                    ? GetOddsFlexBetChoices(schema, row, oddsFlexs)
                    : GetNormalBetChoices(schema, row)
            };
        }

        private static IList<BetChoice> GetNormalBetChoices(BetTypeSchema schema, MatchOddsData row)
            => schema.Reserve == null ? MapBetChoices(schema.BetChoices, row) : MapBetChoicesReserve(schema, row);

        private static IList<BetChoice> GetOddsFlexBetChoices(BetTypeSchema schema, MatchOddsData row, IEnumerable<OddsFlexGroup> oddFlexGroups)
        {
            var betChoices = new List<BetChoice>();
            var oddsFlexGroup = oddFlexGroups.FirstOrDefault(odds => odds.OddsId == row.OddsID);

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

        private static IList<BetChoice> MapBetChoices(IEnumerable<BetChoiceSchema> betChoiceSchemas, MatchOddsData row)
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
                        Value = Convert.ToString(GetPropValue(row, dataAttribute)),
                        NotConvert = true
                    });
                }
                else if (betChoiceSchema.IsResourceX || betChoiceSchema.IsResourceY)
                {
                    const int maxResouceIdAmount = 4;
                    const int subStringLength = 2;
                    var index = betChoiceSchema.IsResourceX ? 0 : 2;
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = (row.ResourceID.Count() >= maxResouceIdAmount) ? row.ResourceID.Substring(index, subStringLength) : string.Empty,
                        NotConvert = true
                    });
                }
                else
                {
                    betChoices.Add(new BetChoice
                    {
                        Code = betChoiceSchema.MatchAttribute,
                        Value = GetOddsValue(betChoiceSchema, GetPropValue(row, dataAttribute)),
                        NotConvert = betChoiceSchema.NotConvert
                    });
                }
            }

            return betChoices;
        }

        private static IList<BetChoice> MapBetChoicesReserve(BetTypeSchema betTypeSchema, MatchOddsData row)
        {
            var betChoices = new List<BetChoice>();
            var betChoice1 = betTypeSchema.BetChoices.ElementAt(1);
            var betChoice2 = betTypeSchema.BetChoices.ElementAt(2);

            var hdp1 = Convert.ToDecimal(GetPropValue(row, betTypeSchema.Reserve.FirstHdp));
            var hdp2 = Convert.ToDecimal(GetPropValue(row, betTypeSchema.Reserve.SecondHdp));

            betChoices.Add(new BetChoice
            {
                Code = betTypeSchema.BetChoices.ElementAt(0).MatchAttribute,
                Value = ConvertOddsToString(hdp2 - hdp1),
                NotConvert = true
            });

            betChoices.Add(new BetChoice
            {
                Code = betChoice1.MatchAttribute,
                Value = ConvertMalayToDecimalOddsString(Convert.ToDecimal(GetPropValue(row, GetDataAttribute(betChoice1))))
            });

            betChoices.Add(new BetChoice
            {
                Code = betChoice2.MatchAttribute,
                Value = ConvertMalayToDecimalOddsString(Convert.ToDecimal(GetPropValue(row, GetDataAttribute(betChoice2))))
            });

            AssignResourceOdds(betChoices, betTypeSchema, row);

            return betChoices;
        }

        private static void AssignResourceOdds(IList<BetChoice> betChoices, BetTypeSchema betTypeSchema, MatchOddsData row)
        {
            var resourceBetChoice = GetResourceBetChoice(betTypeSchema.BetChoices);

            if (resourceBetChoice != null)
            {
                betChoices.Add(new BetChoice
                {
                    Code = resourceBetChoice.MatchAttribute,
                    Value = row.ResourceID,
                    NotConvert = true
                });
            }
        }

        private static string GetDataAttribute(BetChoiceSchema betChoiceSchema)
            => string.IsNullOrEmpty(betChoiceSchema.DataAttribute) ? betChoiceSchema.MatchAttribute : betChoiceSchema.DataAttribute;

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

        private static object GetPropValue(MatchOddsData matchOddsData, string propName)
            => matchOddsData.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.GetValue(matchOddsData, null);
    }
}
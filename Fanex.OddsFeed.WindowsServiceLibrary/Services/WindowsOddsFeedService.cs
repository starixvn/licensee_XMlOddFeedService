namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using Common;
    using Common.Configurations;
    using Common.DTO;
    using Common.Helpers;
    using Common.Repository;
    using Common.Schemas;
    using Fanex.Logging;
    using Mapping;
    using Newtonsoft.Json;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class WindowsOddsFeedService : IWindowsOddsFeedService
    {
        private readonly NLog.Logger fileLogger = LogManager.GetCurrentClassLogger();
        private static readonly string cachePrefix = AppSettings.CachePrefix;
        private static readonly string oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string liveMatchBucket = AppSettings.BucketLiveMatches;
        private static readonly string deadMatchBucket = AppSettings.BucketDeadMatches;
        private static readonly string outRightBucket = AppSettings.BucketOutrights;
        private static readonly string currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly int totalVersion = AppSettings.TotalVersion;

        public void Reset()
        {
            Store(oddsFeedBucket, AppSettings.BetTypeSchemaKey, new List<BetTypeSchema>());
            Store(oddsFeedBucket, CacheKeys.CurrentKey(cachePrefix), -1);
            Store(oddsFeedBucket, CacheKeys.FullChainKey(cachePrefix), false);
        }

        public bool Update()
        {
            var result = false;
            var startDate = DateTime.Now;
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            fileLogger.Trace("Start - " + timeStamp);
            var betTypeSchemas = GetBetTypeSchemas();
            fileLogger.Trace("X - " + timeStamp);
            var liveMatches = GetLiveMatches(betTypeSchemas);
            fileLogger.Trace("Y - " + timeStamp);
            var deadBallMatches = GetDeadballMatches(betTypeSchemas);
            fileLogger.Trace("Done get from DB - " + timeStamp);

            var deadBallLength = deadBallMatches.Count;
            var splitIndex = deadBallLength / 2;
            var deadBalls1 = new List<Match>();
            var deadBalls2 = new List<Match>();

            for (var i = 0; i < splitIndex; i++)
            {
                deadBalls1.Add(deadBallMatches[i]);
            }

            for (var j = splitIndex; j < deadBallLength; j++)
            {
                deadBalls2.Add(deadBallMatches[j]);
            }

            fileLogger.Trace("Done split - " + timeStamp);

            fileLogger.Trace(deadBallMatches.Count);
            fileLogger.Trace(deadBalls1.Count);
            fileLogger.Trace(deadBalls2.Count);

            var outrights = GetOutrights();

            var currentVersion = GetCurrentVersion() + 1;
            if (currentVersion == totalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();

            result = Store(oddsFeedBucket, CacheKeys.CurrentKey(cachePrefix), currentVersion);

            result &= Store(liveMatchBucket, versionKey, liveMatches);
            fileLogger.Trace("A-" + timeStamp);
            result &= Store(deadMatchBucket, versionKey + "_1", deadBalls1);
            fileLogger.Trace("B-" + timeStamp);
            result &= Store(deadMatchBucket, versionKey + "_2", deadBalls2);
            fileLogger.Trace("C-" + timeStamp);
            result &= Store(outRightBucket, versionKey, outrights);
            fileLogger.Trace("D-" + timeStamp);

            if (result && currentVersion == totalVersion - 1)
            {
                result &= Store(oddsFeedBucket, CacheKeys.FullChainKey(cachePrefix), true);
            }

            fileLogger.Trace("E-" + timeStamp);

            var endDate = DateTime.Now;
            var duration = (endDate - startDate).TotalMilliseconds;
            fileLogger.Info("Update: " + duration);

            return result;
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var messages = new[]
            {
                "ENVIRONMENT: " + AppSettings.Environment,
                "ERROR: " + exception.Message,
                "SOURCE: " + exception.Source,
                "TARGETSITE: " + exception.TargetSite,
                "STACKTRACE: " + exception.StackTrace
            };

            return string.Join(Environment.NewLine, messages);
        }

        private IList<Match> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var liveOdds = DbAccess.GetLiveOddsData())
            {
                var matchMapping = new MatchMapping();
                var liveTables = liveOdds.Tables[0].Copy();
                var oddsFlex = GetOddsFlexGroups(liveOdds.Tables[1]);
                var matches = matchMapping.GetMatches(liveTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();
                var matchesTest = matchMapping.GetMatchesTest(liveTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();

                fileLogger.Info("matches: " + matches.Count);
                fileLogger.Info("matchesTest: " + matchesTest.Count);
                var isSame = true;

                for (var i = 0; i < matches.Count; i++)
                {
                    if (matches[i].MatchId != matchesTest[i].MatchId
                        || matches[i].LeagueId != matchesTest[i].LeagueId
                        || matches[i].LeagueEn != matchesTest[i].LeagueEn
                        || matches[i].LeagueTw != matchesTest[i].LeagueTw
                        || matches[i].LeagueCn != matchesTest[i].LeagueCn
                        || matches[i].LeagueIndonesia != matchesTest[i].LeagueIndonesia
                        || matches[i].MatchOddsList.Count != matchesTest[i].MatchOddsList.Count
                        )
                    {
                        fileLogger.Info("matches["+ i + "]: " );
                        fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                        fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                        isSame = false;
                        break;
                    }

                    var oddsList1 = matches[i].MatchOddsList.OrderBy(t => t.BetTypeId).ThenBy(t=>t.Time).ToList();
                    var oddsList2 = matchesTest[i].MatchOddsList.OrderBy(t => t.BetTypeId).ThenBy(t => t.Time).ToList();

                    for (var j = 0; j < oddsList1.Count; j++)
                    {
                        if (oddsList1[j].MatchId != oddsList2[j].MatchId
                            || oddsList1[j].BetTypeId != oddsList2[j].BetTypeId
                            || oddsList1[j].Tag != oddsList2[j].Tag
                            || oddsList1[j].Time != oddsList2[j].Time
                            || oddsList1[j].TimeStamp != oddsList2[j].TimeStamp
                            )
                        {
                            fileLogger.Info("matches[]: " + i + "- Odds[]: " + j);
                            fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                            fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                            fileLogger.Info(JsonConvert.SerializeObject(oddsList1[j]));
                            fileLogger.Info(JsonConvert.SerializeObject(oddsList2[j]));
                            isSame = false;
                            break;
                        }

                        var betChoices1 = oddsList1[j].BetChoices.OrderBy(t => t.Code).ToList();
                        var betChoices2 = oddsList2[j].BetChoices.OrderBy(t => t.Code).ToList();
                        for (var k = 0; k < betChoices1.Count; k++)
                        {
                            if (betChoices1[k].Code != betChoices2[k].Code
                               || betChoices1[k].Value != betChoices2[k].Value
                               || betChoices1[k].NotConvert != betChoices2[k].NotConvert
                                )
                            {
                                var oddsList3 = matches[i].MatchOddsList.Where(t => t.BetTypeId == oddsList1[j].BetTypeId).OrderBy(t => t.Time).ThenBy(t=>t.BetChoices[0].Code).ThenBy(t => t.BetChoices[0].Value).ToList();
                                var oddsList4 = matchesTest[i].MatchOddsList.Where(t => t.BetTypeId == oddsList1[j].BetTypeId).OrderBy(t => t.Time).ThenBy(t => t.BetChoices[0].Code).ThenBy(t => t.BetChoices[0].Value).ToList();
                                var check2 = true;
                                for (var n = 0; n < oddsList3.Count; n++)
                                {
                                    var betChoices3 = oddsList3[n].BetChoices.OrderBy(t => t.Code).ToList();
                                    var betChoices4 = oddsList4[n].BetChoices.OrderBy(t => t.Code).ToList();
                                    for (var m = 0; m < betChoices3.Count; m++)
                                    {
                                        if (betChoices3[m].Code != betChoices4[m].Code
                                       || betChoices3[m].Value != betChoices4[m].Value
                                        )
                                        {
                                            check2 = false;
                                        }
                                    }
                                }

                                if (!check2)
                                {
                                    fileLogger.Info("matches[]: " + i + "- Odds[]: " + j + "-betChoices: " + k);
                                    fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                                    fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                                    fileLogger.Info(JsonConvert.SerializeObject(betChoices1[k]));
                                    fileLogger.Info(JsonConvert.SerializeObject(betChoices2[k]));
                                    isSame = false;
                                    break;
                                }
                            }
                        }

                        if (!isSame)
                        {
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }

                }

                fileLogger.Info("Data Check Live: " + isSame);

                return matches;
            }
        }

        private IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var deadballOdds = DbAccess.GetDeadballOddsData())
            {
                var matchMapping = new MatchMapping();

                var deadballTables = deadballOdds.Tables[0].Copy();
                var oddsFlexTables = deadballOdds.Tables[1].Copy();
                var oddsFlex = GetOddsFlexGroups(oddsFlexTables);

                var matches = matchMapping.GetMatches(deadballTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();
                var matchesTest = matchMapping.GetMatchesTest(deadballTables, betTypeSchemas, oddsFlex, false).OrderBy(t => t.MatchId).ToList();

                fileLogger.Info("matches: " + matches.Count);
                fileLogger.Info("matchesTest: " + matchesTest.Count);
                var isSame = true;

                for (var i = 0; i < matches.Count; i++)
                {
                    if (matches[i].MatchId != matchesTest[i].MatchId
                        || matches[i].LeagueId != matchesTest[i].LeagueId
                        || matches[i].LeagueEn != matchesTest[i].LeagueEn
                        || matches[i].LeagueTw != matchesTest[i].LeagueTw
                        || matches[i].LeagueCn != matchesTest[i].LeagueCn
                        || matches[i].LeagueIndonesia != matchesTest[i].LeagueIndonesia

                        )
                    {
                        fileLogger.Info("matches[]: " + i);
                        fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                        fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                        isSame = false;
                        break;
                    }

                    var oddsList1 = matches[i].MatchOddsList.OrderBy(t => t.BetTypeId).ThenBy(t => t.Time).ToList();
                    var oddsList2 = matchesTest[i].MatchOddsList.OrderBy(t => t.BetTypeId).ThenBy(t => t.Time).ToList();
                    for (var j = 0; j < oddsList1.Count; j++)
                    {
                        if (oddsList1[j].MatchId != oddsList2[j].MatchId
                            || oddsList1[j].BetTypeId != oddsList2[j].BetTypeId
                            || oddsList1[j].Tag != oddsList2[j].Tag
                            || oddsList1[j].Time != oddsList2[j].Time
                            || oddsList1[j].TimeStamp != oddsList2[j].TimeStamp
                            )
                        {
                            fileLogger.Info("matches[]: " + i + "- Odds[]: " + j);
                            fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                            fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                            fileLogger.Info(JsonConvert.SerializeObject(oddsList1[j]));
                            fileLogger.Info(JsonConvert.SerializeObject(oddsList2[j]));
                            isSame = false;
                            break;
                        }

                        var betChoices1 = oddsList1[j].BetChoices.OrderBy(t => t.Code).ThenBy(t=>t.Value).ToList();
                        var betChoices2 = oddsList2[j].BetChoices.OrderBy(t => t.Code).ThenBy(t => t.Value).ToList();
                        for (var k = 0; k < betChoices1.Count; k++)
                        {
                            if (betChoices1[k].Code != betChoices2[k].Code
                               || betChoices1[k].Value != betChoices2[k].Value
                               || betChoices1[k].NotConvert != betChoices2[k].NotConvert
                                )
                            {
                                var oddsList3 = matches[i].MatchOddsList.Where(t => t.BetTypeId == oddsList1[j].BetTypeId).OrderBy(t => t.Time).ThenBy(t => t.BetChoices[0].Code).ThenBy(t => t.BetChoices[0].Value).ToList();
                                var oddsList4 = matchesTest[i].MatchOddsList.Where(t => t.BetTypeId == oddsList1[j].BetTypeId).OrderBy(t => t.Time).ThenBy(t => t.BetChoices[0].Code).ThenBy(t => t.BetChoices[0].Value).ToList();
                                var check2 = true;
                                for (var n = 0; n < oddsList3.Count; n++)
                                {
                                    var betChoices3 = oddsList3[n].BetChoices.OrderBy(t => t.Code).ToList();
                                    var betChoices4 = oddsList4[n].BetChoices.OrderBy(t => t.Code).ToList();
                                    for (var m = 0; m < betChoices3.Count; m++)
                                    {
                                        if (betChoices3[m].Code != betChoices4[m].Code
                                       || betChoices3[m].Value != betChoices4[m].Value
                                        )
                                        {
                                            check2 = false;
                                        }
                                    }
                                }

                                if (!check2)
                                {
                                    fileLogger.Info("matches[]: " + i + "- Odds[]: " + j + "-betChoices: " + k);
                                    fileLogger.Info(JsonConvert.SerializeObject(matches[i]));
                                    fileLogger.Info(JsonConvert.SerializeObject(matchesTest[i]));
                                    fileLogger.Info(JsonConvert.SerializeObject(betChoices1[k]));
                                    fileLogger.Info(JsonConvert.SerializeObject(betChoices2[k]));
                                    isSame = false;
                                    break;
                                }
                            }
                        }

                        if (!isSame)
                        {
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }

                }

                fileLogger.Info("Data Check DeadBall: " + isSame);

                return matches;
            }
        }

        private static IList<Outright> GetOutrights()
        {
            using (var outrightOdds = DbAccess.GetOutrightOddsData())
            {
                var outrightMapping = new OutrightMapping();
                return outrightMapping.GetOutrights(outrightOdds);
            }
        }

        private IEnumerable<OddsFlexGroup> GetOddsFlexGroups(DataTable table)
        {
            fileLogger.Trace("Start GetOddsFlexGroups");

            var oddsFlexGroups = new List<OddsFlexGroup>();

            foreach (DataRow row in table.Rows)
            {
                var oddsId = Convert.ToInt32(row["oddsid"]);
                var oddsFlex = new OddsFlex
                {
                    BetChoice = row["betChoice"].ToString(),
                    BetType = Convert.ToInt32(row["bettype"]),
                    OddId = oddsId,
                    Odds = Convert.ToDecimal(row["odds"]),
                    OddsInfo = row["OddsInfo"].ToString()
                };
                var oddsFlexGroup = oddsFlexGroups.FirstOrDefault(odds => odds.OddsId == oddsId);

                if (oddsFlexGroup == null)
                {
                    oddsFlexGroups.Add(new OddsFlexGroup
                    {
                        OddsId = oddsId,
                        OddsFlexs = new List<OddsFlex> { oddsFlex }
                    });
                }
                else
                {
                    oddsFlexGroup.OddsFlexs.Add(oddsFlex);
                }
            }

            fileLogger.Trace("End GetOddsFlexGroups");
            return oddsFlexGroups;
        }

        private static IList<BetTypeSchema> GetBetTypeSchemas()
        {
            var document = RedisRepository.ReadData<IList<BetTypeSchema>>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, AppSettings.BetTypeSchemaKey));

            if (document == null || document.Count == 0)
            {
                var newValue = JsonHelper.GetBetTypeSchemas<BetTypeSchema>(AppSettings.BetTypeSchemaPath);

                if (newValue != null)
                {
                    RedisRepository.StoreData<IList<BetTypeSchema>>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, AppSettings.BetTypeSchemaKey), newValue);
                    return newValue;
                }

                return new List<BetTypeSchema>();
            }

            return document;
        }

        private static bool Store<T>(string bucketName, string key, T value)
        {
            try
            {
                return RedisRepository.StoreData<T>(string.Format(Constants.RedisKeyFormat, bucketName, key), value);
            }
            catch (Exception ex)
            {
                var message = GetExceptionMessage(ex);
                LogService.Error(message);
                return false;
            }
        }

        private static int GetCurrentVersion()
        {
            return RedisRepository.ReadData<int>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, currentVersionKey));
        }
    }
}
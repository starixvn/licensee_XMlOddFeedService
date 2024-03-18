namespace Fanex.OddsFeed.WindowsServiceLibrary.UnitTests.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Common.Converters;
    using Common.DTO;
    using Common.Schemas;
    using Fanex.OddsFeed.Common.Mapping;
    using NUnit.Framework;

    [TestFixture]
    public class MatchMappingTest
    {
        private MatchMapping _target;

        [SetUp]
        public void Setup()
        {
            _target = new MatchMapping();
        }

        [Test]
        public void GetMatchNodes_NormalOdds_ReturnCorrectNormalMatchNode()
        {
            // Arrange
            var normalDataTable = GetNormalDataTable();
            var schemas = GetSchemas();

            // Action
            var result = _target.GetMatches(normalDataTable, schemas, null, false).First();

            // Assert
            Assert.AreEqual(normalDataTable.Rows[0]["MatchId"], result.MatchId);
            Assert.AreEqual(ConvertToDecimalOdds(normalDataTable.Rows[0]["Odds1A"]), result.MatchOddsList[0].BetChoices.ElementAt(0).Value);
            Assert.AreEqual(ConvertToDecimalOdds(normalDataTable.Rows[0]["Odds2A"]), result.MatchOddsList[0].BetChoices.ElementAt(1).Value);
        }

        [Test]
        public void GetMatchNodes_HandicapOdds_ReturnCorrectHandicapMatchNode()
        {
            // Arrange
            var handicapDataTable = GetHandicapDataTable();
            var schemas = GetSchemas();

            // Action
            var result = _target.GetMatches(handicapDataTable, schemas, null, false).First();

            // Assert
            Assert.AreEqual(handicapDataTable.Rows[0]["MatchId"], result.MatchId);
            Assert.AreEqual(ConvertToDecimalOdds(handicapDataTable.Rows[0]["Odds1A"]), result.MatchOddsList[0].BetChoices.ElementAt(1).Value);
            Assert.AreEqual(ConvertToDecimalOdds(handicapDataTable.Rows[0]["Odds2A"]), result.MatchOddsList[0].BetChoices.ElementAt(2).Value);
        }

        [Test]
        public void GetMatchNodes_FlexOdds_ReturnCorrectFlexOddsMatchNode()
        {
            // Arrange
            var oddsFlexDataTable = GetOddsFlexDataTable();
            var oddFlexGroups = GetOddFlexGroups();
            var schemas = GetSchemas();

            // Action
            var result = _target.GetMatches(oddsFlexDataTable, schemas, oddFlexGroups, false).First();

            // Assert
            Assert.AreEqual(oddsFlexDataTable.Rows[0]["MatchId"], result.MatchId);
            Assert.AreEqual(ConvertToDecimalOdds(oddFlexGroups.ElementAt(0).OddsFlexs.ElementAt(0).Odds), result.MatchOddsList[0].BetChoices.ElementAt(0).Value);
            Assert.AreEqual(ConvertToDecimalOdds(oddFlexGroups.ElementAt(0).OddsFlexs.ElementAt(1).Odds), result.MatchOddsList[0].BetChoices.ElementAt(1).Value);
        }

        [Test]
        public void GetMatchNodes_ResourceIdXYY_ReturnCorrectMatchNode()
        {
            // Arrange
            var resourceDataTable = GetResourceIDDataTable();
            var schemas = GetSchemas();

            // Action
            var result = _target.GetMatches(resourceDataTable, schemas, null, false).First();

            // Assert
            var resourceID = Convert.ToString(resourceDataTable.Rows[0]["resourceID"]);
            Assert.AreEqual(resourceDataTable.Rows[0]["MatchId"], result.MatchId);
            Assert.AreEqual(ConvertToDecimalOdds(resourceDataTable.Rows[0]["Odds1A"]), result.MatchOddsList[0].BetChoices.ElementAt(0).Value);
            Assert.AreEqual(ConvertToDecimalOdds(resourceDataTable.Rows[0]["Odds2A"]), result.MatchOddsList[0].BetChoices.ElementAt(1).Value);
            Assert.AreEqual((resourceID.Count() >= 4) ? resourceID.Substring(0, 2) : string.Empty, result.MatchOddsList[0].BetChoices.ElementAt(2).Value);
            Assert.AreEqual((resourceID.Count() >= 4) ? resourceID.Substring(2, 2) : string.Empty, result.MatchOddsList[0].BetChoices.ElementAt(3).Value);
        }

        private static string ConvertToDecimalOdds(object odds)
        {
            return string.Format("{0:0.#########}", OddsConverter.ToDecimalOddsFromMalayOdds(Convert.ToDecimal(odds)));
        }

        private static IEnumerable<BetTypeSchema> GetSchemas()
        {
            return new[]
            {
                new BetTypeSchema
                {
                    BetTypeId = 1,
                    Tag = "ah",
                    Reserve = new ReserveSchema
                    {
                        FirstHdp = "Hdp1",
                        SecondHdp = "Hdp2"
                    },
                    BetChoices = new []
                    {
                        new BetChoiceSchema
                        {
                            MatchAttribute = "h",
                            NotConvert = true
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "o",
                            DataAttribute = "Odds1A"
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "u",
                            DataAttribute = "Odds2A"
                        }
                    }
                },
                new BetTypeSchema
                {
                    BetTypeId = 2,
                    Tag = "oe",
                    BetChoices = new []
                    {
                        new BetChoiceSchema
                        {
                            MatchAttribute = "o",
                            DataAttribute = "Odds1A"
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "e",
                            DataAttribute = "Odds2A"
                        }
                    }
                },
                new BetTypeSchema
                {
                    BetTypeId = 3,
                    Tag = "cs_12",
                    IsOddsFlex = true,
                    BetChoices = new []
                    {
                        new BetChoiceSchema
                        {
                            MatchAttribute = "O01",
                            DataAttribute = "0:0"
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "O02",
                            DataAttribute = "0:1"
                        }
                    }
                },
                new BetTypeSchema
                {
                    BetTypeId = 613,
                    Tag = "bkb_613",
                    BetChoices = new []
                    {
                        new BetChoiceSchema
                        {
                            MatchAttribute = "h",
                            DataAttribute = "odds1a"
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "a",
                            DataAttribute = "odds2a"
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "x",
                            DataAttribute = "ResourceID.x",
                            IsResourceX = true
                        },
                        new BetChoiceSchema
                        {
                            MatchAttribute = "y",
                            DataAttribute = "ResourceID.yy",
                            IsResourceY = true
                        }
                    }
                }
            };
        }

        private static DataTable GetHandicapDataTable()
        {
            var dataTable = new DataTable();

            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "MatchID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "BetType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Hdp1"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Hdp2"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds1A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds2A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "ChangeTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "TimeStamp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }

            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "SportTypeName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "SportType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "KickOffTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "LeagueID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "CountryCode"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Home"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Away"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }

            var row = dataTable.NewRow();
            row["MatchID"] = 1;
            row["BetType"] = 1;
            row["Hdp1"] = 1;
            row["Hdp2"] = 0.5;
            row["Odds1A"] = 0.8;
            row["Odds2A"] = 0.9;
            row["ChangeTime"] = "11/01/2016 6:11:32 AM";
            row["TimeStamp"] = 123456;

            row["SportTypeName"] = "Test";
            row["SportType"] = 1;
            row["KickOffTime"] = "11/01/2016 6:11:32 AM";
            row["LeagueID"] = 1;
            row["CountryCode"] = "Test";
            row["Home"] = "Test";
            row["HomeCN"] = "Test";
            row["HomeC_UNI"] = "Test";
            row["HomeId"] = "Test";
            row["HomeTh"] = "Test";
            row["HomeKo"] = "Test";
            row["HomeVn"] = "Test";
            row["HomeJp"] = "Test";
            row["HomeHd"] = "Test";
            row["Away"] = "Test";
            row["AwayCN"] = "Test";
            row["AwayC_UNI"] = "Test";
            row["AwayId"] = "Test";
            row["AwayTh"] = "Test";
            row["AwayKo"] = "Test";
            row["AwayVn"] = "Test";
            row["AwayJp"] = "Test";
            row["AwayHd"] = "Test";
            row["LeagueName"] = "Test";
            row["LeagueNameCN"] = "Test";
            row["LeagueNameC_UNI"] = "Test";
            row["leaguenameId"] = "Test";
            row["leaguenameTh"] = "Test";
            row["leaguenameKo"] = "Test";
            row["leaguenameVn"] = "Test";
            row["leaguenameJp"] = "Test";
            row["leaguenameHd"] = "Test";

            dataTable.Rows.Add(row);

            return dataTable;
        }

        private static DataTable GetNormalDataTable()
        {
            var dataTable = new DataTable();

            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "MatchID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "BetType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds1A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds2A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "ChangeTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "TimeStamp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "SportTypeName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "SportType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "KickOffTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "LeagueID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "CountryCode"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Home"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Away"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }

            var row = dataTable.NewRow();
            row["MatchID"] = 1;
            row["BetType"] = 2;
            row["Odds1A"] = 0.8;
            row["Odds2A"] = 0.9;
            row["ChangeTime"] = "11/01/2016 6:11:32 AM";
            row["TimeStamp"] = 123456;
            row["SportTypeName"] = "Test";
            row["SportType"] = 1;
            row["KickOffTime"] = "11/01/2016 6:11:32 AM";
            row["LeagueID"] = 1;
            row["CountryCode"] = "Test";
            row["Home"] = "Test";
            row["HomeCN"] = "Test";
            row["HomeC_UNI"] = "Test";
            row["HomeId"] = "Test";
            row["HomeTh"] = "Test";
            row["HomeKo"] = "Test";
            row["HomeVn"] = "Test";
            row["HomeJp"] = "Test";
            row["HomeHd"] = "Test";
            row["Away"] = "Test";
            row["AwayCN"] = "Test";
            row["AwayC_UNI"] = "Test";
            row["AwayId"] = "Test";
            row["AwayTh"] = "Test";
            row["AwayKo"] = "Test";
            row["AwayVn"] = "Test";
            row["AwayJp"] = "Test";
            row["AwayHd"] = "Test";
            row["LeagueName"] = "Test";
            row["LeagueNameCN"] = "Test";
            row["LeagueNameC_UNI"] = "Test";
            row["leaguenameId"] = "Test";
            row["leaguenameTh"] = "Test";
            row["leaguenameKo"] = "Test";
            row["leaguenameVn"] = "Test";
            row["leaguenameJp"] = "Test";
            row["leaguenameHd"] = "Test";

            dataTable.Rows.Add(row);

            return dataTable;
        }

        private static DataTable GetResourceIDDataTable()
        {
            var dataTable = new DataTable();

            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "MatchID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "BetType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds1A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(decimal),
                ColumnName = "Odds2A"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "resourceID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "ChangeTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "TimeStamp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "SportTypeName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "SportType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "KickOffTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "LeagueID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "CountryCode"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Home"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Away"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }

            var row = dataTable.NewRow();
            row["MatchID"] = 1;
            row["BetType"] = 613;
            row["Odds1A"] = 0.8;
            row["Odds2A"] = 0.9;
            row["resourceID"] = "0115";
            row["ChangeTime"] = "11/01/2016 6:11:32 AM";
            row["TimeStamp"] = 123456;
            row["SportTypeName"] = "Test";
            row["SportType"] = 1;
            row["KickOffTime"] = "11/01/2016 6:11:32 AM";
            row["LeagueID"] = 1;
            row["CountryCode"] = "Test";
            row["Home"] = "Test";
            row["HomeCN"] = "Test";
            row["HomeC_UNI"] = "Test";
            row["HomeId"] = "Test";
            row["HomeTh"] = "Test";
            row["HomeKo"] = "Test";
            row["HomeVn"] = "Test";
            row["HomeJp"] = "Test";
            row["HomeHd"] = "Test";
            row["Away"] = "Test";
            row["AwayCN"] = "Test";
            row["AwayC_UNI"] = "Test";
            row["AwayId"] = "Test";
            row["AwayTh"] = "Test";
            row["AwayKo"] = "Test";
            row["AwayVn"] = "Test";
            row["AwayJp"] = "Test";
            row["AwayHd"] = "Test";
            row["LeagueName"] = "Test";
            row["LeagueNameCN"] = "Test";
            row["LeagueNameC_UNI"] = "Test";
            row["leaguenameId"] = "Test";
            row["leaguenameTh"] = "Test";
            row["leaguenameKo"] = "Test";
            row["leaguenameVn"] = "Test";
            row["leaguenameJp"] = "Test";
            row["leaguenameHd"] = "Test";

            dataTable.Rows.Add(row);

            return dataTable;
        }

        private static DataTable GetOddsFlexDataTable()
        {
            var dataTable = new DataTable();

            AssignOddsFlexHeader(dataTable);

            var row = dataTable.NewRow();

            row["MatchID"] = 3;
            row["BetType"] = 3;
            row["ChangeTime"] = "11/01/2016 6:11:32 AM";
            row["oddsid"] = 1;
            row["TimeStamp"] = 123456;
            row["SportTypeName"] = "Test";
            row["SportType"] = 1;
            row["KickOffTime"] = "11/01/2016 6:11:32 AM";
            row["LeagueID"] = 1;
            row["CountryCode"] = "Test";
            row["Home"] = "Test";
            row["HomeCN"] = "Test";
            row["HomeC_UNI"] = "Test";
            row["HomeId"] = "Test";
            row["HomeTh"] = "Test";
            row["HomeKo"] = "Test";
            row["HomeVn"] = "Test";
            row["HomeJp"] = "Test";
            row["HomeHd"] = "Test";
            row["Away"] = "Test";
            row["AwayCN"] = "Test";
            row["AwayC_UNI"] = "Test";
            row["AwayId"] = "Test";
            row["AwayTh"] = "Test";
            row["AwayKo"] = "Test";
            row["AwayVn"] = "Test";
            row["AwayJp"] = "Test";
            row["AwayHd"] = "Test";
            row["LeagueName"] = "Test";
            row["LeagueNameCN"] = "Test";
            row["LeagueNameC_UNI"] = "Test";
            row["leaguenameId"] = "Test";
            row["leaguenameTh"] = "Test";
            row["leaguenameKo"] = "Test";
            row["leaguenameVn"] = "Test";
            row["leaguenameJp"] = "Test";
            row["leaguenameHd"] = "Test";

            dataTable.Rows.Add(row);

            return dataTable;
        }

        private static void AssignOddsFlexHeader(DataTable dataTable)
        {
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "MatchID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "BetType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "ChangeTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "TimeStamp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "oddsid"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "SportTypeName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "SportType"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(DateTime),
                ColumnName = "KickOffTime"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "LeagueID"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "CountryCode"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Home"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Away"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueName"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameCN"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "LeagueNameC_UNI"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "HomeHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "AwayHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameId"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameTh"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameKo"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameVn"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameJp"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
            using (var dataColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "leaguenameHd"
            })
            {
                dataTable.Columns.Add(dataColumn);
            }
        }

        private static IEnumerable<OddsFlexGroup> GetOddFlexGroups()
        {
            return new[]
            {
                new OddsFlexGroup
                {
                    OddsId  =1,
                    OddsFlexs = new []
                    {
                        new OddsFlex { OddId = 1, Odds = 1, BetType = 3, BetChoice = "0:0" },
                        new OddsFlex { OddId = 1, Odds = 4, BetType = 3, BetChoice = "0:1" }
                    }
                }
            };
        }
    }
}
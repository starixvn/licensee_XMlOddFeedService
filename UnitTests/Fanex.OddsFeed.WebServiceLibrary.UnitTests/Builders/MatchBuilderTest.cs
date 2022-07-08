namespace Fanex.OddsFeed.WebServiceLibrary.UnitTests.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Common.DTO;
    using Models;
    using NUnit.Framework;
    using WebServiceLibrary.Builders;

    [TestFixture]
    public class MatchBuilderTest
    {
        private MatchBuilder _target;

        [SetUp]
        public void Setup()
        {
            _target = new MatchBuilder();
        }

        [Test]
        public void CreateMatchNodeList_DeadballMatchNodeList_ReturnDeadballMatchNodes()
        {
            // Arrange
            var matchXmlNode = GetDeadballMatchXmlNode();
            var oddsFeedNode = new StringBuilder();
            var model = new OddsFeedsModel
            {
                BookieName = "111candy",
                UserProfile = new UserProfile
                {
                    OddRegion = 1,
                    OddsType = new []{ 2 },
                    Languages = new List<string> { "en-US", "zh-TW", "zh-CN" }
                },
                UserQuery = new UserQuery()
            };

            // Action
            _target.AppendMatchNodes(
                oddsFeedNode,
                GetMatchNodes(false),
                model);

            // Assert
            Assert.AreEqual(matchXmlNode, oddsFeedNode.ToString());
        }

        [Test]
        public void CreateMatchNodeList_LiveMatchNodeList_ReturnLiveMatchNodes()
        {
            // Arrange 
            var matchXmlNode = GetLiveMatchXmlNode();
            var oddsFeedNode = new StringBuilder();
            var model = new OddsFeedsModel
            {
                BookieName = "111candy",
                UserProfile = new UserProfile
                {
                    OddRegion = 1,
                    OddsType = new[] { 2 },
                    Languages = new List<string> { "en-US", "zh-TW", "zh-CN" }
                },
                UserQuery = new UserQuery()
            };

            // Action
            _target.AppendMatchNodes(
                oddsFeedNode,
                GetMatchNodes(true),
                model);

            // Assert
            Assert.AreEqual(matchXmlNode, oddsFeedNode.ToString());
        }

        private static string GetDeadballMatchXmlNode()
        {
            return "<match id=\"20485706\" ge=\"UEFA EUROPA LEAGUE\" gt=\"GT-UEFA EUROPA LEAGUE\" gs=\"GS-UEFA EUROPA LEAGUE\" he=\"FC Astana\" ht=\"FC Astana\" hs=\"FC Astana\" ae=\"Olympiakos\" at=\"Olympiakos\" as=\"Olympiakos\" mt=\"11/3/2016 11:59:59 AM\" sport=\"Soccer\" running=\"0\">"
                + "<bookie name=\"111candy\" simp=\"111candy\" trad=\"111candy\">"
                + "<odd t=\"oe\" o=\"1\" e=\"0.9\" time=\"11/2/2016 3:51:07 PM\" timestamp=\"1477332693\" />"
                + "</bookie>"
                + "</match>";
        }

        private static string GetLiveMatchXmlNode()
        {
            return "<match id=\"20485706\" ge=\"UEFA EUROPA LEAGUE\" gt=\"GT-UEFA EUROPA LEAGUE\" gs=\"GS-UEFA EUROPA LEAGUE\" he=\"FC Astana\" ht=\"FC Astana\" hs=\"FC Astana\" ae=\"Olympiakos\" at=\"Olympiakos\" as=\"Olympiakos\" mt=\"11/3/2016 11:59:59 AM\" sport=\"Soccer\" running=\"1\">"
                + "<bookie name=\"111candy\" simp=\"111candy\" trad=\"111candy\">"
                + "<odd t=\"oe\" o=\"1\" e=\"0.9\" time=\"11/2/2016 3:51:07 PM\" timestamp=\"1477332693\" />"
                + "</bookie>"
                + "</match>";
        }

        private static SafeList<Match> GetMatchNodes(bool isLive)
        {
            return new SafeList<Match>
            {
                new Match
                {
                    MatchId = 20485706,
                    LeagueEn = "UEFA EUROPA LEAGUE",
                    LeagueTw = "GT-UEFA EUROPA LEAGUE",
                    LeagueCn = "GS-UEFA EUROPA LEAGUE",
                    HomeEn = "FC Astana",
                    HomeTw = "FC Astana",
                    HomeCn = "FC Astana",
                    AwayEn = "Olympiakos",
                    AwayTw = "Olympiakos",
                    AwayCn = "Olympiakos",
                    KickOffTime = new DateTime(2016, 11, 3, 11, 59, 59),
                    SportName = "Soccer",
                    Timestamp = 1477878874,
                    IsLive = isLive,
                    MatchOddsList = new []
                    {
                        new MatchOdds
                        {
                            Tag = "oe",
                            BetTypeId = 1,
                            Time = new DateTime(2016, 11, 2, 15, 51, 07),
                            Timestamp = 1477332693,
                            BetChoices = new[]
                            {
                                new BetChoice { Code = "o", Value = "2.0" },
                                new BetChoice { Code = "e", Value = "1.9" }
                            }
                        }
                    }
                }
            };
        }
    }
}

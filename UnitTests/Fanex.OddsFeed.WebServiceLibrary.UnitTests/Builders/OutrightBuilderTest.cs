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
    public class OutrightBuilderTest
    {
        private OutrightBuilder _target;

        [SetUp]
        public void Setup()
        {
            _target = new OutrightBuilder();
        }

        [Test]
        public void CreateOutrightsNode_OutrightNodeList_ReturnXmlNode()
        {
            // Arrange
            var outrightXmlNode = GetOutrightXmlNode();
            var oddsFeedNode = new StringBuilder();
            var outrights = GetOutrightNodes();
            var model = new OddsFeedsModel
            {
                UserProfile = new UserProfile(),
                UserQuery = new UserQuery(),
                IsOutright = true
            };
            var userLanguage = new UserLanguage
            {
                IsSupportedEnglish = true,
                IsSupportedTraditionalChinese = true,
                IsSupportedSimplifiedChinese = true
            };

            // Action
            _target.AppendOutrightsNode(oddsFeedNode, outrights, model, userLanguage);

            // Assert
            Assert.AreEqual(outrightXmlNode, oddsFeedNode.ToString());
        }

        private static string GetOutrightXmlNode()
        {
            return "<outrights>"
                + "<event id=\"9966\" ge=\"*UEFA EUROPA LEAGUE - GROUP A WINNER\" gt=\"*UEFA EUROPA LEAGUE - GROUP A WINNER\" gs=\"*UEFA EUROPA LEAGUE - GROUP A WINNER\" mt=\"12/8/2016 12:00:00 AM\" sport=\"Soccer\">"
                + "<odd id=\"7207912\" te=\"Manchester United\" tt=\"Manchester United\" ts=\"Manchester United\" o=\"0.3\" />"
                + "</event>"
                + "</outrights>";
        }

        private static IList<Outright> GetOutrightNodes()
        {
            return new[]
            {
                new Outright
                {
                    EventId = 9966,
                    EventEn = "*UEFA EUROPA LEAGUE - GROUP A WINNER",
                    EventTw = "*UEFA EUROPA LEAGUE - GROUP A WINNER",
                    EventCn = "*UEFA EUROPA LEAGUE - GROUP A WINNER",
                    SportId = 1,
                    SportName = "Soccer",
                    Time = new DateTime(2016,12,8),
                    OutrightOddsList = new SafeList<OutrightOdds>
                    {
                        new OutrightOdds
                        {
                            OddsId = 7207912,
                            Value = 0.3m,
                            OddNameEn = "Manchester United",
                            OddNameTw = "Manchester United",
                            OddNameCn = "Manchester United"
                        }
                    }
                }
            };
        }
    }
}

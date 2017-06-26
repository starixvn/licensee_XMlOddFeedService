namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Collections;
    using System.Web.Mvc;
    using Common.Configurations;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class InsertBetdaqController : BaseController
    {
        private const string FailedBadOddsId = "Failed - Bad oddsId";
        private const string Success = "OK";
        private const string SuccessAndAlready = "OK Already";

        private readonly IBetDaqService _betDaqService;

        public InsertBetdaqController()
        {
            _betDaqService = new BetDaqService();
        }

        public ActionResult Index(string username)
        {
            if (!username.Equals("betdaq", StringComparison.OrdinalIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }

            if (!AppSettings.IsDisableUMTimeFlag && IsDownTime())
            {
                return Content(FailedBadOddsId);
            }

            return Content(InsertBetdaq(username, RequestHelper.DomainName(Request.ServerVariables)));
        }

        private string InsertBetdaq(string username, string domainName)
        {
            var ipAddress = GetClientIpAddress();
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return FailedBadOddsId;
            }

            return GetMessage(_betDaqService.InsertBetDaq(GetSpParams(ipAddress), IsDownTime()));
        }

        private Hashtable GetSpParams(string ipAddress)
        {
            return new Hashtable
            {
                { "oddsid", Request["OddsId"] },
                { "hdp1", Request["Handicap1"] },
                { "hdp2", Request["Handicap2"] },
                { "betteam", Request["BetTeam"] },
                { "odds", Request["Odds"] },
                { "stake", Request["Stake"] },
                { "betdaqid", Request["RequestId"] },
                { "bet_ip", ipAddress }
            };
        }

        private static string GetMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 0:
                    return Success;

                case 2:
                    return SuccessAndAlready;

                default:
                    return FailedBadOddsId;
            }
        }
    }
}
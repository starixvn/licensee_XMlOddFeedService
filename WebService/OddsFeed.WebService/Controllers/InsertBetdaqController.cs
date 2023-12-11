namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public class InsertBetdaqController : BaseController
    {
        private const string FailedBadOddsId = "Failed - Bad oddsId";
        private const string Success = "OK";
        private const string SuccessAndAlready = "OK Already";
        private const int SuccessAndAlreadyRrrorCode = 2;

        private readonly IBetDaqService betDaqService;
        private readonly WebSettings webSettings;

        public InsertBetdaqController(IServiceProvider serviceProvider, WebSettings webSettings) : base(serviceProvider)
        {
            betDaqService = serviceProvider.GetRequiredService<IBetDaqService>();
            this.webSettings = webSettings;
        }

        public async Task<IActionResult> Index(string username)
        {
            if (!username.Equals("betdaq", StringComparison.OrdinalIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }

            if (!webSettings.IsDisableUMTimeFlag && await IsDownTimeAsync())
            {
                return Content(FailedBadOddsId);
            }

            return Content(await InsertBetdaq(username, RequestHelper.DomainName(HttpContext)));
        }

        private async Task<string> InsertBetdaq(string username, string domainName)
        {
            var ipAddress = GetClientIpAddress();
            var authenticatedPartner = await GetAuthenticatedPartner(username, domainName, ipAddress);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return FailedBadOddsId;
            }

            return GetMessage(await betDaqService.InsertBetDaq(GetSpParams(ipAddress), await IsDownTimeAsync()));
        }

        private Hashtable GetSpParams(string ipAddress)
        {
            return new Hashtable
            {
                { "oddsid", Request.GetValueFromRequestByKey("OddsId") },
                { "hdp1", Request.GetValueFromRequestByKey("Handicap1") },
                { "hdp2", Request.GetValueFromRequestByKey("Handicap2") },
                { "betteam", Request.GetValueFromRequestByKey("BetTeam") },
                { "odds", Request.GetValueFromRequestByKey("Odds") },
                { "stake", Request.GetValueFromRequestByKey("Stake") },
                { "betdaqid", Request.GetValueFromRequestByKey("RequestId") },
                { "bet_ip", ipAddress }
            };
        }

        private static string GetMessage(int errorCode)
        {
            switch (errorCode)
            {
                case 0:
                    return Success;

                case SuccessAndAlreadyRrrorCode:
                    return SuccessAndAlready;

                default:
                    return FailedBadOddsId;
            }
        }
    }
}
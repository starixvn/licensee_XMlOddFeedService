namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class BetDaqController : BaseController
    {
        private readonly IBetDaqService _betdaqService;

        public BetDaqController()
        {
            _betdaqService = new BetDaqService();
        }

        public ActionResult GetSchedule(string userName)
        {
            if (!userName.Equals("betdaq", StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }
            
            if (IsDownTime())
            {
                return Content(string.Empty);
            }
            
            var scheduleContent = GetScheduleContent(userName, RequestHelper.DomainName(Request.ServerVariables), HttpContext.Request["s"]);

            return Content(scheduleContent);
        }
        
        public ActionResult GetOdds(string userName)
        {
            if (!userName.Equals("betdaq", StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }

            if (IsDownTime())
            {
                return Content(string.Empty);
            }

            return Content(GetOdds(userName, RequestHelper.DomainName(Request.ServerVariables), HttpContext.Request["s"]));
        }
       
        private string GetScheduleContent(string username, string domainName, string filter)
        {
            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }
            
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, GetClientIpAddress());

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return _betdaqService.GetSchedule(filter);
            }

            return string.Empty;
        }

        private string GetOdds(string username, string domainName, string filter = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }

            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, GetClientIpAddress());
            
            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return _betdaqService.GetOddWithUserIsBetdaq(filter);                  
            }

            return string.Empty;
        }
    }
}

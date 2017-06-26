namespace Fanex.OddsFeed.WebServiceLibrary.Attributes
{
    using System.Text;
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;
    using WebServiceLibrary.Criteria;
    using WebServiceLibrary.Repositories;

    public sealed class PartnerPermissionAttribute : ActionFilterAttribute
    {
        private readonly IPartnerRepository _partnerRepository;

        public PartnerPermissionAttribute()
        {
            _partnerRepository = new PartnerRepository();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var username = filterContext.ActionParameters["username"].ToString();
            var domainName = RequestHelper.DomainName(request.ServerVariables);
            var ipAddress = RequestHelper.GetClientIpAddress();

            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                filterContext.Result = new ContentResult
                {
                    Content = authenticatedPartner.Error,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "text/xml"
                };
            }
            else
            {
                filterContext.ActionParameters["userProfile"] = authenticatedPartner.UserProfile;
            }
        }

        private AuthenticatedPartner GetAuthenticatedPartner(string username, string domainName, string ip)
        {
            return _partnerRepository.GetAuthenticatedPartner(new CheckPermissionCriteria
            {
                UserName = username,
                DomainName = domainName,
                Ip = ip
            });
        }
    }
}
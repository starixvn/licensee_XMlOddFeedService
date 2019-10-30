namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using App_Start;
    using Fanex.Caching;
    using OddsFeed.Common.DTO;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using WebServiceLibrary;
    using WebServiceLibrary.Criteria;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Repositories;

    [CustomHandleError]
    public class BaseController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IPartnerRepository _partnerRepository;
        private const int TimeExpireCache = 20;

        public BaseController()
        {
            _maintenanceService = new MaintenanceService();
            _partnerRepository = new PartnerRepository();
        }

        protected bool IsDownTime()
        {
            return _maintenanceService.IsUnderMaintenance();
        }

        protected UnderMaintenanceTime GetUndermaintenanceTime()
        {
            return _maintenanceService.GetUnderMaintenanceTime();
        }

        protected string GetDowntimeContent()
        {
            return ErrorBuilder.GetUMDownTime(GetUndermaintenanceTime().To);
        }

        protected AuthenticatedPartner GetAuthenticatedPartner(string userName)
        {
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            
            return GetAuthenticatedPartner(userName, domainName, ipAddress);
        }

        protected AuthenticatedPartner GetAuthenticatedPartner(string userName, string domainName, string ip)
        {
            var cacheKeyAuthenticatedPartner = "Partner_" + userName + ip + domainName;
            var authenticatedPartner = ConfiguredCacheService.Get<AuthenticatedPartner>(cacheKeyAuthenticatedPartner);
            if (authenticatedPartner == null)
            {
                authenticatedPartner = _partnerRepository.GetAuthenticatedPartner(new CheckPermissionCriteria
                {
                    UserName = userName,
                    DomainName = domainName,
                    Ip = ip
                });

                ConfiguredCacheService.Set(cacheKeyAuthenticatedPartner, authenticatedPartner,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(TimeExpireCache)));
            }

            WriteLogRequestAccess(userName, ip, domainName);

            return authenticatedPartner;
        }

        private void WriteLogRequestAccess(string username, string clientIpAddress, string domain)
        {
            var partnerRepository = new PartnerRepository();
            var criteria = new WriteLogRequestAccessCriteria
            {
                Ip = clientIpAddress,
                UserName = username,
                DomainName = domain
            };

            partnerRepository.WriteLogRequestAccess(criteria);
        }

        protected static string GetClientIpAddress()
        {
            return RequestHelper.GetClientIpAddress();
        }

        protected ActionResult XmlContent(string data)
        {
            return Content(data, "application/xml", Encoding.UTF8);
        }
    }
}
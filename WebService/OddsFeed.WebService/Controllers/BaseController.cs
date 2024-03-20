namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Builders;
    using OddsFeed.WebService.Criteria;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Filters;
    using OddsFeed.WebService.Services;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;

    [ServiceFilter(typeof(RateLimitFilterAttribute))]
    public class BaseController : Controller
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IPartnerService partnerService;
        private readonly IBaseService baseService;
        private readonly IRedisCacheService redisCacheService;
        private const int TimeExpireCache = 60;

        public BaseController(IServiceProvider serviceProvider)
        {
            maintenanceService = serviceProvider.GetRequiredService<IMaintenanceService>();
            partnerService = serviceProvider.GetRequiredService<IPartnerService>();
            baseService = serviceProvider.GetRequiredService<IBaseService>();
            redisCacheService = serviceProvider.GetRequiredService<IRedisCacheService>();
        }

        protected async Task<bool> IsDownTimeAsync()
            => await maintenanceService.IsUnderMaintenanceAsync();

        protected async Task<UnderMaintenanceTime> GetUndermaintenanceTimeAsync()
            => await maintenanceService.GetUnderMaintenanceTimeAsync();

        protected async Task<string> GetDowntimeContent()
            => ErrorBuilder.GetUMDownTime((await GetUndermaintenanceTimeAsync()).To);

        protected async Task<AuthenticatedPartner> GetAuthenticatedPartner(string userName)
        {
            var domainName = RequestHelper.DomainName(HttpContext);
            var ipAddress = GetClientIpAddress();

            return await GetAuthenticatedPartner(userName, domainName, ipAddress);
        }

        protected async Task<AuthenticatedPartner> GetAuthenticatedPartner(string userName, string domainName, string ip)
        {
            var cacheKeyAuthenticatedPartner = "Partner" + userName + ip + domainName;
            var authenticatedPartner = await redisCacheService.GetAsync<AuthenticatedPartner>(cacheKeyAuthenticatedPartner);
            if (authenticatedPartner == null)
            {
                authenticatedPartner = await partnerService.GetAuthenticatedPartnerAsync(new CheckPermissionCriteria
                {
                    UserName = userName,
                    DomainName = domainName,
                    Ip = ip
                });

                var cacheItemOptions = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(TimeExpireCache));
                await redisCacheService.SetAsync(cacheKeyAuthenticatedPartner, authenticatedPartner, cacheItemOptions);
            }

            await WriteLogRequestAccess(userName, ip, domainName);

            return authenticatedPartner;
        }

        private async Task WriteLogRequestAccess(string username, string clientIpAddress, string domain)
        {
            var criteria = new WriteLogRequestAccessCriteria
            {
                Ip = clientIpAddress,
                UserName = username,
                DomainName = domain
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        protected string GetClientIpAddress()
            => RequestHelper.GetClientIpAddress(HttpContext);

        protected ActionResult XmlContent(string data)
            => Content(data, "application/xml", Encoding.UTF8);
    }
}
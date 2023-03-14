namespace Fanex.OddsFeed.Web.Controllers
{
    using System.Text;
    using Fanex.Caching;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Filters;
    using Fanex.OddsFeed.Web.Services.Constants;
    using Fanex.OddsFeed.Web.Services.Criteria;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Repositories.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;

    [ServiceFilter(typeof(RateLimitFilterAttribute))]
    public class BaseController : Controller
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IPartnerRepository partnerRepository;
        private readonly IBaseService baseService;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogServiceProvider logServiceProvider;
        private const int TimeExpireCache = 60;

        public BaseController(IServiceProvider serviceProvider)
        {
            maintenanceService = serviceProvider.GetRequiredService<IMaintenanceService>();
            partnerRepository = serviceProvider.GetRequiredService<IPartnerRepository>();
            baseService = serviceProvider.GetRequiredService<IBaseService>();
            redisCacheService = serviceProvider.GetRequiredService<IRedisCacheService>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
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
                authenticatedPartner = await partnerRepository.GetAuthenticatedPartner(new CheckPermissionCriteria
                {
                    UserName = userName,
                    DomainName = domainName,
                    Ip = ip
                });

                await redisCacheService.SetAsync(cacheKeyAuthenticatedPartner, authenticatedPartner,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(TimeExpireCache)));
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
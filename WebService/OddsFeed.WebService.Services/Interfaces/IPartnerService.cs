namespace OddsFeed.WebService.Services
{
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Criteria;

    public interface IPartnerService
    {
        Task<string> GetBetTypesXmlContentAsync(string username, UserProfile userProfile);

        Task<string> GetLeaguesXmlContentAsync(string username, UserProfile userProfile);

        Task<string> GetSportsXmlContentAsync(string username, UserProfile userProfile);

        Task<AuthenticatedPartner> GetAuthenticatedPartnerAsync(CheckPermissionCriteria criteria);
    }
}
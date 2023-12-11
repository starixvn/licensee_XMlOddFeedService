namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Criteria;

    public interface IPartnerService
    {
        Task<string> GetBetTypesXmlContent(string username, UserProfile userProfile);

        Task<string> GetLeaguesXmlContent(string username, UserProfile userProfile);

        Task<string> GetSportsXmlContent(string username, UserProfile userProfile);

        Task<AuthenticatedPartner> GetAuthenticatedPartnerAsync(CheckPermissionCriteria criteria);
    }
}
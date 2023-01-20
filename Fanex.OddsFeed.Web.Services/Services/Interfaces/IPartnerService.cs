namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using Fanex.OddsFeed.Models.DTO;

    public interface IPartnerService
    {
        Task<string> GetBetTypesXmlContent(string username, UserProfile userProfile);

        Task<string> GetLeaguesXmlContent(string username, UserProfile userProfile);

        Task<string> GetSportsXmlContent(string username, UserProfile userProfile);
    }
}
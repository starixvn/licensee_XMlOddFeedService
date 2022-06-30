namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Fanex.OddsFeed.Common.DTO;

    public interface IPartnerService
    {
        string GetBetTypesXmlContent(string username, UserProfile userProfile);

        string GetLeaguesXmlContent(string username, UserProfile userProfile);

        string GetSportsXmlContent(string username, UserProfile userProfile);
    }
}

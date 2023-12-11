namespace OddsFeed.WebService.Core.Filters
{
    using OddsFeed.Models.DTO;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Outright;

    public interface IOutrightFilter
    {
        bool FilterOutright(Outright outright, UserProfile userProfile, UserQuery userQuery);
    }
}
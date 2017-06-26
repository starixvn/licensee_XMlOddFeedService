namespace Fanex.OddsFeed.WebServiceLibrary.Filters
{
    using Common.DTO;

    public interface IOutrightFilter
    {
        bool FilterOutright(Outright outright, UserProfile userProfile, UserQuery userQuery);
    }
}

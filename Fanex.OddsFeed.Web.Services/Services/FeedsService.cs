namespace Fanex.OddsFeed.Web.Services.Services
{
    using System.Globalization;
    using System.Text;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Criteria;
    using Fanex.OddsFeed.Web.Services.Enum;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Newtonsoft.Json;

    public class FeedsService : IFeedsService
    {
        private readonly IBaseService baseService;

        public FeedsService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<string> GetFeedsData(FormatType formatType, string feedName, string username, string domain, string ipAddress, int? fromid)
        {
            string feeds;

            switch (feedName.ToLower(CultureInfo.InvariantCulture))
            {
                case "ip":
                    feeds = await GetIpFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "team":
                    feeds = await GetTeamFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "league":
                    feeds = await GetLeagueFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "country":
                    feeds = await GetCountryFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "sport":
                    feeds = await GetSportFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "event":
                    feeds = await GetEventFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                default:
                    feeds = string.Empty;
                    break;
            }

            return feeds;
        }

        private async Task<string> GetIpFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var ipList = await baseService.GetDataAsync<Ip>(new GetIpCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(ipList)
                : GetFeedsString("Ips", ipList);
        }

        private async Task<string> GetTeamFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var teamList = await baseService.GetDataAsync<Team>(new GetTeamCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(teamList)
                : GetFeedsString("Teams", teamList);
        }

        private async Task<string> GetLeagueFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var leagueList = await baseService.GetDataAsync<League>(new GetLeagueCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(leagueList)
                : GetFeedsString("Leagues", leagueList);
        }

        private async Task<string> GetCountryFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var countryList = await baseService.GetDataAsync<Country>(new GetCountryCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(countryList)
                : GetFeedsString("Countries", countryList);
        }

        private async Task<string> GetSportFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var sportList = await baseService.GetDataAsync<Sport>(new GetSportCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(sportList)
                : GetFeedsString("Sports", sportList);
        }

        private async Task<string> GetEventFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var eventList = await baseService.GetDataAsync<Event>(new GetEventCriteria
            {
                Username = username,
                Domain = domain,
                Ip = ipAddress,
                FromId = fromid
            });

            return formatType == FormatType.Json
                ? JsonConvert.SerializeObject(eventList)
                : GetFeedsString("Events", eventList);
        }

        private static string GetFeedsString<T>(string rootName, IEnumerable<T> feedsList)
        {
            var feedsString = new StringBuilder($"<{rootName}>");

            foreach (var feeds in feedsList)
            {
                AppendFeeds(feedsString, feeds);
            }

            feedsString.Append($"</{rootName}>");

            return feedsString.ToString();
        }

        private static void AppendFeeds<T>(StringBuilder feedsString, T feeds)
        {
            var type = feeds.GetType();
            var properties = type.GetProperties();

            feedsString.Append($"<{type.Name}>");

            foreach (var property in properties)
            {
                feedsString.Append($"<{property.Name}>{property.GetValue(feeds, null)}</{property.Name}>");
            }

            feedsString.Append($"</{type.Name}>");
        }
    }
}
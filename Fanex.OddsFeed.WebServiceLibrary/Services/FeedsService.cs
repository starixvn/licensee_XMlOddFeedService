namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Criteria;
    using Fanex.OddsFeed.Common.Configurations;
    using Newtonsoft.Json;
    using Repositories;

    public class FeedsService : IFeedsService
    {
        private readonly IFeedsRepository _feedsRepository;

        public FeedsService()
        {
            _feedsRepository = new FeedsRepository();
        }

        public string GetFeedsData(
            FormatType formatType,
            string feedName,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            string feeds;

            switch (feedName.ToLower(CultureInfo.InvariantCulture))
            {
                case "ip":
                    feeds = GetIpFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "team":
                    feeds = GetTeamFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "league":
                    feeds = GetLeagueFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "country":
                    feeds = GetCountryFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "sport":
                    feeds = GetSportFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                case "event":
                    feeds = GetEventFeeds(formatType, username, domain, ipAddress, fromid);
                    break;

                default:
                    feeds = string.Empty;
                    break;
            }

            return feeds;
        }

        private string GetIpFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var ipList = _feedsRepository.GetIpList(new GetIpCriteria
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

        private string GetTeamFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var teamList = _feedsRepository.GetTeamList(new GetTeamCriteria
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

        private string GetLeagueFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var leagueList = _feedsRepository.GetLeagueList(new GetLeagueCriteria
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

        private string GetCountryFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var countryList = _feedsRepository.GetCountryList(new GetCountryCriteria
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

        private string GetSportFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var sportList = _feedsRepository.GetSportList(new GetSportCriteria
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

        private string GetEventFeeds(
            FormatType formatType,
            string username,
            string domain,
            string ipAddress,
            int? fromid)
        {
            var eventList = _feedsRepository.GetEventList(new GetEventCriteria
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
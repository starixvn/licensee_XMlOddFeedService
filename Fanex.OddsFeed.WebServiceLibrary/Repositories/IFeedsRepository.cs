namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using Common.DTO;
    using Criteria;

    public interface IFeedsRepository
    {
        IEnumerable<Event> GetEventList(GetEventCriteria criteria);

        IEnumerable<Team> GetTeamList(GetTeamCriteria criteria);

        IEnumerable<Country> GetCountryList(GetCountryCriteria criteria);

        IEnumerable<League> GetLeagueList(GetLeagueCriteria criteria);

        IEnumerable<Sport> GetSportList(GetSportCriteria criteria);

        IEnumerable<Ip> GetIpList(GetIpCriteria criteria);
    }
}

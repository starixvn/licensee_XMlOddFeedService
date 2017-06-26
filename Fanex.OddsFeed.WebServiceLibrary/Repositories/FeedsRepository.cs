namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using Common.DTO;
    using Criteria;

    public class FeedsRepository: BaseRepository, IFeedsRepository
    {
        public IEnumerable<Event> GetEventList(GetEventCriteria criteria)
        {
            return GetListOf<Event>(criteria);
        }

        public IEnumerable<Team> GetTeamList(GetTeamCriteria criteria)
        {
            return GetListOf<Team>(criteria);
        }

        public IEnumerable<Country> GetCountryList(GetCountryCriteria criteria)
        {
            return GetListOf<Country>(criteria);
        }

        public IEnumerable<League> GetLeagueList(GetLeagueCriteria criteria)
        {
            return GetListOf<League>(criteria);
        }

        public IEnumerable<Sport> GetSportList(GetSportCriteria criteria)
        {
            return GetListOf<Sport>(criteria);
        }

        public IEnumerable<Ip> GetIpList(GetIpCriteria criteria)
        {
            return GetListOf<Ip>(criteria);
        }
    }
}

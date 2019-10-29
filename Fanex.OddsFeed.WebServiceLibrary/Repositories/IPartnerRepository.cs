namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Criteria;

    public interface IPartnerRepository
    {
        AuthenticatedPartner GetAuthenticatedPartner(CheckPermissionCriteria criteria);

        bool WriteLogRequestAccess(WriteLogRequestAccessCriteria criteria);

        IEnumerable<PartnerBetType> GetPartnerBetTypes(GetPartnerBetTypeCriteria criteria);

        IEnumerable<PartnerLeague> GetPartnerLeagues(GetPartnerLeagueCriteria criteria);

        IEnumerable<PartnerSport> GetPartnerSports(GetPartnerSportTypeCriteria criteria);
    }
}

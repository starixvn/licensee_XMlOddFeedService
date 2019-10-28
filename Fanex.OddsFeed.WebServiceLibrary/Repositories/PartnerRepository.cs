namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Converters;
    using Common.DataObjects;
    using Common.DTO;
    using Criteria;
    using Fanex.Common;
    using Fanex.Data;

    public class PartnerRepository : BaseRepository, IPartnerRepository
    {
        private const string All = "all";

        public AuthenticatedPartner GetAuthenticatedPartner(CheckPermissionCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var inputParams = GetInputParams(criteria);

                var profileDataList = db.Query<ProfileData>(inputParams);
                var outputParams = db.GetOuputParams();

                var authenticatedPartner = new AuthenticatedPartner
                {
                    SystemName = Converter.ToString2(outputParams["systemname"]),
                    UserProfile = profileDataList.Any() ? GetUserProfile(profileDataList.FirstOrDefault()) : null,
                    Error = ErrorBuilder.GetError(Converter.ToInt(outputParams[ErrorCode]), outputParams[ErrorMsg].ToString())
                };

                if (string.IsNullOrEmpty(authenticatedPartner.Error) 
                    && authenticatedPartner.UserProfile == null)
                {
                    authenticatedPartner.Error = ErrorBuilder.GetUnAuthenticated();
                }

                return authenticatedPartner;
            }
        }

        public IEnumerable<PartnerBetType> GetPartnerBetTypes(GetPartnerBetTypeCriteria criteria)
        {
            return GetListOf<PartnerBetType>(criteria);
        }

        public IEnumerable<PartnerLeague> GetPartnerLeagues(GetPartnerLeagueCriteria criteria)
        {
            return GetListOf<PartnerLeague>(criteria);
        }

        public IEnumerable<PartnerSport> GetPartnerSports(GetPartnerSportTypeCriteria criteria)
        {
            return GetListOf<PartnerSport>(criteria);
        }

        private static Hashtable GetInputParams(CheckPermissionCriteria criteria)
        {
            var inputParams = criteria.GetSpParams();

            inputParams.Add(ErrorCode, 0);
            inputParams.Add(ErrorMsg, string.Empty);

            return inputParams;
        }

        private static UserProfile GetUserProfile(ProfileData profileData)
        {
            return new UserProfile
            {
                OddsType = ListConverter.ConvertToListInt(profileData.OddsType),
                OddRegion = profileData.OddsRegion,
                DelayTimes = ListConverter.ConvertToListInt(profileData.DelayTimes),
                Languages = ListConverter.ConvertToListString(profileData.LanguageList),
                Sports = string.Equals(profileData.SportIDList, All) ? null : ListConverter.ConvertToListInt(profileData.SportIDList),
                Countries = string.Equals(profileData.CountryCodeList, All) ? null : ListConverter.ConvertToListString(profileData.CountryCodeList),
                Leagues = string.Equals(profileData.LeagueIDList, All) ? null : ListConverter.ConvertToListInt(profileData.LeagueIDList),
                Outrights = string.Equals(profileData.OutrightIDList, All) ? null : ListConverter.ConvertToListInt(profileData.OutrightIDList),
                BetTypes = string.IsNullOrEmpty(profileData.BetTypeIDList) || string.Equals(profileData.BetTypeIDList, All) ? null : ListConverter.ConvertToListInt(profileData.BetTypeIDList),
                LeagueKeyword = profileData.LeagueKeyword,
                OutrightsKeyword = profileData.LeagueKeyword_O,
                Domain = profileData.Domain
            };
        }
    }
}

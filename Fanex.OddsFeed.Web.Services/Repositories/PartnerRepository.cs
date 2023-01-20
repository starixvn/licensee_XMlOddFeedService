namespace Fanex.OddsFeed.Web.Services.Repositories
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Models.Data;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Constants;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Repositories.Interfaces;

    public class PartnerRepository : IPartnerRepository
    {
        public async Task<AuthenticatedPartner> GetAuthenticatedPartner(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var profileDataList = await db.QueryAsync<ProfileData>(criteria);
                var outputParams = db.GetOutputParams();

                var authenticatedPartner = new AuthenticatedPartner
                {
                    SystemName = outputParams["SystemName"] is DBNull ? string.Empty : Convert.ToString(outputParams["systemname"]),
                    UserProfile = profileDataList.Any() ? GetUserProfile(profileDataList.FirstOrDefault()) : null,
                    Error = GetErrorMessage(outputParams)
                };

                if (string.IsNullOrEmpty(authenticatedPartner.Error) && authenticatedPartner.UserProfile == null)
                {
                    authenticatedPartner.Error = ErrorBuilder.GetUnAuthenticated();
                }

                return authenticatedPartner;
            }
        }

        private string GetErrorMessage(Hashtable outputParams)
        {
            var errCode = (outputParams["errCode"] is DBNull) ? 0 : Convert.ToInt32(outputParams["errCode"]);
            var errMsg = (outputParams["errMsg"] is DBNull) ? string.Empty : Convert.ToString(outputParams["errMsg"]);
            return ErrorBuilder.GetError(errCode, errMsg);
        }

        private static UserProfile GetUserProfile(ProfileData profileData)
        {
            const string All = "all";
            return new UserProfile
            {
                OddsType = ListConverter.ConvertToListInt(profileData.OddsType),
                OddRegion = profileData.OddsRegion,
                OddsDelayTimes = ListConverter.ConvertToListInt(profileData.OddsDelayTimes),
                Languages = ListConverter.ConvertToListString(profileData.LanguageList),
                Sports = string.Equals(profileData.SportIDList, All) ? null : ListConverter.ConvertToListInt(profileData.SportIDList),
                Countries = string.Equals(profileData.CountryCodeList, All) ? null : ListConverter.ConvertToListString(profileData.CountryCodeList),
                Leagues = string.Equals(profileData.LeagueIDList, All) ? null : ListConverter.ConvertToListInt(profileData.LeagueIDList),
                Outrights = string.Equals(profileData.OutrightIDList, All) ? null : ListConverter.ConvertToListInt(profileData.OutrightIDList),
                BetTypes = string.IsNullOrEmpty(profileData.BetTypeIDList) || string.Equals(profileData.BetTypeIDList, All) ? null : ListConverter.ConvertToListInt(profileData.BetTypeIDList),
                LeagueKeyword = profileData.LeagueKeyword,
                OutrightsKeyword = profileData.LeagueKeyword_O,
                Domain = profileData.Domain,
                ResultDelayTimes = !string.IsNullOrWhiteSpace(profileData.ResultDelayTimes) ? int.Parse(profileData.ResultDelayTimes) : 0
            };
        }
    }
}
namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Common.DTO;
    using Common.Helpers;
    using Criteria;
    using Repositories;
    using System.Security;
    using System.Text;

    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _repository;
        private readonly IBetTypeService _betTypeService;
        private const string Language_English = "en-US";
        private const string Language_TraditionalChinese = "zh-TW";
        private const string Language_SimplifiedChinese = "zh-CN";
        private const string Language_Indonesia = "id-ID";
        private const string Language_Thai = "th-TH";
        private const string Language_Korean = "ko-KR";
        private const string Language_Vietnamese = "vi-VN";
        private const string Language_Japanese = "ja-JP";
        private const string Language_Hindi = "hi-IN";

        public PartnerService()
        {
            _repository = new PartnerRepository();
            _betTypeService = new BetTypeService();
        }

        public string GetBetTypesXmlContent(string username, UserProfile userProfile)
        {
            var bettypes = _repository.GetPartnerBetTypes(new GetPartnerBetTypeCriteria { Username = username });
            var content = new StringBuilder("<bettypes>");

            foreach (var bettype in bettypes)
            {
                content.Append($"<type id=\"{bettype.Id}\" t=\"{_betTypeService.GetBetTypeNameById(bettype.Id)}\"");

                if (userProfile.Languages.Contains(Language_English))
                {
                    content.Append($" te=\"{SecurityElement.Escape(bettype.NameEnglish)}\"");
                }

                if (userProfile.Languages.Contains(Language_TraditionalChinese))
                {
                    content.Append($" tt=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameTraditionalChinese, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    content.Append($" ts=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameSimplifiedChinese, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    content.Append($" tb=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameIndonesia, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    content.Append($" ta=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameThai, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    content.Append($" tk=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameKorean, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    content.Append($" tv=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameVietnamese, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    content.Append($" tj=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameJapanese, bettype.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    content.Append($" th=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameHindi, bettype.NameEnglish))}\"");
                }

                content.Append($" />");
            }

            content.Append("</bettypes>");

            return content.ToString();
        }

        public string GetLeaguesXmlContent(string username, UserProfile userProfile)
        {
            var leagues = _repository.GetPartnerLeagues(new GetPartnerLeagueCriteria { Username = username });
            var content = new StringBuilder("<leagues>");

            foreach (var league in leagues)
            {
                content.Append($"<league id=\"{league.LeagueId}\" sid=\"{league.SportId}\" se=\"{SecurityElement.Escape(league.SportName)}\"");

                if (userProfile.Languages.Contains(Language_English))
                {
                    content.Append($" ge=\"{SecurityElement.Escape(league.NameEnglish)}\"");
                }

                if (userProfile.Languages.Contains(Language_TraditionalChinese))
                {
                    content.Append($" gt=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameTraditionalChinese, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    content.Append($" gs=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameSimplifiedChinese, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    content.Append($" gb=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameIndonesia, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    content.Append($" ga=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameThai, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    content.Append($" gk=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameKorean, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    content.Append($" gv=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameVietnamese, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    content.Append($" gj=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameJapanese, league.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    content.Append($" gh=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameHindi, league.NameEnglish))}\"");
                }

                content.Append(" />");
            }

            content.Append("</leagues>");

            return content.ToString();
        }

        public string GetSportsXmlContent(string username, UserProfile userProfile)
        {
            var sports = _repository.GetPartnerSports(new GetPartnerSportTypeCriteria { Username = username });
            var content = new StringBuilder("<sports>");

            foreach (var sport in sports)
            {
                content.Append($"<sport id=\"{sport.Id}\"");

                if (userProfile.Languages.Contains(Language_English))
                {
                    content.Append($" se=\"{SecurityElement.Escape(sport.NameEnglish)}\"");
                }

                if (userProfile.Languages.Contains(Language_TraditionalChinese))
                {
                    content.Append($" st=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameTraditionalChinese, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    content.Append($" ss=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameSimplifiedChinese, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    content.Append($" sb=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameIndonesia, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    content.Append($" sa=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameThai, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    content.Append($" sk=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameKorean, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    content.Append($" sv=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameVietnamese, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    content.Append($" sj=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameJapanese, sport.NameEnglish))}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    content.Append($" sh=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameHindi, sport.NameEnglish))}\"");
                }

                content.Append("/>");
            }

            content.Append("</sports>");

            return content.ToString();
        }
    }
}

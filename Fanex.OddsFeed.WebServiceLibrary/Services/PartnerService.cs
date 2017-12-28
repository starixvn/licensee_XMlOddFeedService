namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using Common.DTO;
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
                    var traditionalChinese = string.IsNullOrEmpty(bettype.NameTraditionalChinese)
                        ? bettype.NameEnglish
                        : bettype.NameTraditionalChinese;
                    content.Append($" tt=\"{SecurityElement.Escape(traditionalChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    var simplifiedChinese = string.IsNullOrEmpty(bettype.NameSimplifiedChinese)
                        ? bettype.NameEnglish
                        : bettype.NameSimplifiedChinese;
                    content.Append($" ts=\"{SecurityElement.Escape(simplifiedChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    var indonesia = string.IsNullOrEmpty(bettype.NameIndonesia)
                        ? bettype.NameEnglish
                        : bettype.NameIndonesia;
                    content.Append($" tb=\"{SecurityElement.Escape(indonesia)}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    var thai = string.IsNullOrEmpty(bettype.NameThai)
                        ? bettype.NameEnglish
                        : bettype.NameThai;
                    content.Append($" ta=\"{SecurityElement.Escape(thai)}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    var korean = string.IsNullOrEmpty(bettype.NameKorean)
                        ? bettype.NameEnglish
                        : bettype.NameKorean;
                    content.Append($" tk=\"{SecurityElement.Escape(korean)}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    var vietnamese = string.IsNullOrEmpty(bettype.NameVietnamese)
                        ? bettype.NameEnglish
                        : bettype.NameVietnamese;
                    content.Append($" tv=\"{SecurityElement.Escape(vietnamese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    var japanese = string.IsNullOrEmpty(bettype.NameJapanese)
                        ? bettype.NameEnglish
                        : bettype.NameJapanese;
                    content.Append($" tj=\"{SecurityElement.Escape(japanese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    var hindi = string.IsNullOrEmpty(bettype.NameHindi)
                        ? bettype.NameEnglish
                        : bettype.NameHindi;
                    content.Append($" th=\"{SecurityElement.Escape(hindi)}\"");
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
                    content.Append($" gt=\"{SecurityElement.Escape(league.NameTraditionalChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    content.Append($" gs=\"{SecurityElement.Escape(league.NameSimplifiedChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    content.Append($" gb=\"{SecurityElement.Escape(league.NameIndonesia)}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    content.Append($" ga=\"{SecurityElement.Escape(league.NameThai)}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    content.Append($" gk=\"{SecurityElement.Escape(league.NameKorean)}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    content.Append($" gv=\"{SecurityElement.Escape(league.NameVietnamese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    content.Append($" gj=\"{SecurityElement.Escape(league.NameJapanese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    content.Append($" gh=\"{SecurityElement.Escape(league.NameHindi)}\"");
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
                    content.Append($" st=\"{SecurityElement.Escape(sport.NameTraditionalChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_SimplifiedChinese))
                {
                    content.Append($" ss=\"{SecurityElement.Escape(sport.NameSimplifiedChinese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Indonesia))
                {
                    content.Append($" sb=\"{SecurityElement.Escape(sport.NameIndonesia)}\"");
                }

                if (userProfile.Languages.Contains(Language_Thai))
                {
                    content.Append($" sa=\"{SecurityElement.Escape(sport.NameThai)}\"");
                }

                if (userProfile.Languages.Contains(Language_Korean))
                {
                    content.Append($" sk=\"{SecurityElement.Escape(sport.NameKorean)}\"");
                }

                if (userProfile.Languages.Contains(Language_Vietnamese))
                {
                    content.Append($" sv=\"{SecurityElement.Escape(sport.NameVietnamese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Japanese))
                {
                    content.Append($" sj=\"{SecurityElement.Escape(sport.NameJapanese)}\"");
                }

                if (userProfile.Languages.Contains(Language_Hindi))
                {
                    content.Append($" sh=\"{SecurityElement.Escape(sport.NameHindi)}\"");
                }

                content.Append("/>");
            }

            content.Append("</sports>");

            return content.ToString();
        }
    }
}

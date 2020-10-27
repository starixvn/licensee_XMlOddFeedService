namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Security;
    using System.Text;
    using Common.DTO;
    using Common.Helpers;
    using Criteria;
    using Repositories;

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

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_English),
                    $" te=\"{SecurityElement.Escape(bettype.NameEnglish)}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_TraditionalChinese),
                    $" tt=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameTraditionalChinese, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_SimplifiedChinese),
                    $" ts=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameSimplifiedChinese, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Indonesia),
                    $" tb=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameIndonesia, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Thai),
                    $" ta=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameThai, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Korean),
                    $" tk=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameKorean, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Vietnamese),
                    $" tv=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameVietnamese, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Japanese),
                    $" tj=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameJapanese, bettype.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Hindi),
                    $" th=\"{SecurityElement.Escape(StringHelper.SelectName(bettype.NameHindi, bettype.NameEnglish))}\"");

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

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_English),
                    $" ge=\"{SecurityElement.Escape(league.NameEnglish)}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_TraditionalChinese),
                    $" gt=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameTraditionalChinese, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_SimplifiedChinese),
                    $" gs=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameSimplifiedChinese, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Indonesia),
                    $" gb=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameIndonesia, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Thai),
                    $" ga=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameThai, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Korean),
                    $" gk=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameKorean, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Vietnamese),
                    $" gv=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameVietnamese, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Japanese),
                    $" gj=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameJapanese, league.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Hindi),
                    $" gh=\"{SecurityElement.Escape(StringHelper.SelectName(league.NameHindi, league.NameEnglish))}\"");

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

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_English),
                    $" se=\"{SecurityElement.Escape(sport.NameEnglish)}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_TraditionalChinese),
                    $" st=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameTraditionalChinese, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_SimplifiedChinese),
                    $" ss=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameSimplifiedChinese, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Indonesia),
                    $" sb=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameIndonesia, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Thai),
                    $" sa=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameThai, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Korean),
                    $" sk=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameKorean, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Vietnamese),
                    $" sv=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameVietnamese, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Japanese),
                    $" sj=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameJapanese, sport.NameEnglish))}\"");

                BuilderWriteContent(
                    content,
                    userProfile.Languages.Contains(Language_Hindi),
                    $" sh=\"{SecurityElement.Escape(StringHelper.SelectName(sport.NameHindi, sport.NameEnglish))}\"");

                content.Append("/>");
            }

            content.Append("</sports>");

            return content.ToString();
        }

        private static void BuilderWriteContent(StringBuilder builder, bool condition, string value)
        {
            if (condition)
            {
                builder.Append(value);
            }
        }
    }
}
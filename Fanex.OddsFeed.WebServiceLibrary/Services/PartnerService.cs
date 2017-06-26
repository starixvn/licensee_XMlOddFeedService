namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Security;
    using System.Text;
    using Common.DTO;
    using Criteria;
    using Repositories;

    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _repository;
        private readonly IBetTypeService _betTypeService;

        public PartnerService()
        {
            _repository = new PartnerRepository();
            _betTypeService = new BetTypeService();
        }

        public string GetBetTypesXmlContent(string username, UserProfile userProfile)
        {
            var bettypes = _repository.GetPartnerBetTypes(new GetPartnerBetTypeCriteria { Username = username });
            var enableEnglish = userProfile.Languages.Contains("en-US");
            var enableTraditionalChinese = userProfile.Languages.Contains("zh-TW");
            var enableSimplifiedChinese = userProfile.Languages.Contains("zh-CN");
            var content = new StringBuilder("<bettypes>");

            foreach (var bettype in bettypes)
            {
                content.Append($"<type id=\"{bettype.Id}\" t=\"{_betTypeService.GetBetTypeNameById(bettype.Id)}\"");

                if (enableEnglish)
                {
                    content.Append($" te=\"{SecurityElement.Escape(bettype.NameEnglish)}\"");
                }

                if (enableTraditionalChinese)
                {
                    var traditionalChinese = string.IsNullOrEmpty(bettype.NameTraditionalChinese)
                        ? bettype.NameEnglish
                        : bettype.NameTraditionalChinese;
                    content.Append($" tt=\"{SecurityElement.Escape(traditionalChinese)}\"");
                }

                if (enableSimplifiedChinese)
                {
                    var simplifiedChinese = string.IsNullOrEmpty(bettype.NameSimplifiedChinese)
                        ? bettype.NameEnglish
                        : bettype.NameSimplifiedChinese;
                    content.Append($" ts=\"{SecurityElement.Escape(simplifiedChinese)}\"");
                }

                content.Append($" />");
            }

            content.Append("</bettypes>");

            return content.ToString();
        }

        public string GetLeaguesXmlContent(string username, UserProfile userProfile)
        {
            var leagues = _repository.GetPartnerLeagues(new GetPartnerLeagueCriteria { Username = username });
            var enableEnglish = userProfile.Languages.Contains("en-US");
            var enableTraditionalChinese = userProfile.Languages.Contains("zh-TW");
            var enableSimplifiedChinese = userProfile.Languages.Contains("zh-CN");

            var content = new StringBuilder("<leagues>");

            foreach (var league in leagues)
            {
                content.Append($"<league id=\"{league.LeagueId}\" sid=\"{league.SportId}\" se=\"{SecurityElement.Escape(league.SportName)}\"");

                if (enableEnglish)
                {
                    content.Append($" ge=\"{SecurityElement.Escape(league.NameEnglish)}\"");
                }

                if (enableTraditionalChinese)
                {
                    content.Append($" gt=\"{SecurityElement.Escape(league.NameTraditionalChinese)}\"");
                }

                if (enableSimplifiedChinese)
                {
                    content.Append($" gs=\"{SecurityElement.Escape(league.NameSimplifiedChinese)}\"");
                }

                content.Append(" />");
            }

            content.Append("</leagues>");

            return content.ToString();
        }

        public string GetSportsXmlContent(string username, UserProfile userProfile)
        {
            var sports = _repository.GetPartnerSports(new GetPartnerSportTypeCriteria { Username = username });
            var enableEnglish = userProfile.Languages.Contains("en-US");
            var enableTraditionalChinese = userProfile.Languages.Contains("zh-TW");
            var enableSimplifiedChinese = userProfile.Languages.Contains("zh-CN");
            var content = new StringBuilder("<sports>");

            foreach (var sport in sports)
            {
                content.Append($"<sport id=\"{sport.Id}\"");

                if (enableEnglish)
                {
                    content.Append($" se=\"{SecurityElement.Escape(sport.NameEnglish)}\"");
                }

                if (enableTraditionalChinese)
                {
                    content.Append($" st=\"{SecurityElement.Escape(sport.NameTraditionalChinese)}\"");
                }

                if (enableSimplifiedChinese)
                {
                    content.Append($" ss=\"{SecurityElement.Escape(sport.NameSimplifiedChinese)}\"");
                }

                content.Append("/>");
            }

            content.Append("</sports>");

            return content.ToString();
        }
    }
}

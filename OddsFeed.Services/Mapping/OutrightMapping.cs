namespace OddsFeed.Services.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Outright;

    public class OutrightMapping : IOutrightMapping
    {
        public List<Outright> GetOutrights(IEnumerable<OutrightOddsData> odds)
        {
            var outrights = odds.AsParallel().GroupBy(x => x.LeagueID).Select(x =>
            {
                var row = x.First();
                return new Outright
                {
                    EventId = x.Key,
                    EventEn = row.LeagueName,
                    EventTw = row.LeagueNameC_UNI,
                    EventCn = row.LeagueNameCN,
                    EventIndonesia = row.LeagueNameId,
                    EventThai = row.LeagueNameTh,
                    EventKorean = row.LeagueNameKo,
                    EventVietnamese = row.LeagueNameVn,
                    EventJapanese = row.LeagueNameJp,
                    EventHindi = row.LeagueNameHi,
                    SportId = row.SportID,
                    SportName = row.LeagueNameTC,
                    CountryCode = row.CountryCode,
                    Time = row.EventDate,
                    OutrightOddsList = new List<OutrightOdds>(x.Select(i => GetEventOdds(i)))
                };
            });

            return outrights.ToList();
        }

        private static OutrightOdds GetEventOdds(OutrightOddsData row)
        {
            return new OutrightOdds
            {
                OddsId = row.OrID,
                OddNameEn = row.TeamName,
                OddNameTw = row.TeamNameC_UNI,
                OddNameCn = row.TeamNameCN,
                OddNameIndonesia = row.TeamNameId,
                OddNameThai = row.TeamNameTh,
                OddNameKorean = row.TeamNameKo,
                OddNameVietnamese = row.TeamNameVn,
                OddNameJapanese = row.TeamNameJp,
                OddNameHindi = row.TeamNameHi,
                Value = row.Odds
            };
        }
    }
}
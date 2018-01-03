namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using Common.DTO;
    using System;
    using System.Data;
    using System.Linq;

    public class OutrightMapping : IOutrightMapping
    {

        public SafeList<Outright> GetOutrights(DataTable dataTable)
        {
            var events = new SafeList<Outright>();
            System.Collections.Concurrent.Partitioner<DataRow> partionerList = System.Collections.Concurrent.Partitioner.Create(dataTable.AsEnumerable().ToList(), true);

            var tempRows = from row in partionerList.AsParallel()
                           select row;

            tempRows.ForAll((row) =>
            {
                var eventOdds = GetEventOdds(row);

                if (eventOdds != null)
                {
                    AddOutrightOdds(row, events, eventOdds);
                }
            });


            return events;
        }

        private static void AddOutrightOdds(DataRow row, SafeList<Outright> outrights, OutrightOdds outrightOdds)
        {
            var currentLeagueId = Convert.ToInt32(row["LeagueID"]);
            var existedOutright = outrights.FirstOrDefault(o => o.EventId == currentLeagueId);

            if (existedOutright != null)
            {
                existedOutright.OutrightOddsList.Add(outrightOdds);
            }
            else
            {
                outrights.Add(GetOutright(row, currentLeagueId, outrightOdds));
            }

        }

        private static Outright GetOutright(DataRow row, int leagueId, OutrightOdds eventOdds)
        {
            return new Outright
            {
                EventId = leagueId,
                EventEn = row["LeagueName"].ToString(),
                EventTw = row["LeagueNameC_UNI"].ToString(),
                EventCn = row["LeagueNameCN"].ToString(),
                EventIndonesia = row["leaguenameId"].ToString(),
                EventThai = row["leaguenameTh"].ToString(),
                EventKorean = row["leaguenameKo"].ToString(),
                EventVietnamese = row["leaguenameVn"].ToString(),
                EventJapanese = row["leaguenameJp"].ToString(),
                EventHindi = row["leaguenameHd"].ToString(),
                SportId = Convert.ToInt32(row["SportID"]),
                SportName = row["LeagueNameTC"].ToString(),
                CountryCode = row["CountryCode"].ToString(),
                Time = Convert.ToDateTime(row["EventDate"].ToString()),
                OutrightOddsList = new SafeList<OutrightOdds> { eventOdds }
            };
        }

        private static OutrightOdds GetEventOdds(DataRow row)
        {
            return new OutrightOdds
            {
                OddsId = Convert.ToInt32(row["OrID"]),
                OddNameEn = row["TeamName"].ToString(),
                OddNameTw = row["TeamNameC_UNI"].ToString(),
                OddNameCn = row["TeamNameCN"].ToString(),
                OddNameIndonesia = row["teamnameId"].ToString(),
                OddNameThai = row["teamnameTh"].ToString(),
                OddNameKorean = row["teamnameKo"].ToString(),
                OddNameVietnamese = row["teamnameVn"].ToString(),
                OddNameJapanese = row["teamnameJp"].ToString(),
                OddNameHindi = row["teamnameHd"].ToString(),
                Value = Convert.ToDecimal(row["Odds"])
            };
        }
    }
}

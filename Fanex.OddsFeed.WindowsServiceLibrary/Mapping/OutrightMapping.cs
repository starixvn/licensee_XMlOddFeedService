namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using Common.DTO;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class OutrightMapping : IOutrightMapping
    {

        public List<Outright> GetOutrights(DataTable dataTable)
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
                    events.Add(GetOutright(row, eventOdds));
                }
            });

            return events.GroupBy(e => e.EventId).Select(g =>
            {
                var odds = new List<OutrightOdds>(g.SelectMany(e => e.OutrightOddsList));
                var outright = g.First();
                outright.OutrightOddsList.Clear();
                outright.OutrightOddsList.AddRange(odds);

                return outright;
            }).ToList();
        }

        private static Outright GetOutright(DataRow row, OutrightOdds eventOdds)
        {
            return new Outright
            {
                EventId = Convert.ToInt32(row["LeagueID"]),
                EventEn = row["LeagueName"].ToString(),
                EventTw = row["LeagueNameC_UNI"].ToString(),
                EventCn = row["LeagueNameCN"].ToString(),
                EventIndonesia = row["leaguenameId"].ToString(),
                EventThai = row["leaguenameTh"].ToString(),
                EventKorean = row["leaguenameKo"].ToString(),
                EventVietnamese = row["leaguenameVn"].ToString(),
                EventJapanese = row["leaguenameJp"].ToString(),
                EventHindi = row["leaguenameHi"].ToString(),
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
                OddNameHindi = row["teamnameHi"].ToString(),
                Value = Convert.ToDecimal(row["Odds"])
            };
        }
    }
}

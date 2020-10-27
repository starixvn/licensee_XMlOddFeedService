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
            System.Collections.Concurrent.Partitioner<DataRow> partionerList = System.Collections.Concurrent.Partitioner.Create(dataTable.AsEnumerable().ToList(), true);

            var outrights = from row in partionerList.AsParallel()
                            group row by row["LeagueID"].ToString() into g
                            select new Outright
                            {
                                EventId = Convert.ToInt32(g.Key),
                                EventEn = g.FirstOrDefault()?["LeagueName"].ToString(),
                                EventTw = g.FirstOrDefault()?["LeagueNameC_UNI"].ToString(),
                                EventCn = g.FirstOrDefault()?["LeagueNameCN"].ToString(),
                                EventIndonesia = g.FirstOrDefault()?["leaguenameId"].ToString(),
                                EventThai = g.FirstOrDefault()?["leaguenameTh"].ToString(),
                                EventKorean = g.FirstOrDefault()?["leaguenameKo"].ToString(),
                                EventVietnamese = g.FirstOrDefault()?["leaguenameVn"].ToString(),
                                EventJapanese = g.FirstOrDefault()?["leaguenameJp"].ToString(),
                                EventHindi = g.FirstOrDefault()?["leaguenameHi"].ToString(),
                                SportId = Convert.ToInt32(g.FirstOrDefault()?["SportID"]),
                                SportName = g.FirstOrDefault()?["LeagueNameTC"].ToString(),
                                CountryCode = g.FirstOrDefault()?["CountryCode"].ToString(),
                                Time = Convert.ToDateTime(g.FirstOrDefault()?["EventDate"].ToString()),
                                OutrightOddsList = (new SafeList<OutrightOdds>(g.Select(i => GetEventOdds(i))))
                            };

            return outrights.ToList();
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

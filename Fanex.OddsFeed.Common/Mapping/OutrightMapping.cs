namespace Fanex.OddsFeed.Common.Mapping
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
                                EventEn = GetEventEn(g),
                                EventTw = GetEventTw(g),
                                EventCn = GetEventCn(g),
                                EventIndonesia = GetEventIndonesia(g),
                                EventThai = GetEventThai(g),
                                EventKorean = GetEventKorean(g),
                                EventVietnamese = GetEventVietnamese(g),
                                EventJapanese = GetEventJapanese(g),
                                EventHindi = GetEventHindi(g),
                                SportId = Convert.ToInt32(g.FirstOrDefault()?["SportID"]),
                                SportName = g.FirstOrDefault()?["LeagueNameTC"].ToString(),
                                CountryCode = g.FirstOrDefault()?["CountryCode"].ToString(),
                                Time = Convert.ToDateTime(g.FirstOrDefault()?["EventDate"].ToString()),
                                OutrightOddsList = (new SafeList<OutrightOdds>(g.Select(i => GetEventOdds(i))))
                            };

            return outrights.ToList();
        }

        private static string GetEventEn(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueName"].ToString();
        }

        private static string GetEventTw(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueNameC_UNI"].ToString();
        }

        private static string GetEventCn(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["LeagueNameCN"].ToString();
        }

        private static string GetEventIndonesia(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameId"].ToString();
        }

        private static string GetEventThai(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameTh"].ToString();
        }

        private static string GetEventKorean(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameKo"].ToString();
        }

        private static string GetEventVietnamese(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameVn"].ToString();
        }

        private static string GetEventJapanese(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameJp"].ToString();
        }

        private static string GetEventHindi(IGrouping<string, DataRow> dataRow)
        {
            return dataRow.FirstOrDefault()?["leaguenameHi"].ToString();
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

namespace ExcelToJson
{
    public class Item
    {
        public string BettypeName { get; set; }
        public int BettypeId { get; set; }
        public string Tag { get; set; }
        public string MatchAttribute { get; set; }

        public string ExtraAttribute { get; set; }

        public bool HasExtraHandicap
        {
            get
            {
                return ExtraAttribute.Contains("Handicap value");
            }
        }

        public bool HasExtraOverUnder
        {
            get
            {
                return ExtraAttribute.Contains("Over Under value");
            }
        }

        public string DataAttribute
        {
            get
            {
                return DBColumns.Replace("OddsES.", "");
            }
        }
        public string BetChoice { get; set; }
        public string Description { get; set; }

        public string DBColumns { get; set; }

        public bool IsResource
        {
            get
            {
                return !IsResourceX && !IsResourceY && DBColumns.Contains("ResourceID");
            }
        }

        public bool IsResourceX
        {
            get
            {
                return DBColumns.Contains("ResourceID.XXyy")
                    || (DBColumns.Contains("ResourceID.x") && !DBColumns.Contains("ResourceID.xxYY"))
                    || (DBColumns.Contains("ResourceID.xx") && !DBColumns.Contains("ResourceID.xxYY"));
            }
        }

        public bool IsResourceY
        {
            get
            {
                return DBColumns.Contains("ResourceID.xxYY")
                    || DBColumns.Contains("ResourceID.yy");
            }
        }
    }
}

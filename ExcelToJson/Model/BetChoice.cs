namespace ExcelToJson
{
    public class BetChoice
    {
        public string MatchAttribute { get; set; }
        public string DataAttribute { get; set; }
        public bool IsResource { get; set; }
        public bool IsResourceX { get; set; }
        public bool IsResourceY { get; set; }

        public bool ShouldSerializeIsResource()
        {
            return IsResource;
        }

        public bool ShouldSerializeIsResourceX()
        {
            return IsResourceX;
        }

        public bool ShouldSerializeIsResourceY()
        {
            return IsResourceY;
        }

        public bool NotConvert { get; set; }

        public bool ShouldSerializeNotConvert()
        {
            return NotConvert;
        }

        public bool ShouldSerializeDataAttribute()
        {
            return DataAttribute != string.Empty;
        }

    }
}

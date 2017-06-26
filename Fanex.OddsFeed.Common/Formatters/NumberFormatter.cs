namespace Fanex.OddsFeed.Common.Formatter
{
    public static class NumberFormatter
    {
        private const string OddsPattern = "F";
        private const string TotalPattern = "#.##";

        public static string FormatOdds(object value)
        {
            decimal result = 0;
            
            if (!decimal.TryParse(value.ToString(), out result))
            {
                result = 0;
            }

            return result.ToString(OddsPattern);
        }

    }
}

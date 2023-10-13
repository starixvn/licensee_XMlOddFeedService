namespace Fanex.OddsFeed.Core.Extensions
{
    public static class OddsConverter
    {
        public static decimal ToHkOdds(decimal malayOdds)
        {
            return malayOdds >= 0
                       ? malayOdds
                       : Truncate(1 / Math.Abs(malayOdds));
        }

        public static decimal ToDecimalOddsFromHkOdds(decimal hongKongOdds)
        {
            return hongKongOdds + 1;
        }

        public static decimal ToDecimalOddsFromMalayOdds(decimal malayOdds)
        {
            if (malayOdds == 0 || malayOdds > 1)
            {
                return malayOdds;
            }

            return ToDecimalOddsFromHkOdds(ToHkOdds(malayOdds));
        }

        public static float ToIndoOdds(int malayOdds)
        {
            return malayOdds > 0 ? -1 / Math.Abs(malayOdds) : 1 / Math.Abs(malayOdds);
        }

        public static decimal ConvertOdds(int oddsRegion, decimal decimalOdds)
        {
            const int malayOdds = 1;
            const int hkOdds = 2;
            const int truncateOdds = 3;
            const int indoOdds = 4;
            const int usOdds = 5;
            if (decimalOdds == 0)
            {
                return decimalOdds;
            }

            switch (oddsRegion)
            {
                case malayOdds:
                    return DecimalToMalay(decimalOdds);

                case hkOdds:
                    return DecimalToHK(decimalOdds);

                case truncateOdds:
                    return Truncate(decimalOdds);

                case indoOdds:
                    return DecimalToIndo(decimalOdds);

                case usOdds:
                    return DecimalToUS(decimalOdds);

                default:
                    return Truncate(decimalOdds);
            }
        }

        public static decimal DecimalToUS(decimal decimalOdds)
        {
            const int maxOdds = 2;
            const int hundred = 100;
            if (decimalOdds <= 1)
            {
                return Truncate(0);
            }

            return decimalOdds < maxOdds
                ? Truncate(-hundred / (decimalOdds - 1))
                : Truncate((decimalOdds - 1) * hundred);
        }

        public static decimal DecimalToHK(decimal decimalOdds)
        {
            var hongKongOdds = decimalOdds <= 1 ? 0 : decimalOdds - 1;

            return Truncate(hongKongOdds);
        }

        public static decimal DecimalToIndo(decimal decimalOdds)
        {
            const int maxOdds = 2;
            if (decimalOdds <= 1)
            {
                return Truncate(0);
            }

            return decimalOdds >= maxOdds
                ? Truncate(decimalOdds - 1)
                : Truncate(1 / (1 - decimalOdds));
        }

        public static decimal DecimalToMalay(decimal dec)
        {
            const int maxOdds = 2;
            if (dec <= 1)
            {
                return Truncate(0);
            }

            return dec <= maxOdds ? Truncate(dec - 1) : Truncate(1 / (1 - dec));
        }

        public static decimal Truncate(decimal value, int decimalPlaces = 2)
        {
            var adjustment = (decimal)Math.Pow(10, decimalPlaces);

            return Math.Truncate(value * adjustment) / adjustment;
        }
    }
}
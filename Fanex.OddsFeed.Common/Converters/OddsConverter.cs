namespace Fanex.OddsFeed.Common.Converters
{
    using System;

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
            if (decimalOdds == 0)
            {
                return decimalOdds;
            }

            switch (oddsRegion)
            {
                case 1:
                    return DecimalToMalay(decimalOdds); 
                case 2:
                    return DecimalToHK(decimalOdds);
                case 3:
                    return Truncate(decimalOdds);
                case 4:
                    return DecimalToIndo(decimalOdds);
                case 5:
                    return DecimalToUS(decimalOdds);
                default:
                    return Truncate(decimalOdds);
            }
        }

        public static decimal DecimalToUS(decimal decimalOdds)
        {
            if (decimalOdds <= 1)
            {
                return Truncate(0);
            }
            else if (decimalOdds < 2)
            {
                return Truncate(-100 / (decimalOdds - 1));
            }
            else
            {
                return Truncate((decimalOdds - 1) * 100);
            }
        }

        public static decimal DecimalToHK(decimal decimalOdds)
        {
            var hongKongOdds = decimalOdds <= 1 ? 0 : decimalOdds - 1;

            return Truncate(hongKongOdds);
        }

        public static decimal DecimalToIndo(decimal decimalOdds)
        {
            if (decimalOdds <= 1)
            {
                return Truncate(0);
            }
            else if (decimalOdds >= 2)
            {
                return Truncate(decimalOdds - 1);
            }
            else
            {
                return Truncate(1 / (1 - decimalOdds));
            }
        }

        public static decimal DecimalToMalay(decimal dec)
        {
            if (dec <= 1)
            {
                return Truncate(0);
            }
            else if (dec <= 2)
            {
                return Truncate(dec - 1);
            }
            else
            {
                return Truncate(1 / (1 - dec));
            }
        }

        public static decimal Truncate(decimal value, int decimalPlaces = 2)
        {
            var adjustment = (decimal)Math.Pow(10, decimalPlaces);

            return Math.Truncate(value * adjustment) / adjustment;
        }
    }
}

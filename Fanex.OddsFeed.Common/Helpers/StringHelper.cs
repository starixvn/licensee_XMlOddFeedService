namespace Fanex.OddsFeed.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StringHelper
    {
        public static bool ContainInList(IList<string> list, string value)
        {
            return list.Any(keyword => Contain(value, keyword));
        }

        public static bool Contain(string value, string keyword)
        {
            return value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static string SelectName(string nameInOtherLanguage, string nameInEnglish)
        {
            return string.IsNullOrEmpty(nameInOtherLanguage) ? nameInEnglish : nameInOtherLanguage;
        }

        public static bool Equals(string value1, string value2)
            => !string.IsNullOrWhiteSpace(value1) && value1.Equals(value2, StringComparison.CurrentCultureIgnoreCase);
    }
}
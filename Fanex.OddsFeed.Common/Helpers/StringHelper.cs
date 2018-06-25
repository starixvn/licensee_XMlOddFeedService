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
    }
}

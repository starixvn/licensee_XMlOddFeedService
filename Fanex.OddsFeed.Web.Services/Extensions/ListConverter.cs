namespace Fanex.OddsFeed.Web.Services.Extensions
{
    public static class ListConverter
    {
        public static IList<int> ConvertToListInt(string stringArray)
        {
            var items = stringArray.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return items.Select(item => Convert.ToInt32(item)).ToList();
        }

        public static IList<string> ConvertToListString(string stringArray, string separator)
            => stringArray.Split(new[] { separator }, StringSplitOptions.None).ToList();

        public static IList<string> ConvertToListString(string stringArray, bool isTrim = false)
        {
            var items = stringArray.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return isTrim ? items.Select(item => item.Trim()).ToList() : items.ToList();
        }
    }
}
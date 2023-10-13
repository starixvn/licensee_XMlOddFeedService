namespace Fanex.OddsFeed.Core.Extensions
{
    using Newtonsoft.Json;

    public static class ReadJsonFromFile
    {
        public static IList<T> GetBetTypeSchemas<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    var temp = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(temp))
                    {
                        return JsonConvert.DeserializeObject<List<T>>(temp);
                    }

                    return new List<T>();
                }
            }

            return new List<T>();
        }
    }
}
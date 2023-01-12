namespace Fanex.OddsFeed.Core.Extensions
{
    using Fanex.Logging;
    using Newtonsoft.Json;

    public class ReadJsonFromFile

    {
        public static IList<T> GetBetTypeSchemas<T>(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

                using (var reader = new StreamReader(fullPath))
                {
                    var temp = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(temp))
                    {
                        return JsonConvert.DeserializeObject<List<T>>(temp);
                    }
                    else
                    {
                        LogService.Info("BettypeSchema configuration is empty");
                    }
                }
            }
            catch (Exception exception)
            {
                var message = $"Read BetType.json unsuccessful - Message: {exception.Message}";

                LogService.Error(message);
            }

            return new List<T>();
        }
    }
}
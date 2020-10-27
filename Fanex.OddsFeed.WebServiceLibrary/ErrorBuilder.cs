namespace Fanex.OddsFeed.WebServiceLibrary
{
    public static class ErrorBuilder
    {
        public static string GetUnAuthenticated() => $"<Error id=\"1\" message=\"You are not authorized to take this action\" />";

        public static string GetParameterSufficent() => $"<Error id=\"2\" message=\"Missing parameter!\" />";

        public static string GetUMDownTime(string time) =>
            $"<Error id=\"4\" message=\"We are having server maintenance, website will be unavailable until {time} (HK Time GMT +8). Sorry for the inconveniences caused!\" />";

        public static string GetOverLimitedOptions() => $"<Error id=\"10024\" message=\"You can only filter maximum 5 options. Please try again.\" />";

        public static string GetException(string exceptionMessage) => $"<Error id=\"5\" message=\"{exceptionMessage}\" />";

        public static string GetError(int errorCode, string errorMessage)
        {
            return errorCode != 0 ? $"<Error id=\"{errorCode}\" message=\"{errorMessage}\" />" : string.Empty;
        }
    }
}
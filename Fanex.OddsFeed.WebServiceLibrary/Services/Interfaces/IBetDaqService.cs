namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System.Collections;

    public interface IBetDaqService
    {
        string GetSchedule(string filter);

        string GetOddWithUserIsBetdaq(string filter);

        int InsertBetDaq(Hashtable spParams, bool isDownTime);
    }
}

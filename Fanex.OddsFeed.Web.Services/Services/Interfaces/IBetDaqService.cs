namespace Fanex.OddsFeed.Web.Services.Services.Interfaces
{
    using System.Collections;

    public interface IBetDaqService
    {
        Task<string> GetSchedule(string filter);

        Task<string> GetOddWithUserIsBetdaq(string filter);

        Task<int> InsertBetDaq(Hashtable spParams, bool isDownTime);
    }
}
namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Collections;
    using System.Threading.Tasks;

    public interface IBetDaqService
    {
        Task<string> GetSchedule(string filter);

        Task<string> GetOddWithUserIsBetdaq(string filter);

        Task<int> InsertBetDaq(Hashtable spParams, bool isDownTime);
    }
}
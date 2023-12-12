namespace OddsFeed.WebService.Services.Interfaces
{
    using System.Threading.Tasks;
    using OddsFeed.WebService.Models.Betdaq;

    public interface IBetDaqService
    {
        Task<string> GetScheduleAsync(string filter);

        Task<string> GetOddWithUserIsBetdaqAsync(string filter);

        Task<int> InsertBetDaqAsync(InsertBetDaqParam param, bool isDownTime);
    }
}
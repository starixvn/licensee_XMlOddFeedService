namespace Fanex.OddsFeed.Core.Models
{
    using Fanex.Data.Repository;

    public abstract class BaseCommandNonQuery : NonQueryCommand
    {
        public int ErrCode { get; set; }

        public string? ErrMsg { get; set; }
    }
}
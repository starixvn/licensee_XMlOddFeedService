namespace Fanex.OddsFeed.WebServiceLibrary.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Fanex.Data.Repository;

    public class BaseRepository
    {
        protected BaseRepository()
        {
        }

        protected const string ErrorCode = "errcode";
        protected const string ErrorMsg = "errmsg";
        protected const string Language = "language";
        protected const string TotalRecords = "totalrecords";
        protected const string Criteria = "criteria";
        protected const int InvalidCriteriaCode = -2;

        protected virtual IEnumerable<TResult> GetListOf<TResult>(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<TResult>(criteria.GetSpParams());

                return result ?? Enumerable.Empty<TResult>();
            }
        }
    }
}
namespace Fanex.OddsFeed.Common.Mapping
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.OddsFeed.Common.DTO;

    public interface IOutrightMapping
    {
        List<Outright> GetOutrights(DataTable dataTable);
    }
}
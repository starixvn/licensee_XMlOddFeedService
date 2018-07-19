namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using Common.DTO;
    using System.Collections.Generic;
    using System.Data;

    public interface IOutrightMapping
    {
        List<Outright> GetOutrights(DataTable dataTable);
    }
}

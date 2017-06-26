namespace Fanex.OddsFeed.WindowsServiceLibrary.Mapping
{
    using System.Collections.Generic;
    using System.Data;
    using Common.DTO;

    public interface IOutrightMapping
    {
        SafeList<Outright> GetOutrights(DataTable dataTable);
    }
}

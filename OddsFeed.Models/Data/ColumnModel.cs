namespace OddsFeed.Models.Data
{
    using System.Collections.Generic;

    public class ColumnModel<T>
    {
        public IEnumerable<T> ColumnOne { get; set; }

        public IEnumerable<T> ColumnTwo { get; set; }

        public IEnumerable<T> ColumnThree { get; set; }
    }
}
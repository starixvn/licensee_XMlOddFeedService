namespace Fanex.OddsFeed.Models.Data
{
    public class ColumnModel<T>
    {
        public IEnumerable<T> ColumnOne { get; set; }

        public IEnumerable<T> ColumnTwo { get; set; }

        public IEnumerable<T> ColumnThree { get; set; }
    }
}
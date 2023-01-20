namespace Fanex.OddsFeed.Models.DTO
{
    public class Ip
    {
        public int IpKeyFrom { get; set; }

        public int IpKeyTo { get; set; }

        public long IpFrom { get; set; }

        public long IpTo { get; set; }

        public string CountryShort { get; set; }

        public string CountryLong { get; set; }

        public string IpRegion { get; set; }

        public string IpCity { get; set; }

        public bool IsModified { get; set; }

        public bool IsException { get; set; }
    }
}

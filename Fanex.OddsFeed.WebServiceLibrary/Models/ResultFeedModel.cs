namespace Fanex.OddsFeed.WebServiceLibrary.Models
{
    using System;
    using Fanex.OddsFeed.Common.DTO;

    public class ResultFeedModel
    {
        public UserProfile UserProfile { get; set; }

        public DateTime Date { get; set; }

        public string BookieName { get; set; }

        public int Timestamp { get; set; }

        public string Username { get; set; }
    }
}
namespace OddsFeed.Models.DTO
{
    public class AuthenticatedPartner
    {
        public string SystemName { get; set; }

        public UserProfile UserProfile { get; set; }

        public string Error { get; set; }
    }
}
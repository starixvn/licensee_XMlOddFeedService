namespace Fanex.OddsFeed.Web.Services.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Models.DTO;

    public interface IPartnerRepository
    {
        Task<AuthenticatedPartner> GetAuthenticatedPartner(ICriteria criteria);
    }
}
using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        IEnumerable<AuctionDTO> GetAllAuctions();
        AuctionDTO GetAuction(Guid id);
        void AddAuction(AuctionDTO auction);
        void UpdateAuction(AuctionDTO auction);
        void DeleteAuction(Guid id);
        IEnumerable<AuctionDTO> GetAuctionsByCategory(int categoryId); // New method for retrieving auctions by category

    }
}
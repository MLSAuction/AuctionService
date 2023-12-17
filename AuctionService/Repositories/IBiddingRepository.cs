using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface IBiddingRepository
    {
        BiddingDTO GetBid(Guid id);
        void AddBid(BiddingDTO bidding);
        BiddingDTO GetHighestBidForAuction(Guid auctionId);
        IEnumerable<BiddingDTO> GetAllBidsForAuction(Guid auctionId);
    }
}

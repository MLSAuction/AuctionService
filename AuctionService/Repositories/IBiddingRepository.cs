using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface IBiddingRepository
    {
        BiddingDTO GetBid(int id);
        void AddBid(BiddingDTO bidding);
        BiddingDTO GetHighestBidForAuction(int auctionId);
        IEnumerable<BiddingDTO> GetAllBidsForAuction(int auctionId);
    }
}

using Microsoft.AspNetCore.Mvc.Formatters;
using MongoDB.Driver;
using AuctionService.Models;
using AuctionService.Repositories.DBContext;


namespace AuctionService.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        private readonly ILogger<BiddingRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<BiddingDTO> _db;

        public BiddingRepository(ILogger<BiddingRepository> logger, IConfiguration configuration, MongoDBContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db.GetCollection<BiddingDTO>("Bids");

        }

        public BiddingDTO GetBid(int id)
        {
            return _db.Find(u => u.BidId == id).FirstOrDefault();
        }

        public void AddBid(BiddingDTO bidding) //replace this with adding to the queue
        {
            _db.InsertOne(bidding);
        }

        public BiddingDTO GetHighestBidForAuction(int auctionId)
        {
            return _db.Find(b => b.AuctionId == auctionId)
                      .SortByDescending(b => b.Price)
                      .ThenBy(b => b.TimePlaced)
                      .FirstOrDefault();
        }

        public IEnumerable<BiddingDTO> GetAllBidsForAuction(int auctionId)
        {
            return _db.Find(b => b.AuctionId == auctionId)
                      .SortByDescending(b => b.Price)
                      .ThenBy(b => b.TimePlaced)
                      .ToList();
        }
    }
}

using Microsoft.AspNetCore.Mvc.Formatters;
using MongoDB.Driver;
using AuctionService.Models;
using AuctionService.Repositories.DBContext;


namespace AuctionService.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly ILogger<AuctionRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<AuctionDTO> _db;

        public AuctionRepository(ILogger<AuctionRepository> logger, IConfiguration configuration, MongoDBContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db.GetCollection<AuctionDTO>("Auctions"); //Fortæller at vores added-informationer(fx. nye auctions) kommer inde under Collection "Auctions" på Mongo

        }

        public IEnumerable<AuctionDTO> GetAllAuctions()
        {
            return _db.Find(_ => true).ToList();
        }

        public AuctionDTO GetAuction(Guid id)
        {
            // Use MongoDB's LINQ methods to query for a auction by ID
            return _db.Find(u => u.AuctionId == id).FirstOrDefault();
        }

        public void AddAuction(AuctionDTO auction)
        {
            // Insert a new auction document into the collection
            _db.InsertOne(auction);
        }

        public void UpdateAuction(AuctionDTO auction)
        {
            // Update an existing auction document based on their ID
            var filter = Builders<AuctionDTO>.Filter.Eq(u => u.AuctionId, auction.AuctionId);
            _db.ReplaceOne(filter, auction);
        }

        public void DeleteAuction(Guid id)
        {
            // Delete a auction document by ID
            var filter = Builders<AuctionDTO>.Filter.Eq(u => u.AuctionId, id);
            _db.DeleteOne(filter);
        }

        public IEnumerable<AuctionDTO> GetAuctionsByCategory(int categoryId)
        {
            return _db.Find(a => a.CategoryId == categoryId).ToList();
        }



    }
}

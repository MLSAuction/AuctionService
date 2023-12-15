using Microsoft.AspNetCore.Mvc.Formatters;
using MongoDB.Driver;
using AuctionService.Models;
using AuctionService.Repositories.DBContext;
using System.IO;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using VaultSharp.V1.Commons;

namespace AuctionService.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        private readonly ILogger<BiddingRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<BiddingDTO> _db;
        private readonly Secret<SecretData> _secret;

        public BiddingRepository(ILogger<BiddingRepository> logger, IConfiguration configuration, MongoDBContext db, Secret<SecretData> secret)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db.GetCollection<BiddingDTO>("Bids");
            _secret = secret;
        }

        public BiddingDTO GetBid(Guid id)
        {
            return _db.Find(u => u.BidId == id)
                      .FirstOrDefault();
        }

        public void AddBid(BiddingDTO bid)
        {
            var factory = new ConnectionFactory { HostName = _secret.Data.Data["MqHost"].ToString() };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "bids",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string bidJson = JsonSerializer.Serialize(bid);
            var body = Encoding.UTF8.GetBytes(bidJson);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "bids",
                                 basicProperties: null,
                                 body: body);
        }

        public BiddingDTO GetHighestBidForAuction(Guid auctionId)
        {
            return _db.Find(b => b.AuctionId == auctionId)
                      .SortByDescending(b => b.Price)
                      .ThenBy(b => b.TimePlaced)
                      .FirstOrDefault();
        }

        public IEnumerable<BiddingDTO> GetAllBidsForAuction(Guid auctionId)
        {
            return _db.Find(b => b.AuctionId == auctionId)
                      .SortByDescending(b => b.Price)
                      .ThenBy(b => b.TimePlaced)
                      .ToList();
        }
    }
}

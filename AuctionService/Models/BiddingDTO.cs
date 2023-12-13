using MongoDB.Bson.Serialization.Attributes;

namespace AuctionService.Models
{
    public class BiddingDTO
    {
        [BsonId]
        public int BidId { get; set; }
        public int UserId {get; set;}
        public int AuctionId {get; set;}
        public int Price { get; set; }
        public DateTime TimePlaced { get; set; }
    }
}

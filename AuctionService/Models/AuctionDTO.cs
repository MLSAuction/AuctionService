using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuctionService.Models
{
    public class AuctionDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // This attribute specifies that the Guid should be stored as a string
        public Guid AuctionId { get; set; }

        [BsonRepresentation(BsonType.String)] // Apply the attribute to UserId
        public Guid? UserId { get; set; }

        [BsonRepresentation(BsonType.String)] // Apply the attribute to CatalogId
        public Guid? CatalogId { get; set; }
        public int CategoryId {get; set;}
        public int MinimumPrice {get; set;}
        public int MinimumPriceInterval {get; set;}
        public int BuyNowPrice {get;set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

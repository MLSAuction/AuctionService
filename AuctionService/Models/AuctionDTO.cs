﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace AuctionService.Models
{
    public class AuctionDTO
    {
        [BsonId]
        public int AuctionId { get; set; }
        public int UserId { get; set; }
        public int CatalogId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

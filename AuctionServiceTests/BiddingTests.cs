using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AuctionService.Controllers;
using AuctionService.Models;
using AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BiddingTests
{
    [TestFixture]
    public class BiddingControllerTests
    {
        private Mock<IAuctionRepository> _auctionRepositoryMock;
        private Mock<IBiddingRepository> _biddingRepositoryMock;
        private AuctionController _biddingController;

        [SetUp]
        public void Setup()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _biddingRepositoryMock = new Mock<IBiddingRepository>();
            var loggerMock = new Mock<ILogger<AuctionController>>();
            var configurationMock = new Mock<IConfiguration>();

            _biddingController = new AuctionController(loggerMock.Object, configurationMock.Object, _auctionRepositoryMock.Object, _biddingRepositoryMock.Object);
        }

        [Test]
        public void GetBid_ValidId_ReturnsOkResult()
        {
            // Arrange
            Guid bidId = Guid.Parse("fccb9563-83a4-4e55-845a-2d65cd27b314");
            var biddingDto = new BiddingDTO { BidId = bidId, UserId = Guid.Parse("ed0d17b8-dbc3-4080-8f4a-abe63b9679b9"), AuctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67"), Price = 100, TimePlaced = DateTime.Now };

            _biddingRepositoryMock.Setup(repo => repo.GetBid(bidId)).Returns(biddingDto);

            // Act
            var result = _biddingController.GetBid(bidId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(biddingDto, result.Value);
        }

    }
}

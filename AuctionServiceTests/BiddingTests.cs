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
        private Mock<IBiddingRepository> _biddingRepositoryMock;
        private BiddingController _biddingController;

        [SetUp]
        public void Setup()
        {
            _biddingRepositoryMock = new Mock<IBiddingRepository>();
            var loggerMock = new Mock<ILogger<BiddingController>>();
            var configurationMock = new Mock<IConfiguration>();

            _biddingController = new BiddingController(loggerMock.Object, configurationMock.Object, _biddingRepositoryMock.Object);
        }

        [Test]
        public void GetBid_ValidId_ReturnsOkResult()
        {
            // Arrange
            var bidId = 1;
            var biddingDto = new BiddingDTO { BidId = bidId, UserId = 1, AuctionId = 1, Price = 100, TimePlaced = DateTime.Now };

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

using NUnit.Framework;
using Moq; // Add this if you are using Moq for mocking
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AuctionService.Controllers;
using AuctionService.Models;
using AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace AuctionServiceTests
{
    [TestFixture]
    public class AuctionControllerTests
    {
        private Mock<IAuctionRepository> _auctionRepositoryMock;
        private AuctionController _auctionController;

        [SetUp]
        public void Setup()
        {
            // Vi opretter en Moq mock for IAuctionRepository
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            var loggerMock = new Mock<ILogger<AuctionController>>();
            var configurationMock = new Mock<IConfiguration>();

            // Initialiser AuctionController med de mockede dependencies.
            _auctionController = new AuctionController(loggerMock.Object, configurationMock.Object, _auctionRepositoryMock.Object);
        }

        [Test]
        public void GetAuction_ValidId_ReturnsOkResult()
        {
            // Arrange -> Definer en bruger med ID 1 og navn "John Doe".
            var auctionId = 1;
            var auctionDto = new AuctionDTO { AuctionId = auctionId};

            // Opsæt mock IAuctionRepository til at returnere brugeren, når GetAuction kaldes med det specificerede ID.
            //For at teste, at denne test virker - Kan man prøve at få den til at fejle, ved at tilføje +1 efter 'auctionId'
            _auctionRepositoryMock.Setup(repo => repo.GetAuction(auctionId)).Returns(auctionDto);

            // ACT -> Udfør handlingen ved at kalde GetAuction-metoden på AuctionController med det specificerede bruger-ID.
            var result = _auctionController.GetAuction(auctionId) as OkObjectResult;

            //Assert -> Bekræfter, at resultatet ikke er null, og at HTTP-statuskoden er 200 (OK).
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Bekræft, at værdien af resultatet er den forventede AuctionDTO..
            Assert.AreEqual(auctionDto, result.Value);
        }

       

    }
}
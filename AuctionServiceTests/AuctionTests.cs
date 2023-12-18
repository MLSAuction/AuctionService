using NUnit.Framework;
using Moq; // Add this if you are using Moq for mocking
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AuctionService.Controllers;
using AuctionService.Models;
using AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace AuctionTests
{
    [TestFixture]
    public class AuctionControllerTests
    {
        private Mock<IAuctionRepository> _auctionRepositoryMock;
        private Mock<IBiddingRepository> _biddingRepositoryMock;
        private AuctionController _auctionController;

        [SetUp]
        public void Setup()
        {
            // Vi opretter en Moq mock for IAuctionRepository
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _biddingRepositoryMock = new Mock<IBiddingRepository>();
            var loggerMock = new Mock<ILogger<AuctionController>>();
            var configurationMock = new Mock<IConfiguration>();

            // Initialiser AuctionController med de mockede dependencies.
            _auctionController = new AuctionController(loggerMock.Object, configurationMock.Object, _auctionRepositoryMock.Object, _biddingRepositoryMock.Object);
        }

        [Test]
        [TestCase("39e30ada-6745-4322-913f-b2e8496a8c67", "77aa0f52-039e-4762-bb7c-0e9bd942b92b", "ea968fbc-6de1-48b3-a3ee-b151920c4dbb", 500, "40e30ada-6745-4322-913f-b2e8496a8c67", "77aa0f52-039e-4762-bb7c-0e9bd942b92b", "ea968fbc-6de1-48b3-a3ee-b151920c4ccc", 1000)]
        public void GetAllActionsReturnsAllAuctions(Guid auctionId, Guid userId, Guid catalogId, int minimumPrice, Guid auctionId2, Guid userId2, Guid catalogId2, int minimumPrice2)
        {
            //arrange
            AuctionDTO auction1 = new AuctionDTO { AuctionId = auctionId, UserId = userId, CatalogId = catalogId, MinimumPrice = minimumPrice };
            AuctionDTO auction2 = new AuctionDTO { AuctionId = auctionId2, UserId = userId2, CatalogId = catalogId2, MinimumPrice = minimumPrice2 };

            var auctions = new List<AuctionDTO> { auction1, auction2 };

            _auctionRepositoryMock.Setup(repo => repo.GetAllAuctions()).Returns(auctions);

            //act
            var result = _auctionController.GetAllAuctions() as OkObjectResult;

            //assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var resultCollection = result.Value as IEnumerable<AuctionDTO>;
            Assert.NotNull(resultCollection);

            CollectionAssert.AreEquivalent(auctions, resultCollection);
        }


        [Test]
        public void GetAuctionsByCategory_ValidCategory_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            var auction1 = new AuctionDTO { AuctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67"), CategoryId = categoryId };
            var auction2 = new AuctionDTO { AuctionId = Guid.Parse("40e30ada-6745-4322-913f-b2e8496a8c68"), CategoryId = categoryId };
            var auctionsInCategory = new List<AuctionDTO> { auction1, auction2 };

            _auctionRepositoryMock.Setup(repo => repo.GetAuctionsByCategory(categoryId)).Returns(auctionsInCategory);

            // Act
            var result = _auctionController.GetAuctionsByCategory(categoryId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var resultCollection = result.Value as IEnumerable<AuctionDTO>;
            Assert.NotNull(resultCollection);

            CollectionAssert.AreEquivalent(auctionsInCategory, resultCollection);

            // Optionally, you can verify that the filtering by category was applied correctly.
            _auctionRepositoryMock.Verify(repo => repo.GetAuctionsByCategory(categoryId), Times.Once);
        }


        [Test]
        public void GetAuction_ValidId_ReturnsOkResult()
        {
            // Arrange -> Definer en bruger med ID 1 og navn "John Doe".
            Guid auctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67");
            var auctionDto = new AuctionDTO { AuctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67") };

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

        [Test]
        public void EditAuction_ValidData_ReturnsOkResult()
        {
            // Arrange
            var editedAuction = new AuctionDTO { AuctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67") };

            //Opsætning af mock IAuctionRepository til at returner auction, når GetAuction bliver kaldt med det specifikke ID.
            _auctionRepositoryMock.Setup(repo => repo.GetAuction(editedAuction.AuctionId)).Returns(new AuctionDTO { AuctionId = editedAuction.AuctionId });

            // Act
            var result = _auctionController.EditAuction(editedAuction) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Auction updated successfully", result.Value);

            // Verificerer at UpdateAuction-metoden på IAuctionRepository blev kaldt med det rigtige ID.
            _auctionRepositoryMock.Verify(repo => repo.UpdateAuction(editedAuction), Times.Once);
        }


        [Test]
        public void DeleteAuction_ValidId_ReturnsOkResult()
        {
            // Arrange
            Guid auctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67");

            _auctionRepositoryMock.Setup(repo => repo.GetAuction(auctionId)).Returns(new AuctionDTO { AuctionId = Guid.Parse("39e30ada-6745-4322-913f-b2e8496a8c67") });

            // Act
            var result = _auctionController.DeleteAuction(auctionId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Auction deleted successfully", result.Value);

            // Verificerer at DeleteAuction-metoden på IAuctionRepository blev kaldt med det rigtige ID.
            _auctionRepositoryMock.Verify(repo => repo.DeleteAuction(auctionId), Times.Once);
        }


    }
}
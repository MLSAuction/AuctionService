using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;
using AuctionService.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IAuctionRepository _auctionService;

        public AuctionController(ILogger<AuctionController> logger, IConfiguration configuration, IAuctionRepository auctionRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _auctionService = auctionRepository;
        }
        /// <summary>
        /// Get all auctions
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getAll")]
        public IActionResult GetAllAuctions()
        {
            var auctions = _auctionService.GetAllAuctions();

            if (auctions == null || !auctions.Any())
            {
                return NotFound(); // Return 404 if no auctions are found
            }

            return Ok(auctions);
        }

        /// <summary>
        /// Get auction by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetAuction(int id)
        {

            AuctionDTO auction = _auctionService.GetAuction(id);

            if (auction == null)
            {
                return NotFound(); // Return 404 if auction is not found
            }

            _logger.LogInformation($"Auction {auction.AuctionId} - Retrived ");

            return Ok(auction);
        }

        /// <summary>
        /// Add an auction
        /// </summary>
        /// <param name="auction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult AddAuction([FromBody] AuctionDTO auction)
        {
            if (auction == null)
            {
                //If NO "Whole-data". Example: If no texting data in the JSON. 
                return BadRequest("Invalid auction data");
            }

            auction.AuctionId = GenerateUniqueId();

            if (_auctionService.GetAuction((int)auction.AuctionId) != null)
            {
                // Handle the case where the ID already exists (e.g., generate a new ID, so it doesnt match the already exist)
                auction.AuctionId = GenerateUniqueId();
            }

            _auctionService.AddAuction(auction);

            return CreatedAtAction(nameof(GetAuction), new { id = auction.AuctionId }, auction);

        }

        /// <summary>
        /// Edit auction
        /// </summary>
        /// <param name="auction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public IActionResult EditAuction([FromBody] AuctionDTO auction)
        {
            if (auction == null)
            {
                return BadRequest("Invalid auction data");
            }

            if (_auctionService.GetAuction((int)auction.AuctionId) == null)
            {
                return BadRequest("Auction ID does not exist in database");
            }

            _auctionService.UpdateAuction(auction);

            return Ok("Auction updated successfully");
        }

        /// <summary>
        /// Delete an auction by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteAuction(int id)
        {
            var auction = _auctionService.GetAuction(id);

            if (auction == null)
            {
                return NotFound(); // Return 404 if auction is not found
            }

            _auctionService.DeleteAuction(id);

            return Ok("Auction deleted successfully");
        }

        /// <summary>
        /// Get auctions by category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("category/{categoryId}")]
        public IActionResult GetAuctionsByCategory(int categoryId)
        {
            var auctionsInCategory = _auctionService.GetAllAuctions().Where(a => a.CatalogId == categoryId).ToList();

            if (auctionsInCategory == null || !auctionsInCategory.Any())
            {
                return NotFound(); // Return 404 if no auctions are found for the specified category
            }

            return Ok(auctionsInCategory);
        }

        private int GenerateUniqueId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }
}

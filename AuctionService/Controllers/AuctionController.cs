using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;
using AuctionService.Models;
using System.Linq;

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

        [HttpGet]
        public IActionResult GetAllAuctions()
        {
            var auctions = _auctionService.GetAllAuctions();

            if (auctions == null || !auctions.Any())
            {
                return NotFound(); // Return 404 if no auctions are found
            }

            return Ok(auctions);
        }

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

        [HttpPost]
        public IActionResult AddAuction([FromBody] AuctionDTO auction)
        {
            if (auction == null)
            {
                //If NO "Whole-data". Example: If no texting data in the JSON. 
                return BadRequest("Invalid auction data");
            }

            if (auction.AuctionId == null)
            {
                //Check if there is ID 
                auction.AuctionId = GenerateUniqueId();
            }

            if (_auctionService.GetAuction((int)auction.AuctionId) != null)
            {
                // Handle the case where the ID already exists (e.g., generate a new ID, so it doesnt match the already exist)
                auction.AuctionId = GenerateUniqueId();
            }

            _auctionService.AddAuction(auction);

            return CreatedAtAction(nameof(GetAuction), new { id = auction.AuctionId }, auction);

        }

        [HttpPut("{id}")]
        public IActionResult EditAuction(int id, [FromBody] AuctionDTO auction)
        {
            if (auction == null)
            {
                return BadRequest("Invalid auction data");
            }

            if (id != auction.AuctionId)
            {
                return BadRequest("Auction ID in the request body does not match the route parameter");
            }

            _auctionService.UpdateAuction(auction);

            return Ok("Auction updated successfully");
        }

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

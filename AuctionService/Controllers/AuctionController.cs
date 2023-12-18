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
        private readonly IBiddingRepository _biddingService;

        public AuctionController(ILogger<AuctionController> logger, IConfiguration configuration, IAuctionRepository auctionRepository, IBiddingRepository biddingRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _auctionService = auctionRepository;
            _biddingService = biddingRepository;
        }
        /// <summary>
        /// Get all auctions
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
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
        public IActionResult GetAuction(Guid id)
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

            if (_auctionService.GetAuction((Guid)auction.AuctionId) != null)
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

            if (_auctionService.GetAuction((Guid)auction.AuctionId) == null)
            {
                return BadRequest("Auction ID does not exist in database");
            }

            _auctionService.UpdateAuction(auction);

            return Ok("Auction updated successfully");
        }

        /// <summary>
        /// Get all bids for specific auction
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("bids/{auctionId}")]
        public IActionResult GetAllBidsForAuction(Guid auctionId)
        {
            IEnumerable<BiddingDTO> bids = _biddingService.GetAllBidsForAuction(auctionId);

            if (!bids.Any())
            {
                return NotFound();
            }

            _logger.LogInformation($"Retrieved {bids.Count()} bids for Auction Id: {auctionId}");

            return Ok(bids);
        }

        /// <summary>
        /// Delete an auction by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteAuction(Guid id)
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
        /// Get highest-bid by auctionId
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("highestBid/{auctionId}")] //Highest bid for auctionId
        public IActionResult GetHighestBidForAuction(Guid auctionId)
        {
            BiddingDTO highestBid = _biddingService.GetHighestBidForAuction(auctionId);

            if (highestBid == null)
            {
                return NotFound(); // Return 404 if no bids are found for the specified auctionId
            }

            _logger.LogInformation($"{highestBid.BidId}, {highestBid.Price} - Highest Bid for Auction {auctionId} Retrived ");

            return Ok(highestBid);
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
            var auctionsInCategory = _auctionService.GetAuctionsByCategory(categoryId);

            if (auctionsInCategory == null || !auctionsInCategory.Any())
            {
                return NotFound(); // Return 404 if no auctions are found for the specified category
            }

            return Ok(auctionsInCategory);
        }

        /// <summary>
        /// Get bid by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("bidding/{id}")]
        public IActionResult GetBid(Guid id)
        {

            BiddingDTO bidding = _biddingService.GetBid(id);

            if (bidding == null)
            {
                return NotFound(); // Return 404 if bid is not found
            }

            _logger.LogInformation($"{bidding.BidId}, {bidding.Price} - Retrived ");

            return Ok(bidding);
        }

        /// <summary>
        /// Add bid to auction
        /// </summary>
        /// <param name="bidding"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("bidding")]
        public IActionResult AddBid([FromBody] BiddingDTO bidding)
        {
            if (bidding == null)
            {
                return BadRequest("Invalid bidding data");
            }

            bidding.BidId = GenerateUniqueId();

            if (_biddingService.GetBid((Guid)bidding.BidId) != null)
            {
                // Handle the case where the ID already exists (e.g., generate a new ID, so it doesnt match the already exist)
                bidding.BidId = GenerateUniqueId();
            }

            _biddingService.AddBid(bidding);

            return CreatedAtAction(nameof(GetBid), new { id = bidding.BidId }, bidding);

        }

        private Guid GenerateUniqueId()
        {
            return Guid.NewGuid();
        }
    }
}

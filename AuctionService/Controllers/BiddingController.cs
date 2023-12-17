﻿using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;
using AuctionService.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BiddingController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IBiddingRepository _biddingService;

        public BiddingController(ILogger<BiddingController> logger, IConfiguration configuration, IBiddingRepository biddingRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _biddingService = biddingRepository;
        }

        /// <summary>
        /// Get bid by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
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
        [HttpPost]
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

        /// <summary>
        /// Get highest-bid by auctionId
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("auction/highestBid/{auctionId}")] //Highest bid for auctionId
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
        /// Get all bids for specific auction
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("auction/{auctionId}")]
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

        private Guid GenerateUniqueId()
        {
            return Guid.NewGuid();
        }
    }
}

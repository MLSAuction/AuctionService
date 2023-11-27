using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;

namespace AuctionService.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly AuctionRepository _repository;

        public AuctionController (ILogger logger, IConfiguration configuration, AuctionRepository repository)
        {
            _logger = logger;
            _configuration = configuration;
            _repository = repository;
        }
    }
}

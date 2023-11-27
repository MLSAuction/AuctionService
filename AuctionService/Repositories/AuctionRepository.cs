namespace AuctionService.Repositories
{
    public class AuctionRepository
    {
        private readonly ILogger<AuctionRepository> _logger;
        private readonly IConfiguration _configuration;

        public AuctionRepository (ILogger<AuctionRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
    }
}

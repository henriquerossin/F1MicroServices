using F1.RaceAPI.Repositories.Interfaces;
using F1.RaceAPI.Services.Interfaces;

namespace F1.RaceAPI.Services
{
    public class RaceService : IRaceService
    {
        private readonly ILogger<RaceService> _logger;
        private readonly IRaceRepository _raceRepository;

        public RaceService(ILogger<RaceService> logger, IRaceRepository raceRepository)
        {
            _logger = logger;
            _raceRepository = raceRepository;
        }
    }
}

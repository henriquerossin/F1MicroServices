using F1.RaceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1.RaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaceController : ControllerBase
    {
        private readonly ILogger<RaceController> _logger;
        private readonly IRaceService _raceService;

        public RaceController(ILogger<RaceController> logger, IRaceService raceService)
        {
            _logger = logger;
            _raceService = raceService;
        }

        [HttpPost]
        public async Task<IActionResult> PostHistoryAsync()
        {
            try
            {
                await _raceService.EventWorkerAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while trying to start event worker.");
                throw;
            }
        }
    }
}

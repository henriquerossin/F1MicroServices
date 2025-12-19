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

        [HttpPost("/Circuit/{idCircuit}/Event/{idEvent}")]
        public async Task<IActionResult> PostHistoryAsync(int idCircuit, int idEvent)
        {
            try
            {
                await _raceService.ConsumeAndSaveHistoryAsync(idCircuit, idEvent);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Error while trying to start event worker.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("/Event/Publish")]
        public async Task<IActionResult> PublicLastEventAsync()
        {
            try
            {
                await _raceService.PublishLastEventAsync();
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}

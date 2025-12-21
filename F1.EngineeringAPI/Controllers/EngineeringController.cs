using F1.EngineeringAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1.EngineeringAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngineeringController : ControllerBase
    {
        private readonly ILogger<EngineeringController> _logger;
        private readonly IEngineeringService _engineeringService;
        public EngineeringController(ILogger<EngineeringController> logger, IEngineeringService engineeringService)
        {
            _logger = logger;
            _engineeringService = engineeringService;
        }

        [HttpPut("Engineering")]
        public async Task<IActionResult> UpdateInfosForEvent()
        {
            try
            {
                var finalConsumer = await _engineeringService.ConsumingQueueAsync();
                var finalUpdatingInfos = await _engineeringService.UpdatingInfosForEventsAsync(finalConsumer);
                var listPlacement = await _engineeringService.UpdatePlacementAsync(finalUpdatingInfos);
                //aqui faz um que vai ir um por um da ultima e passar pra producer
                foreach (var h in listPlacement)
                {
                    await _engineeringService.ProduceQueueAsync(h);
                }

                //await _engineeringService.NotifyTeamApiToUpdate();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating engineering infos for event.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

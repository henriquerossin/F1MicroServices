using F1.EngineeringAPI.Services.Interfaces;
using F1.Models.DTOs.HistoryDTOs;
using Microsoft.AspNetCore.Http;
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
                //aqui faz um que var ir um por um da ultima e passar pra producer
                foreach (var h in listPlacement)
                {
                    await _engineeringService.ProduceQueueAsync(h);
                }
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

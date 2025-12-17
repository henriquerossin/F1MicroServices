using F1.EngineeringAPI.Services.Interfaces;
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

        [HttpPut]
        public async Task<IActionResult> UpdateInfosForEvent()
        {
            try
            {
                //tratar se a lista retornar nula
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

using F1.TeamAPI.Services.Interfaces;
using F1.TeamAPI.DTOs.TeamCreation;
using Microsoft.AspNetCore.Mvc;

namespace F1.TeamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ILogger<TeamController> _logger;
        private readonly ITeamService _teamService;

        public TeamController(ILogger<TeamController> logger, ITeamService teamService)
        {
            _logger = logger;
            _teamService = teamService;
        }

        [HttpGet("validateTeam")]
        public async Task<IActionResult> ValidateTeam()
        {
            var isValid = await _teamService.ValidateTeamAsync();
            return Ok(isValid);
        }



        [HttpPost("createFullTeamManually")]
        public async Task<IActionResult> CreateFullTeam(
        [FromBody] CreateFullTeamRequestDTO dto)
        {
            await _teamService.CreateFullTeamAsync(dto);
            return Ok();
        }


        [HttpPost("createFullTeamRandom")]
        public async Task<IActionResult> CreateFullTeamRandom(
        [FromBody] CreateFullTeamRequestDTO dto)
        {
            await _teamService.CreateFullTeamRandomAsync(dto);
            return Ok();
        }

        [HttpPost("UpdateCurrentInfo")]
        public async Task<IActionResult> UpdatingCurrentInfoAsync()
        {
            try
            {
                var finalConsumer = await _teamService.ConsumingQueue();
                var finalUpdatingInfos = await _teamService.UpdatingCurrentInfo(finalConsumer);
                foreach (var h in finalUpdatingInfos)
                {
                    await _teamService.ProduceQueueAsync(h);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating team infos.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreateQueueHistory")]
        public async Task<IActionResult> CreateQueueHistoryAsync()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the history queue.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ProduceQueueHistory")]
        public async Task<IActionResult> ProduceQueueHistoryAsync()
        {
            try
            {
                await _teamService.GetAllHistoryAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while producing the history queue.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

using F1.TeamAPI.DTOs.TeamCreation;
using F1.TeamAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1.TeamAPI.Controllers
{
    [ApiController]
    [Route("team")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("validateTeam")]
        public async Task<IActionResult> ValidateTeam()
        {
            var isValid = await _teamService.ValidateTeamAsync();
            return Ok(isValid);
        }




        [HttpPost("createFullManually")]
        public async Task<IActionResult> CreateFullTeam(
        [FromBody] CreateFullTeamRequestDTO dto)
        {
            await _teamService.CreateFullTeamAsync(dto);
            return Ok();
        }

    }



}

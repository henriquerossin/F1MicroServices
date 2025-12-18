using F1.CompetitionAPI.Services.Interfaces;
using F1.Models.DTOs.CompetitionDTOs;
using F1.Models.TeamModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace F1.CompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {

        private readonly ILogger<CompetitionController> _logger;
        private readonly ICompetitionService _service;

        public CompetitionController (ILogger<CompetitionController> logger, ICompetitionService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("GetAllCircuits")]
        public async Task<ActionResult<List<GetCircuitDTO>>> GetAllCircuitsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all circuits...");
                var circuits = await _service.GetAllCircuitsAsync();
                return Ok(circuits);
            }
            catch(SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCircuitAsync(CreateCircuitDTO circuitDTO)
        {
            try
            {
                _logger.LogInformation("Creating circuit...");
                await _service.CreateCircuitAsync(circuitDTO);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpPatch("Activate/{id}")]
        public async Task<IActionResult> ActivateCircuitAsync(int id)
        {
            try
            {
                _logger.LogInformation("Activating...");
                await _service.ActivateCircuitAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpPatch("Inactivate/{id}")]
        public async Task<IActionResult> InactivateCircuitAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deactivating...");
                await _service.InactivateCircuitAsync(id);
                return Ok();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpGet("CountActives")]
        public async Task<ActionResult<int>> CountActivesAsync()
        {
            try
            {
                _logger.LogInformation("Counting...");
                var total = await _service.CountActivesAsync();

                return Ok(total);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpGet("IsTempStarted")]
        public async Task<ActionResult<bool>> IsTempStarted()
        {
            try
            {
                _logger.LogInformation("Verifying...");
                var tempIsActive = await _service.IsTempStarted();

                return Ok(tempIsActive);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpPut("StartTemp")]
        public async Task<IActionResult> StartTempAsync()
        {
            try
            {
                _logger.LogInformation("Starting...");
                await _service.StartTempAsync();
                return Ok();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpGet("GetOrdened")]
        public async Task<ActionResult<List<GetCircuitDTO>>> GetAllCircuitsActivesOrdenedAsync()
        {
            try
            {
                _logger.LogInformation("Getting all circuits ordened...");
                var circuits = await _service.GetAllCircuitsActivesOrdenedAsync();
                return Ok(circuits);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        [HttpGet("GetCircuitIdName")]
        public async Task<ActionResult<GetCircuitIdAndNameDTO>> GetCircuitIdAndName()
        {
            try
            {
                _logger.LogInformation("Getting circuit");
                var circuit = await _service.GetCircuitIdAndName();
                return Ok(circuit);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }
    }
}

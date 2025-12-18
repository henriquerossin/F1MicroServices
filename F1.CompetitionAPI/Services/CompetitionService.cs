using F1.CompetitionAPI.Controllers;
using F1.CompetitionAPI.Repositories.Interfaces;
using F1.CompetitionAPI.Services.Interfaces;
using F1.Models.CompetitionModels;
using F1.Models.DTOs.CompetitionDTOs;
using Microsoft.Data.SqlClient;

namespace F1.CompetitionAPI.Services
{
    public class CompetitionService : ICompetitionService
    {
        private readonly ILogger<CompetitionService> _logger;
        private readonly ICompetitionRepository _repository;

        public CompetitionService(ILogger<CompetitionService> logger, ICompetitionRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public async Task ActivateCircuitAsync(int id)
        {
            try
            {
                var totalActive = await CountActivesAsync();
                
                if(totalActive < 24)
                {
                    await _repository.ActivateCircuitAsync(id);
                }
                else
                {
                    _logger.LogError("Impossible to active more than 24 circuits");
                    throw new InvalidOperationException("Impossible to activate more than 24 circuits.");
                }
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

        public async Task<int> CountActivesAsync()
        {
            try
            {
                var total = await _repository.CountActivesAsync();
                return total;
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

        public async Task CreateCircuitAsync(CreateCircuitDTO circuitDTO)
        {
            try
            {
                bool beActive = false;
                int? round = null;

                if (circuitDTO.Round.HasValue)
                {
                    var totalActive = await CountActivesAsync();

                    if(totalActive < 24)
                    {
                        beActive = true;
                        round = circuitDTO.Round;
                    }
                    else
                    {
                        beActive = false;
                        round = null;
                    }
                }

                var circuit = new Circuit
                (
                    circuitDTO.Name,
                    circuitDTO.Country,
                    circuitDTO.Laps,
                    round,
                    beActive,
                    false
                );

                if(circuitDTO.Round >= 1 && circuitDTO.Round <= 24 || round is null)
                {
                    await _repository.CreateCircuitAsync(circuit);
                }
                else
                {
                    _logger.LogError("Impossible to have more than 24 rounds and less than 1");
                    throw new InvalidOperationException("Impossible to have more than 24 rounds and less than 1");
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                _logger.LogWarning(ex, "Round already in use");
                throw new InvalidOperationException("Round already in use by another active circuit.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error!");
                throw;
            }
        }

        public async Task<List<GetCircuitDTO>> GetAllCircuitsAsync()
        {
            try
            {
                return await _repository.GetAllCircuitsAsync();
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

        public async Task InactivateCircuitAsync(int id)
        {
            try
            {
                await _repository.InactivateCircuitAsync(id);
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

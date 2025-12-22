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
        private readonly IHttpClientFactory _httpClientFactory;

        public CompetitionService(ILogger<CompetitionService> logger, ICompetitionRepository repository , IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _repository = repository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task ActivateCircuitAsync(int id)
        {
            try
            {
                var tempIsActive = await IsTempStarted();

                if (tempIsActive is false)
                {

                    var totalActive = await CountActivesAsync();

                    if (totalActive < 24)
                    {
                        await _repository.ActivateCircuitAsync(id);
                    }
                    else
                    {
                        _logger.LogError("Impossible to active more than 24 circuits");
                        throw new InvalidOperationException("Impossible to activate more than 24 circuits.");
                    }
                }
                else
                {
                    _logger.LogError("Temp is alredy started");
                    throw new InvalidOperationException("Temp is alredy started");
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

        public async Task ConcludeCircuitAsync()
        {
            //AGORA ISSO NÃO QUEBRA QUANDO ESTIVER NO ULTIMO EVENTO
            try
            {
                var circuit = await _repository.GetCircuitReadyAsync();

                if(circuit.Round is not 24)
                {
                    int round1 = circuit.Round;
                    int round2 = circuit.Round + 1;

                    await _repository.ConcludeCircuitAsync(round1, round2);
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

                    if (totalActive < 24)
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

                if (circuitDTO.Round >= 1 && circuitDTO.Round <= 24 || round is null)
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

        public async Task<List<GetCircuitDTO>> GetAllCircuitsActivesOrdenedAsync()
        {
            try
            {
                return await _repository.GetAllCircuitsActivesOrdenedAsync();
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

        public async Task<GetCircuitIdAndNameDTO> GetCircuitIdAndName()
        {
            try
            {
                return await _repository.GetCircuitIdAndName();
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

        public async Task<CircuitResponseDTO> GetCircuitReadyAsync()
        {
            try
            {
                var tempIsActive = await IsTempStarted();

                if (tempIsActive is true)
                {
                    return await _repository.GetCircuitReadyAsync();
                }
                else
                {
                    _logger.LogError("Impossible to get the circuit before temp start");
                    throw new InvalidOperationException("Impossible to get the circuit before temp start");
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

        public async Task InactivateCircuitAsync(int id)
        {
            try
            {
                var tempIsActive = await IsTempStarted();

                if (tempIsActive is false)
                {
                    await _repository.InactivateCircuitAsync(id);

                }
                else
                {
                    _logger.LogError("Temp is alredy started");
                    throw new InvalidOperationException("Temp is alredy started");
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

        public async Task<bool> IsTempStarted()
        {
            try
            {
                var circuit = await _repository.IsTempStarted();
                var tempIsActive = true;

                if (circuit is null)
                {
                    tempIsActive = false;
                }

                return tempIsActive;
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

        public async Task StartTempAsync()
        {
            try
            {
                var tempIsActive = await IsTempStarted();

                if (tempIsActive is true)
                {
                    _logger.LogError("Temp is alredy started");
                    throw new InvalidOperationException("Temp is alredy started");
                }
                else
                {
                    var totalActive = await CountActivesAsync();
                    if (totalActive != 24)
                    {
                        _logger.LogError("Temp only starts with 24 circuits");
                        throw new InvalidOperationException("Temp only starts with 24 circuits");
                    }
                    else
                    {
                        var cadastroCompleto = true; await ValidateTeamAsync(); // validação que pego no endpoint do pedro 
                        if (cadastroCompleto is false)
                        {
                            _logger.LogError("Temporada só começa com o cadastros completo de todas as equipes");
                            throw new InvalidOperationException("Temporada só começa com o cadastros completo de todas as equipes");
                        }
                        else
                        {
                            //var circuits = await _repository.GetAllCircuitsActivesOrdenedAsync();
                            await _repository.StartTemp();
                            await CallingProduceQueueHistoryAsync();

                            //_ = PostHistoryAsync();   
                        }
                    }
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

        public async Task CallingProduceQueueHistoryAsync()
        {
            var client = _httpClientFactory.CreateClient("TeamClient");
            await client.PostAsync("ProduceQueueHistory", null);
        }

        public async Task PostHistoryAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("RaceClient");
                await client.PostAsync("Circuit/1/Event/1", null);
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

        public async Task<bool> ValidateTeamAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("TeamClient");

                var response = await client.GetAsync("validateTeam");

                return await response.Content.ReadFromJsonAsync<bool>();
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

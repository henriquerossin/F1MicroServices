using Dapper;
using F1.CompetitionAPI.Data;
using F1.CompetitionAPI.Repositories.Interfaces;
using F1.Models.CompetitionModels;
using F1.Models.DTOs.CompetitionDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace F1.CompetitionAPI.Repositories
{
    public class CompetitionRepository : ICompetitionRepository
    {
        private readonly ILogger<CompetitionRepository> _logger;
        private readonly SqlConnection _connection;

        public CompetitionRepository(ILogger<CompetitionRepository> logger, ConnectionDB connection)
        {
            _logger = logger;
            _connection = connection.GetConnection();
        }

        public async Task ActivateCircuitAsync(int id)
        {
            try
            {
                var sql = @"UPDATE Circuit SET Active = 1 WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, new { Id = id });
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

        public async Task ConcludeCircuitAsync(int round)
        {
            try
            {
                //tratar logica no repository - passar pra service?
                int round1 = round;
                int round2 = round + 1;

                var sql1 = @"UPDATE Circuit SET Ready = 0 WHERE [Round] = @Round";
                await _connection.ExecuteAsync(sql1, new { Round = round1 });

                var sql2 = @"UPDATE Circuit SET Ready = 1 WHERE [Round] = @Round";
                await _connection.ExecuteAsync(sql2, new { Round = round2 });
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

        public Task<int> CountActivesAsync()
        {
            try
            {
                var sql = @"SELECT COUNT(*) FROM Circuit WHERE Active = 1";

                var total = _connection.ExecuteScalarAsync<int>(sql);

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

        public async Task CreateCircuitAsync(Circuit circuit)
        {
            try
            {
                var sql = @"INSERT INTO Circuit([Name], Country, Laps, Round, Active, Ready) VALUES(@name, @country, @laps, @round, @active, @ready)";

                await _connection.ExecuteAsync(sql, new { name = circuit.Name, country = circuit.Country, laps = circuit.Laps, round = circuit.Round, active = circuit.Active, ready = circuit.Ready });
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

        public async Task<ActionResult<List<GetCircuitDTO>>> GetAllCircuitsActivesOrdenedAsync()
        {

            try
            {
                var sql = "SELECT Id, [Name], Country, Laps, Round, Active FROM Circuit WHERE Active = 1 ORDER BY [Round]";

                return (await _connection.QueryAsync<GetCircuitDTO>(sql)).ToList();
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
            var sql = "SELECT Id, [Name], Country, Laps, Round, Active FROM Circuit";

            try
            {
                return (await _connection.QueryAsync<GetCircuitDTO>(sql)).ToList();
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
                var sql = @"SELECT Id, [Name] FROM Circuit WHERE Ready = 1";

                var circuit = await _connection.QueryFirstOrDefaultAsync<GetCircuitIdAndNameDTO>(sql);

                return circuit;
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
                var sql = @"SELECT Id, [Name], Country, Laps, Round, Active, Ready FROM Circuit WHERE Ready = 1";

                var circuit = await _connection.QueryFirstOrDefaultAsync<CircuitResponseDTO>(sql);

                return circuit;
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
                var sql = @"UPDATE Circuit SET Active = 0, Round = NULL WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, new { Id = id });
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

        public async Task<Circuit> IsTempStarted()
        {
            try
            {
                var sql = @"SELECT TOP 1 * FROM Circuit WHERE Ready = 1";

                return await _connection.QueryFirstOrDefaultAsync<Circuit>(sql);
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

        public async Task StartTemp()
        {
            try
            {
                var sql = @"UPDATE Circuit SET Ready = 1 WHERE [Round] = 1";

                await _connection.ExecuteAsync(sql);
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

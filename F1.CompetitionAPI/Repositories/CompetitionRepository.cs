using Dapper;
using F1.CompetitionAPI.Data;
using F1.CompetitionAPI.Repositories.Interfaces;
using F1.Models.CompetitionModels;
using F1.Models.DTOs.CompetitionDTOs;
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

                await _connection.ExecuteAsync(sql, new { Id = id});
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

                await _connection.ExecuteAsync(sql, new { name = circuit.Name, country = circuit.Country, laps = circuit.Laps,  round = circuit.Round, active = circuit.Active, ready = circuit.Ready });
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
    }
}

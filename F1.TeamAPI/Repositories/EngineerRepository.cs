using Dapper;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class EngineerRepository : IEngineerRepository
    {
        private readonly SqlConnection _connection;

        public EngineerRepository(ConnectionDB c)
        {
            _connection = c.GetSlqConnection();
        }

        public async Task CreateEngineerAsync(EngineerRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    INSERT INTO Engineer
                    (
                        Name,
                        Surname,
                        Age,
                        Experience,
                        Type,
                        TeamId,
                        CarId
                    )
                    VALUES
                    (
                        @Name,
                        @Surname,
                        @Age,
                        @Experience,
                        @Type,
                        @TeamId,
                        @CarId
                    );
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    dto.Name,
                    dto.Surname,
                    dto.Age,
                    dto.Experience,
                    dto.Type,
                    dto.TeamId,
                    dto.CarId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao registrar engenheiro: " + ex.Message);
            }
        }

        public async Task DeleteEngineerAsync(int id)
        {
            try
            {
                const string sql = @"
                    UPDATE Engineer
                    SET IsActive = 0
                    WHERE Id = @Id;
                ";

                await _connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar engenheiro: " + ex.Message);
            }
        }

        public async Task<List<EngineerResponseDTO>> GetAllEngineersAsync()
        {
            try
            {
                const string sql = @"
                    SELECT
                        Id,
                        Name,
                        Surname,
                        Age,
                        Experience,
                        Type,
                        TeamId,
                        CarId
                    FROM Engineer
                    WHERE IsActive = 1;
                ";

                return (await _connection.QueryAsync<EngineerResponseDTO>(sql)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter engenheiros: " + ex.Message);
            }
        }

        public async Task<List<EngineerResponseDTO>> GetEngineersByTeamAsync(int teamId)
        {
            try
            {
                const string sql = @"
                    SELECT
                        e.Id,
                        e.Name,
                        e.Surname,
                        e.Age,
                        e.Experience,
                        e.Type,
                        e.Status,
                        e.TeamId,
                        e.CarId
                    FROM Engineer e
                    INNER JOIN Team t ON t.Id = e.TeamId
                    WHERE
                        e.TeamId = @TeamId
                        AND e.IsActive = 1
                        AND t.IsActive = 1;
                ";

                return (await _connection.QueryAsync<EngineerResponseDTO>(
                    sql,
                    new { TeamId = teamId }
                )).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter engenheiros do time: " + ex.Message);
            }
        }

        public async Task UpdateEngineerAsync(int id, EngineerRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    UPDATE Engineer
                    SET
                        Name = @Name,
                        Surname = @Surname,
                        Age = @Age,
                        Experience = @Experience,
                        Type = @Type,
                        TeamId = @TeamId,
                        CarId = @CarId
                    WHERE
                        Id = @Id
                        AND IsActive = 1;
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    dto.Name,
                    dto.Surname,
                    dto.Age,
                    dto.Experience,
                    dto.Type,
                    dto.TeamId,
                    dto.CarId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar engenheiro: " + ex.Message);
            }
        }
    }
}

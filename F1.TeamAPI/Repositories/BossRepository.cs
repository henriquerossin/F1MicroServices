using Dapper;
using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class BossRepository : IBossRepository
    {
        private readonly SqlConnection _connection;

        public BossRepository(ConnectionDB c)
        {
            _connection = c.GetSlqConnection();
        }

        public async Task CreateBossAsync(BossRequestDTO dto)
        {
            try
            {
                var sql = @"
                    INSERT INTO Boss
                    (Name, Surname, Age, Type, Status, TeamId)
                    VALUES
                    (@Name, @Surname, @Age, @Type, @Status, @TeamId);
                ";

                await _connection.ExecuteAsync(sql, dto);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao registrar chefe: " + ex.Message);
            }
        }

        public async Task DeleteBossAsync(int id)
        {
            try
            {
                var sql = @"UPDATE Boss SET IsActive = 0 WHERE Id = @Id;";

                await _connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar chefe: " + ex.Message);
            }
        }

        public async Task<List<BossResponseDTO>> GetAllBossesAsync()
        {
            try
            {
                var sql = @"
                    SELECT
                        Id,
                        Name,
                        Surname,
                        Age,
                        Type,
                        Status,
                        TeamId
                    FROM Boss
                    WHERE IsActive = 1;
                ";

                return (await _connection.QueryAsync<BossResponseDTO>(sql)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter chefes: " + ex.Message);
            }
        }

        public async Task<List<BossResponseDTO>> GetBossesByTeamAsync(int teamId)
        {
            try
            {
                var sql = @"
                    SELECT
                        b.Id,
                        b.Name,
                        b.Surname,
                        b.Age,
                        b.Type,
                        b.Status,
                        b.TeamId
                    FROM Boss b
                    INNER JOIN Team t ON t.Id = b.TeamId
                    WHERE b.TeamId = @TeamId
                      AND b.IsActive = 1
                      AND t.IsActive = 1;
                ";

                return (await _connection.QueryAsync<BossResponseDTO>(
                    sql,
                    new { TeamId = teamId }
                )).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter chefe do time: " + ex.Message);
            }
        }

        public async Task UpdateBossAsync(int id, BossRequestDTO dto)
        {
            try
            {
                var sql = @"
                    UPDATE Boss
                    SET
                        Name = @Name,
                        Surname = @Surname,
                        Age = @Age,
                        Type = @Type,
                        Status = @Status,
                        TeamId = @TeamId
                    WHERE Id = @Id
                      AND IsActive = 1;
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    dto.Name,
                    dto.Surname,
                    dto.Age,
                    dto.Type,
                    dto.Status,
                    dto.TeamId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar chefe: " + ex.Message);
            }
        }
    }
}

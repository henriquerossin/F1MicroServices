using Dapper;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class PilotRepository : IPilotRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<PilotRepository> _logger;

        public PilotRepository(ConnectionDB c, ILogger<PilotRepository> logger)
        {
            _connection = c.GetSlqConnection();
            _logger = logger;
        }

        public async Task CreatePilotAsync(PilotRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    INSERT INTO Pilot
                    (
                        Name,
                        Surname,
                        Weight,
                        Age,
                        IdentificationNumber,
                        Status,
                        Experience,
                        Handicap,
                        Points,
                        TeamId,
                    )
                    VALUES
                    (
                        @Name,
                        @Surname,
                        @Weight,
                        @Age,
                        @IdentificationNumber,
                        1,
                        @Experience,
                        @Handicap,
                        @TeamId,
                    );
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    dto.Name,
                    dto.Surname,
                    dto.Weight,
                    dto.Age,
                    dto.IdentificationNumber,
                    dto.Experience,
                    dto.Handicap,
                    dto.TeamId,
                    dto.Position
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao registrar piloto: " + ex.Message);
            }
        }

        public async Task DeletePilotAsync(int id)
        {
            try
            {
                const string sql = @"
                    UPDATE Pilot
                    SET Status = 0
                    WHERE Id = @Id;
                ";

                await _connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar piloto: " + ex.Message);
            }
        }

        public async Task<List<PilotResponseDTO>> GetAllPilotsAsync()
        {
            try
            {
                const string sql = @"
                    SELECT
                        Id,
                        Name,
                        Surname,
                        Age,
                        Weight,
                        Points,
                        Position
                    FROM Pilot
                    WHERE Status = 1;
                ";

                var pilots = (await _connection.QueryAsync<PilotResponseDTO>(sql)).ToList();
                return pilots;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter pilotos: " + ex.Message);
            }
        }


        public async Task<List<PilotResponseDTO>> GetAllPilotsFinalAsync()
        {
            try
            {
                const string sql = @"
                    SELECT
                        Id,
                        Name,
                        Surname,
                        Age,
                        Weight,
                        Points,
                        Position
                    FROM Pilot
                    WHERE Status = 1;
                    ORDER BY Points;
                ";

                var pilots = (await _connection.QueryAsync<PilotResponseDTO>(sql)).ToList();
                return pilots;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter pilotos: " + ex.Message);
            }
        }

        public async Task<List<PilotResponseDTO>> GetPilotsByTeamAsync(int teamId)
        {
            try
            {
                const string sql = @"
                    SELECT
                        p.Id,
                        p.Name,
                        p.Surname,
                        p.Age,
                        p.Weight,
                        p.Points,
                        p.Position
                    FROM Pilot p
                    INNER JOIN Team t ON t.Id = p.TeamId
                    WHERE
                        p.TeamId = @TeamId
                        AND p.Status = 1
                        AND t.IsActive = 1;
                ";

                var pilots = (await _connection.QueryAsync<PilotResponseDTO>(
                    sql,
                    new { TeamId = teamId }
                )).ToList();

                return pilots;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter pilotos do time: " + ex.Message);
            }
        }

        public async Task UpdatePilotAsync(int id, PilotRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    UPDATE Pilot
                    SET
                        Name = @Name,
                        Surname = @Surname,
                        Weight = @Weight,
                        Age = @Age,
                        IdentificationNumber = @IdentificationNumber,
                        Experience = @Experience,
                        Handicap = @Handicap,
                        Points = @Points,
                        TeamId = @TeamId,
                        Position = @Position
                    WHERE
                        Id = @Id
                        AND Status = 1;
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    dto.Name,
                    dto.Surname,
                    dto.Weight,
                    dto.Age,
                    dto.IdentificationNumber,
                    dto.Experience,
                    dto.Handicap,
                    dto.Points,
                    dto.TeamId,
                    dto.Position
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar piloto: " + ex.Message);
            }
        }

        public async Task UpdatePilotHandicapAndPointsAsync(
            int pilotId,
            decimal handicap,
            int points)
        {
            const string sql = @"
                UPDATE Pilot
                SET
                    Handicap = @Handicap,
                    Points = @Points
                WHERE
                    Id = @PilotId
                    AND Status = 1;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                PilotId = pilotId,
                Handicap = handicap,
                Points = points
            });
        }

        public async Task<List<PilotHistoryResponseDTO>> GetAllPilotsHistoryAsync()
        {
            var sql =
                @"SELECT Id as PilotId, Name as PilotName, Handicap as PilotHandicap, Points as PilotPoints, Position as PilotPlacement, Experience, TeamId
                FROM Pilot";

            var pilots = await _connection.QueryAsync<PilotHistoryResponseDTO>(sql);
            return pilots.ToList();
        }
    }
}

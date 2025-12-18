using Dapper;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class PilotRepository : IPilotRepository
    {
        public readonly SqlConnection _connection;
        public PilotRepository(ConnectionDB c)
        {
            _connection = c.GetSlqConnection();
        }
        public async Task CreatePilotAsync(PilotRequestDTO dto)
        {
            try
            {
                var sql = @"INSERT INTO Pilot (Name, Surname, Weight, Age, IdentificationNumber, Status, Experience, Handicap, Points, TeamId, Position, IsActive)
                       VALUES (@Name, @Surname, @Weight, @Age, @IdentificationNumber, @Status, @Experience, @Handicap, @Points, @TeamId, @Position, @IsActive)";
                await _connection.ExecuteAsync(sql, new { dto.Name, dto.Surname, dto.Weight, dto.Age, dto.IdentificationNumber, dto.Status, dto.Experience, dto.Handicap, dto.Points, dto.TeamId, dto.IsActive });
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
                var sql = "UPDATE Pilot Set IsActive = 0 WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar time" + ex.Message);
            }
        }

        public async Task<List<PilotResponseDTO>> GetAllPilotsAsync()
        {
            try
            {
                var sql = @"SELECT 
                          Id,
                          Name,
                          Surname,
                          Age,
                          Weight,
                          Points,
                          Position
                          FROM Pilot
                          WHERE IsActive = 1;";
                var p = (await _connection.QueryAsync<PilotResponseDTO>(sql)).ToList();

                return p;
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
                var sql = @"SELECT 
                           p.Id,
                           p.Name,
                           p.Surname,
                           p.Age,
                           p.Weight,
                           p.Points,
                           p.Position
                           FROM Pilot p
                           INNER JOIN Team t ON t.Id = p.TeamId
                           WHERE p.TeamId = @TeamId
                           AND p.IsActive = 1
                           AND t.IsActive = 1;";

                var pilots = (await _connection.QueryAsync<PilotResponseDTO>(sql,
                    new { TeamId = teamId })).ToList();

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
                var sql = @"
            UPDATE Pilot
            SET
                Name = @Name,
                Surname = @Surname,
                Weight = @Weight,
                Age = @Age,
                IdentificationNumber = @IdentificationNumber,
                Status = @Status,
                Experience = @Experience,
                Handicap = @Handicap,
                Points = @Points,
                TeamId = @TeamId,
                Position = @Position
            WHERE Id = @Id
              AND IsActive = 1;
        ";

                await _connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    dto.Name,
                    dto.Surname,
                    dto.Weight,
                    dto.Age,
                    dto.IdentificationNumber,
                    dto.Status,
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
                               WHERE Id = @PilotId;";

            await _connection.ExecuteAsync(sql, new
            {
                PilotId = pilotId,
                Handicap = handicap,
                Points = points
            });
        }




    }
}

using Dapper;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        public readonly SqlConnection _connection;

        public TeamRepository(ConnectionDB c)
        {
            _connection = c.GetSlqConnection();
        }

        public async Task<List<TeamResponseDTO>> GetAllTeamsAsync()
        {
            try//vai se tratar gatooota
            {
                var sql = @"SELECT Id, Name, Points, Placement 
                           FROM Team 
                           WHERE IsActive = 1;";
                var teams = (await _connection.QueryAsync<TeamResponseDTO>(sql)).ToList();

                return teams;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter times: " + ex.Message);
            }

        }

        public async Task CreateTeamAsync(Team team)
        {
            try
            {
                var sql = @"INSERT INTO Team (Name, Points, Placement, IsActive)
                          VALUES (@Name, @Points, @Placement, @IsActive)";

                var id = await _connection.ExecuteScalarAsync<int>(sql, team);

                typeof(Team).GetProperty(nameof(Team.Id))!.SetValue(team, id);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao registrar time: " + ex.Message);
            }
        }

        public async Task DeleteTeamAsync(int id)
        {
            try
            {
                var sql = "UPDATE Team Set IsActive = 0 WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, new { id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar time" + ex.Message);
            }
        }


        public async Task UpdateTeamPlacementAndPointsAsync(int teamId, int placement, int points)
        {
            const string sql = @"
                               UPDATE Team
                               SET 
                               Placement = @Placement,
                               Points = @Points
                               WHERE Id = @TeamId;";

            await _connection.ExecuteAsync(sql, new
            {
                TeamId = teamId,
                Placement = placement,
                Points = points
            });
        }


    }
}


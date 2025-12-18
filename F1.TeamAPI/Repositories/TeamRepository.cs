using Dapper;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
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
            try
            {
                var sql = "SELECT Name, Points, Placement FROM Team;";
                var teams = (await _connection.QueryAsync<TeamResponseDTO>(sql)).ToList();

                return teams;
            }
            catch (Exception ex)
            {
                {
                    throw new Exception("Erro ao obter times: " + ex.Message);
                }
            }
        }


        public async Task CreateTeamAsync(TeamRequestDTO dto)
        {
            try{
                var sql = @"INSERT INTO TEAM (Name, Points, Placement)
                          VALUES (@Name, @Points, @Placement)";

            await _connection.ExecuteAsync(sql, new { dto.Name, dto.Points, dto.Placement}); //usuario so informa o nome, o resto vai 0 pelo contrutor
            } catch
            {

            }
        }

        public Task UpdateTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTeamAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

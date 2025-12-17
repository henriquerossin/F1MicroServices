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
        public Task CreatePilotAsync(PilotRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeletePilotAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PilotResponseDTO>> GetAllPilotsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PilotResponseDTO> GetPilotByTeamAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePilotAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

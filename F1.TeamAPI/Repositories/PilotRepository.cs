using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Repositories
{
    public class PilotRepository : IPilotRepository
    {
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

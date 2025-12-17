using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Repositories
{
    public class EngineerRepository : IEngineerRepository
    {
        public Task CreateEngineerAsync(EngineerRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEngineerAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<EngineerResponseDTO>> GetAllEngineersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EngineerResponseDTO> GetEngineersByTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateEngineerAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

using F1.Models.DTOs.TeamDTOs.EngineerDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IEngineerRepository
    {
        Task<List<EngineerResponseDTO>> GetAllTeamsAsync();
        Task<EngineerResponseDTO> GetTeamByIdAsync(int id);
        Task<EngineerResponseDTO> GetTeamByNameAsync(string name);
        Task CreateTeamAsync(EngineerRequestDTO dto);
        Task UpdateTeamAsync(int id);
        Task DeleteTeamAsync(int id);
    }
}

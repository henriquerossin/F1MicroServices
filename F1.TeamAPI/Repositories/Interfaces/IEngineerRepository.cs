using F1.Models.DTOs.TeamDTOs.EngineerDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IEngineerRepository
    {
        Task<List<EngineerResponseDTO>> GetAllEngineersAsync();
        Task<List<EngineerResponseDTO>> GetEngineersByTeamAsync(int id);
        Task CreateEngineerAsync(EngineerRequestDTO dto);
        Task UpdateEngineerAsync(int id, EngineerRequestDTO dto);
        Task DeleteEngineerAsync(int id);
        Task<List<EngineerHistoryResponseDTO>> GetAllEngineersHistoryAsync();
    }
}

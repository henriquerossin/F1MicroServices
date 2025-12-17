using F1.Models.DTOs.TeamDTOs.BossDTOs;
namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IBossRepository
    {
        Task<List<BossResponseDTO>> GetAllTeamsAsync();
        Task<BossResponseDTO> GetTeamByIdAsync(int id);
        Task<BossResponseDTO> GetTeamByNameAsync(string name);
        Task CreateTeamAsync(BossRequestDTO dto);
        Task UpdateTeamAsync(int id);
        Task DeleteTeamAsync(int id);
    }
}

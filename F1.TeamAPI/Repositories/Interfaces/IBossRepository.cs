using F1.Models.DTOs.TeamDTOs.BossDTOs;
namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IBossRepository
    {
        Task<List<BossResponseDTO>> GetAllBosseAsync();
        Task<BossResponseDTO> GetBossByIdAsync(int id);
        Task<BossResponseDTO> GetBossesByTeamAsync();
        Task CreateBossAsync(BossRequestDTO dto);
        Task UpdateBossAsync(int id);
        Task DeleteBossAsync(int id);
    }
}

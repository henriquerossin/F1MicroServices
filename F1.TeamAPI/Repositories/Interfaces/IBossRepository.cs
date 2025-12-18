using F1.Models.DTOs.TeamDTOs.BossDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IBossRepository
    {
        Task<List<BossResponseDTO>> GetAllBossesAsync();
        Task<List<BossResponseDTO>> GetBossesByTeamAsync(int teamId);
        Task CreateBossAsync(BossRequestDTO dto);
        Task UpdateBossAsync(int id, BossRequestDTO dto);
        Task DeleteBossAsync(int id);
    }
}

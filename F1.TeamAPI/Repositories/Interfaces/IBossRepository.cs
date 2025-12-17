using F1.Models.DTOs.TeamDTOs.BossDTOs;
namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IBossRepository
    {
        Task<List<BossResponseDTO>> GetAllBossesAsync();
        Task<BossResponseDTO> GetBossesByTeamAsync(int id);
        Task CreateBossAsync(BossRequestDTO dto);
        Task UpdateBossAsync(int id);
        Task DeleteBossAsync(int id);
    }
}

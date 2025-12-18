using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Repositories
{
    public class BossRepository : IBossRepository
    {
        public Task CreateBossAsync(BossRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBossAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<BossResponseDTO>> GetAllBossesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BossResponseDTO> GetBossesByTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBossAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

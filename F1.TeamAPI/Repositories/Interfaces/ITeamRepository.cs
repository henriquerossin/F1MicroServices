using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamResponseDTO>> GetAllTeamsAsync();
        Task CreateTeamAsync(TeamRequestDTO dto);
        Task DeleteTeamAsync(int id);
    }
}

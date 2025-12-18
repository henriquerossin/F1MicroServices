using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamResponseDTO>> GetAllTeamsAsync();
        Task CreateTeamAsync(Team team);
        Task DeleteTeamAsync(int id);
    }
}

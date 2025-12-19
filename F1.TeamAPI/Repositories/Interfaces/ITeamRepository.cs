using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;
using F1.TeamAPI.DTOs.TeamCreation;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamResponseDTO>> GetAllTeamsAsync();
        Task CreateTeamAsync(TeamRequestDTO dto);
        Task DeleteTeamAsync(int id);

        Task UpdateTeamPlacementAndPointsAsync(int teamId, int placement, int points);
        Task CreateFullTeamAsync(CreateFullTeamRequestDTO dto);
        Task<List<TeamResponseDTO>> GetAllTeamsFinalAsync();
    }
}

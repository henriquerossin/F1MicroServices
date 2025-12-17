using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamResponseDTO>> GetAllTeamsAsync();
        Task<TeamResponseDTO> GetTeamByIdAsync(int id);
        Task<TeamResponseDTO> GetTeamByNameAsync(string name);
        Task CreateTeamAsync(TeamRequestDTO dto);
        Task UpdateTeamAsync(int id);
        Task DeleteTeamAsync(int id);
    }
}

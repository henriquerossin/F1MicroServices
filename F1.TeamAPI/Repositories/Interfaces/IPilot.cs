using F1.Models.DTOs.TeamDTOs.PilotDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IPilot
    {
        Task<List<PilotResponseDTO>> GetAllTeamsAsync();
        Task<PilotResponseDTO> GetTeamByIdAsync(int id);
        Task<PilotResponseDTO> GetTeamByNameAsync(string name);
        Task CreateTeamAsync(PilotRequestDTO dto);
        Task UpdateTeamAsync(int id);
        Task DeleteTeamAsync(int id);
    }
}

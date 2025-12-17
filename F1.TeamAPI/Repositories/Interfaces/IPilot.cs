using F1.Models.DTOs.TeamDTOs.PilotDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IPilot
    {
        Task<List<PilotResponseDTO>> GetAllPilotsAsync();
        Task<PilotResponseDTO> GetPilotByTeamAsync(string name);
        Task CreatePilotAsync(PilotRequestDTO dto);
        Task UpdatePilotAsync(int id);
        Task DeletePilotAsync(int id);
    }
}

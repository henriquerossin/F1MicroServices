using F1.Models.DTOs.TeamDTOs.PilotDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface IPilotRepository
    {
        Task<List<PilotResponseDTO>> GetAllPilotsAsync();
        Task<List<PilotResponseDTO>> GetPilotsByTeamAsync(int id);
        Task CreatePilotAsync(PilotRequestDTO dto);
        Task UpdatePilotAsync(int id, PilotRequestDTO pilot);
        Task DeletePilotAsync(int id);

        Task UpdatePilotHandicapAsync(int pilotId, decimal handicap, int points);
    }
}

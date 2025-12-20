using F1.Models.DTOs.HistoryDTOs;

namespace F1.RaceAPI.Repositories.Interfaces
{
    public interface IRaceRepository
    {
        Task SaveEventAsync(FinalHistoryResponseDTO history);
        Task<FinalHistoryResponseDTO> GetLastEventAsync();
        Task<FinalHistoryResponseDTO?> GetOneFinalHistory(int idCircuit, int idEvent);
        Task<bool> GetLastCircuitAsync(int idCircuit);
    }
}

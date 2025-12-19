using F1.Models.DTOs.HistoryDTOs;

namespace F1.RaceAPI.Services.Interfaces
{
    public interface IRaceService
    {
        Task ConsumeAndSaveHistoryAsync(int idRound, int idEvent);
        Task PublishLastEventAsync();
        Task<CircuitHistoryIdNameResponseDTO?> GetCircuitIdName();
    }
}

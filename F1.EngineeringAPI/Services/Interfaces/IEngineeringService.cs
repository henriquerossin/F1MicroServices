using F1.Models.DTOs.HistoryDTOs;

namespace F1.EngineeringAPI.Services.Interfaces
{
    public interface IEngineeringService
    {
        Task<FinalHistoryResponseDTO> ConsumingQueueAsync(CancellationToken cancellationToken = default);
        Task<FinalHistoryResponseDTO> UpdatingInfosForEventsAsync(FinalHistoryResponseDTO finalHistory);
        Task<List<HistoryDTO>> UpdatePlacementAsync(FinalHistoryResponseDTO finalHistory);
        Task ProduceQueueAsync(HistoryDTO history);
    }
}

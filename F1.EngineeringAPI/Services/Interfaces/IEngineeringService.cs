using F1.Models.DTOs.HistoryDTOs;

namespace F1.EngineeringAPI.Services.Interfaces
{
    public interface IEngineeringService
    {
        Task<List<HistoryDTO>> ConsumingQueueAsync();
        Task UpdatingInfosForEventsAsync(List<HistoryDTO> listHistories);
        Task ProduceQueueAsync();
    }
}

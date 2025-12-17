using F1.Models.DTOs.HistoryDTOs;

namespace F1.RaceAPI.Services.Interfaces
{
    public interface IRaceService
    {
        Task SaveEventAsync();
    }
}

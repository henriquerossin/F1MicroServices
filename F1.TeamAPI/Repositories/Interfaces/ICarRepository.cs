using F1.Models.DTOs.TeamDTOs.CarDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<List<CarResponseDTO>> GetAllCarsAsync();
        Task<List<CarResponseDTO>> GetCarsByTeamAsync(int teamId);
        Task CreateCarAsync(CarRequestDTO dto);
        Task UpdateCarAsync(int id, CarRequestDTO dto);
        Task DeleteCarAsync(int id);
        Task UpdateCACPByPilotIdAsync(int id, decimal ca, decimal cp);
        Task<List<CarHistoryResponseDTO>> GetAllCarsHistoryAsync();
    }
}
